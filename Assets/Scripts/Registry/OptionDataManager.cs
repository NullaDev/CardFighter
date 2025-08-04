using System.Collections.Generic;
using System.Linq;
using GameLogic.Option;
using Newtonsoft.Json;
using UnityEngine;

namespace Registry
{
    public class OptionDataManager
    {
        private bool _hasLoaded = false;
        public readonly Dictionary<string, List<Option>> OptionMap = new();
        
        private const string OptionFolderRoot = "Options";
        
        public void DebugLoadedOptionInfo()
        {
            Debug.Log("Loading options...");
            
            var keyCount = OptionMap.Count;
            var optionCount = OptionMap.Sum(kv => kv.Value.Count);
            Debug.Log($"Total option types: {keyCount}");
            Debug.Log($"Total options: {optionCount}");
        }
        
        public void LoadFromFile()
        {
            if (_hasLoaded) return;
            this._hasLoaded = true;

            var jsonFiles = Resources.LoadAll<TextAsset>(OptionFolderRoot);
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Converters = new List<JsonConverter> { new OptionActionConverter() }
            };

            foreach (var file in jsonFiles)
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, List<Option>>>(file.text, settings);
                foreach (var kv in dict)
                {
                    if (!OptionMap.ContainsKey(kv.Key))
                        OptionMap[kv.Key] = new List<Option>();

                    OptionMap[kv.Key].AddRange(kv.Value);
                }
            }

            DebugLoadedOptionInfo();
        }

        public List<Option> GetOptions(string key, PlayerData playerData, int maxLen=3)
        {
            if (!OptionMap.TryGetValue(key, out var allOptions)) return new List<Option>();
            return allOptions.FindAll(opt => opt.PlayerClass == "generic" || opt.PlayerClass == playerData.PlayerClass.ToString())
                .OrderBy(_ => playerData.Random.Next())
                .Take(maxLen)
                .ToList();
        }
    }
}