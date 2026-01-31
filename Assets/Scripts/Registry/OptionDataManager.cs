using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameLogic.Option;
using GameLogic.Runtime;
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
            var layer = MapData.Instance.CurrentLayer;
            var config = MapData.Instance.CurrentMap.Config;
            var eventMaxLevel = (int)(config.BonusLevel.EventStart + config.BonusLevel.EventRamp * layer);

            var validEventKeys = new List<string>();
            foreach (var key in OptionMap.Keys)
            {
                if (!key.StartsWith("event_level_"))
                    continue;

                var parts = key.Split('_');
                if (parts.Length < 3)
                    continue;

                if (int.TryParse(parts[2], out var level))
                {
                    if (level <= eventMaxLevel)
                    {
                        validEventKeys.Add(key);
                    }
                }
            }

            if (validEventKeys.Count == 0)
                throw new Exception($"no event loaded for max level {eventMaxLevel}");

            var randomKey = validEventKeys[Random.Range(0, validEventKeys.Count)];
            return OptionMap[randomKey];
        }
    }
}