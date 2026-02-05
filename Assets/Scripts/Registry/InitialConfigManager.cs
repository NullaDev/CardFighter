using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Registry
{
    public class PlayerClassConfig
    {
        public int MaxHP = 20;
        public int InitialInGameGold = 0;
        public int InitialInGameCost = 1;
        public int MaxInGameCost = 5;
        public int MaxCarryCost = 10;
    }
    
    public class DebugConfig
    {
        public string DebugStageType;
        public string DebugStageID;
        public List<string> DebugCards = new List<string>();
        public List<string> DebugItems = new List<string>();
    }
    
    public class InitialConfigManager
    {
        private static readonly string ClassConfigPath = Path.Combine(Application.dataPath, "../GameData/Initial/class.json");
        private static readonly string DebugConfigPath = Path.Combine(Application.dataPath, "../GameData/Initial/debug.json");

        private bool _hasLoaded = false;
        public Dictionary<string, PlayerClassConfig> ClassConfigs = new();
        public DebugConfig DebugConfig = new();
        
        public void DebugLoadedConfig()
        {
            Debug.Log($"[GameConfigManager] Loaded deck configs, total number: {ClassConfigs.Count}");
        }

        public void LoadFromFile()
        {
            if (_hasLoaded) return;
            _hasLoaded = true;

            if (File.Exists(DebugConfigPath))
            {
                try
                {
                    var jsonText = File.ReadAllText(DebugConfigPath);
                    var parsed = JsonConvert.DeserializeObject<DebugConfig>(jsonText);

                    if (parsed == null)
                    {
                        Debug.LogError($"[GameConfigManager] Invalid JSON structure: {DebugConfigPath}");
                    }
                    else
                    {
                        DebugConfig = parsed;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            if (!File.Exists(ClassConfigPath))
            {
                Debug.LogWarning($"[GameConfigManager] Config file not found, creating default: {ClassConfigPath}");
                return;
            }

            try
            {
                var jsonText = File.ReadAllText(ClassConfigPath);
                var parsed = JsonConvert.DeserializeObject<Dictionary<string, PlayerClassConfig>>(jsonText);

                if (parsed == null)
                {
                    Debug.LogError($"[GameConfigManager] Invalid JSON structure: {ClassConfigPath}");
                }
                else
                {
                    ClassConfigs = parsed;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameConfigManager] Failed to read {ClassConfigPath}\n{e}");
            }
            
            DebugLoadedConfig();
        }

        public PlayerClassConfig GetConfigFor(PlayerClass pClass)
        {
            if (ClassConfigs.TryGetValue(pClass.ToString(), out var cfg))
                return cfg;

            Debug.LogWarning($"[GameConfigManager] Missing config for {pClass}, using defaults.");
            return GetConfigFor(PlayerClass.GENERIC);
        }
    }
}