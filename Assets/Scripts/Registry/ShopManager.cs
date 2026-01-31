using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Random = System.Random;

namespace Registry
{
    public class ShopItemData
    {
        public int Price = 0;
        public int Level = 0;
    }
    
    public class ShopConfigData
    {
        public int SlotCount { get; set; }
        public float ItemRatio { get; set; }

        public Dictionary<string, ShopItemData> CardPriceTable { get; set; } = new();
        public Dictionary<string, ShopItemData> ItemPriceTable { get; set; } = new();
    }
    
    public class ShopEntry
    {
        public string Name;
        public bool IsCard;
        public int Price;
        public int Level;
    }
    
    public class ShopManager
    {
        private bool _hasLoaded = false;

        private static readonly string ShopJsonPath = Path.Combine(Application.dataPath, "../GameData/Shop/price.json");

        private ShopConfigData _config = new();

        public void DebugLoadedShopInfo()
        {
            Debug.Log($"[ShopManager] Loaded shop config: Slot={_config.SlotCount}, ItemRatio={_config.ItemRatio}");
            Debug.Log($"[ShopManager] Card Prices Count={_config.CardPriceTable.Count}");
            Debug.Log($"[ShopManager] Item Prices Count={_config.ItemPriceTable.Count}");
        }

        public void LoadFromFile()
        {
            if (_hasLoaded) return;
            _hasLoaded = true;

            if (!File.Exists(ShopJsonPath))
            {
                Debug.LogError($"[ShopManager] shop.json not found: {ShopJsonPath}");
                return;
            }

            try
            {
                var json = File.ReadAllText(ShopJsonPath);
                var data = JsonConvert.DeserializeObject<ShopConfigData>(json);

                if (data == null)
                {
                    Debug.LogError("[ShopManager] JSON structure invalid.");
                    return;
                }

                _config = data;
            }
            catch (Exception e)
            {
                Debug.LogError($"[ShopManager] Failed to parse price.json\n{e}");
                return;
            }

            DebugLoadedShopInfo();
        }
        
        public List<ShopEntry> GetShopEntries(int level, Random rand = null)
        {
            rand ??= new Random();
            var result = new List<ShopEntry>();

            var cardPool = (from kv in _config.CardPriceTable where kv.Value.Level <= level select kv.Key).ToList();
            var itemPool = (from kv in _config.ItemPriceTable where kv.Value.Level <= level select kv.Key).ToList();

            for (var i = 0; i < _config.SlotCount; i++)
            {
                if (rand.NextDouble() < _config.ItemRatio)
                {
                    if (itemPool.Count == 0) break;
                    var index = rand.Next(itemPool.Count);
                    var name = itemPool[index];
                    itemPool.RemoveAt(index);

                    var data = _config.ItemPriceTable[name];
                    result.Add(new ShopEntry
                    {
                        Name = name,
                        IsCard = false,
                        Price = data.Price,
                        Level = data.Level
                    });
                }
                else
                {
                    if (cardPool.Count == 0) break;
                    var index = rand.Next(cardPool.Count);
                    var name = cardPool[index];
                    cardPool.RemoveAt(index);

                    var data = _config.CardPriceTable[name];
                    result.Add(new ShopEntry
                    {
                        Name = name,
                        IsCard = true,
                        Price = data.Price,
                        Level = data.Level
                    });
                }
            }

            return result;
        }
    }
}