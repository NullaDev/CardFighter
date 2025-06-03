using System.Collections.Generic;
using HeldItem;
using Newtonsoft.Json;
using UnityEngine;

namespace Registry
{
    public class HeldItemDataManager
    {
        private bool _hasLoaded = false;

        public const string ItemFolderRoot = "HeldItems/";
        public static string[] SubFolders = { "Generic", "RU", "Test" };

        private readonly List<HeldItem.HeldItem> _listItems = new();

        public void DebugLoadedItemInfo()
        {
            Debug.Log("Loading held items, total number: " + this._listItems.Count);
            // foreach (var item in this._listItems)
            // {
            //     Debug.Log("Item name: " + item.Name);
            // }
        }

        public void LoadFromFile()
        {
            if (_hasLoaded) return;
            _hasLoaded = true;

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new HeldItemEffectConverter());

            foreach (var subFolder in SubFolders)
            {
                var fullPath = ItemFolderRoot + subFolder;
                var itemList = Resources.LoadAll<TextAsset>(fullPath);

                foreach (var item in itemList)
                {
                    var parsed = JsonConvert.DeserializeObject<HeldItem.HeldItem>(item.text, settings);
                    if (parsed != null)
                        _listItems.Add(parsed);
                }
            }

            DebugLoadedItemInfo();
        }

        public HeldItem.HeldItem Find(string itemId)
        {
            return _listItems.Find(i => i.ID.Equals(itemId));
        }

        public List<HeldItem.HeldItem> All() => _listItems;
    }
}