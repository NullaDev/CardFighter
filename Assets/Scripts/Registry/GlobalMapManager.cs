using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Registry.Data;
using UnityEngine;

namespace Registry
{
    public class GlobalMapDataManager
    {
        private static readonly string ConfigPath = Path.Combine(Application.dataPath, "../GameData/Initial/global_map.json");

        public GlobalMapConfig LoadedConfig { get; private set; }

        private bool _loaded = false;
        
        public void LoadFromFile()
        {
            if (_loaded) return;
            _loaded = true;

            if (!File.Exists(ConfigPath))
            {
                Debug.LogError($"[GlobalMapDataManager] Config not found: {ConfigPath}");
                LoadedConfig = new GlobalMapConfig { Maps = new List<RogueMapConfig>() };
                return;
            }

            try
            {
                var json = File.ReadAllText(ConfigPath);
                LoadedConfig = GlobalMapConfig.CreateFromJson(json);
                if (LoadedConfig == null)
                {
                    Debug.LogError("[GlobalMapDataManager] Invalid JSON structure!");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[GlobalMapDataManager] Failed to load config: {e}");
            }
        }
    }
}