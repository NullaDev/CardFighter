using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameLogic.Option;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Registry
{
    public class OptionDataManager
    {
        private bool _hasLoaded = false;
        public readonly Dictionary<string, OptionBundle> OptionMap = new();
        
        private static readonly string OptionFolderRoot = Path.Combine(Application.dataPath, "../GameData/Options");
        
        public void DebugLoadedOptionInfo()
        {
            var keyCount = OptionMap.Count;
            var optionCount = OptionMap.Sum(kv => kv.Value.GuaranteedOptions.Count + kv.Value.OptionalOptions.Count);
            Debug.Log($"[OptionDataManager] Total option bundles: {keyCount}");
            Debug.Log($"[OptionDataManager] Total individual options: {optionCount}");
        }
        
        public void LoadFromFile()
        {
            if (_hasLoaded) return;
            this._hasLoaded = true;

            if (!Directory.Exists(OptionFolderRoot))
            {
                Debug.LogError($"[OptionDataManager] Folder not found: {OptionFolderRoot}");
                return;
            }

            var jsonFiles = Directory.GetFiles(OptionFolderRoot, "*.json", SearchOption.AllDirectories);
            if (jsonFiles.Length == 0)
            {
                Debug.LogWarning($"[OptionDataManager] No option json files found in {OptionFolderRoot}");
                return;
            }

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Converters = new List<JsonConverter> { new OptionActionConverter() }
            };

            foreach (var file in jsonFiles)
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, OptionBundle>>(json, settings);

                    if (dict == null)
                    {
                        Debug.LogWarning($"[OptionDataManager] Failed to parse {file}");
                        continue;
                    }

                    foreach (var kv in dict)
                    {
                        OptionMap[kv.Key] = kv.Value;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[OptionDataManager] Error loading {file}: {e.Message}");
                }
            }

            DebugLoadedOptionInfo();
        }

        [CanBeNull]
        public OptionBundle GetBundle(string key)
        {
            return OptionMap.GetValueOrDefault(key);
        }
        
        public OptionBundle GetRandomEventBundle()
        {
            var eventKeys = OptionMap.Keys.Where(k => k.StartsWith("event_")).ToList();
            if (eventKeys.Count == 0)
                throw new Exception("no event loaded");
            var randomKey = eventKeys[Random.Range(0, eventKeys.Count)];
            return OptionMap[randomKey];
        }
    }
}