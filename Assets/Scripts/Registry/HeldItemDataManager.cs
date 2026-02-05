using System;
using System.Collections.Generic;
using System.IO;
using GameLogic.Item;
using Newtonsoft.Json;
using UnityEngine;

namespace Registry
{
    public class HeldItemDataManager
    {
        private bool _hasLoaded = false;

        public static readonly string ItemFolderRoot = Path.Combine(Application.dataPath, "../GameData/HeldItems");

        private readonly List<HeldItem> _listItems = new();

        public void DebugLoadedItemInfo()
        {
            Debug.Log($"[HeldItemDataManager] Loaded held items: {_listItems.Count}");
            // foreach (var item in this._listItems)
            // {
            //     Debug.Log($"[HeldItemDataManager] Item name: {item.Name}");
            // }
        }

        public void LoadFromFile()
        {
            if (_hasLoaded) return;
            _hasLoaded = true;

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new HeldItemEffectConverter());
            
            if (!Directory.Exists(ItemFolderRoot))
            {
                Debug.LogError($"[HeldItemDataManager] Held item folder not found in path: {ItemFolderRoot}");
                return;
            }
            
            var jsonFiles = Directory.GetFiles(ItemFolderRoot, "*.json", SearchOption.AllDirectories);
            if (jsonFiles.Length == 0)
            {
                Debug.LogWarning($"[CardDataManager] No item json files found in {ItemFolderRoot}");
                return;
            }
            
            foreach (var file in jsonFiles)
            {
                try
                {
                    var jsonText = File.ReadAllText(file);
                    var parsed = JsonConvert.DeserializeObject<HeldItem>(jsonText, settings);
                    if (parsed != null)
                    {
                        _listItems.Add(parsed);
                    }
                    else
                    {
                        Debug.LogWarning($"[HeldItemDataManager] Failed to parse item file: {file}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[HeldItemDataManager] Error loading {file}: {e.Message}");
                }
            }

            DebugLoadedItemInfo();
        }

        public HeldItem Find(string itemId)
        {
            return _listItems.Find(i => i.ID.Equals(itemId));
        }

        public List<HeldItem> All() => _listItems;
    }
}