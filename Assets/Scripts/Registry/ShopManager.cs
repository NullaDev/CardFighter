using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Random = System.Random;

namespace Registry
{
    public class ShopConfigData
    {
        public int SlotCount { get; set; }
        public float ItemRatio { get; set; }

        public Dictionary<string, int> CardPriceTable { get; set; } = new();
        public Dictionary<string, int> ItemPriceTable { get; set; } = new();
    }
    
    public class ShopEntry
    {
        public string Name;
        public bool IsCard;
        public int Price;
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
        
        public List<ShopEntry> GetShopEntries(Random rand)
        {
            rand ??= new Random();
            
            var result = new List<ShopEntry>();

            var cardPool = new List<string>(_config.CardPriceTable.Keys);
            var itemPool = new List<string>(_config.ItemPriceTable.Keys);
            for (var i = 0; i < _config.SlotCount; i++)
            {
                if (rand.NextDouble() < _config.ItemRatio)
                {
                    if (itemPool.Count == 0) break;
                    var index = rand.Next(itemPool.Count);
                    var name = itemPool[index];
                    itemPool.RemoveAt(index);

                    result.Add(new ShopEntry()
                    {
                        Name = name,
                        IsCard = false,
                        Price = _config.ItemPriceTable[name]
                    });
                }
                else
                {
                    if (cardPool.Count == 0) break;
                    var index = rand.Next(cardPool.Count);
                    var name = cardPool[index];
                    cardPool.RemoveAt(index);

                    result.Add(new ShopEntry()
                    {
                        Name = name,
                        IsCard = true,
                        Price = _config.CardPriceTable[name]
                    });
                }
            }

            return result;
        }
    }
}