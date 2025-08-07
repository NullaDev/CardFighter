using System.Collections.Generic;
using System.Linq;
using GameLogic.Option;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace Registry
{
    public class OptionDataManager
    {
        private bool _hasLoaded = false;
        public readonly Dictionary<string, OptionBundle> OptionMap = new();
        
        private const string OptionFolderRoot = "Options";
        
        public void DebugLoadedOptionInfo()
        {
            Debug.Log("Loading options...");
            
            var keyCount = OptionMap.Count;
            var optionCount = OptionMap.Sum(kv => kv.Value.GuaranteedOptions.Count + kv.Value.OptionalOptions.Count);
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
                var dict = JsonConvert.DeserializeObject<Dictionary<string, OptionBundle>>(file.text, settings);
                foreach (var kv in dict)
                {
                    OptionMap[kv.Key] = kv.Value;
                }
            }

            DebugLoadedOptionInfo();
        }

        [CanBeNull]
        public OptionBundle GetBundle(string key)
        {
            return OptionMap.GetValueOrDefault(key);
        }
    }
}