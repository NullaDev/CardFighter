using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Registry
{
    public enum PlayerClass
    {
        RU,
        DAO,
        MO,
        FA,
        MING,
        BING,
        NONG,
        YINYANG,
        ZONGHENG,
        GENERIC,
    }

    public class InitialDeckManager
    {
        private bool _hasLoaded = false;
        private static readonly string DeckJsonPath = Path.Combine(Application.dataPath, "../GameData/Initial/deck.json");
        private readonly Dictionary<PlayerClass, Dictionary<string, int>> _deckDict = new();
        
        public void DebugLoadedDeckInfo()
        {
            Debug.Log($"[InitialDeckManager] Loaded deck configs, total number: {_deckDict.Count}");
            // foreach (var kv in _deckDict)
            // {
            //     Debug.Log($"  - Class: {kv.Key}, Cards: {kv.Value.Count}");
            // }
        }

        public void LoadFromFile()
        {
            if (_hasLoaded) return;
            _hasLoaded = true;
            
            if (!File.Exists(DeckJsonPath))
            {
                Debug.LogError($"[InitialDeckManager] Deck file not found in path: {DeckJsonPath}");
                return;
            }

            try
            {
                var jsonText = File.ReadAllText(DeckJsonPath);
                var rawDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(jsonText);

                if (rawDict == null)
                {
                    Debug.LogError($"[InitialDeckManager] Invalid JSON structure in {DeckJsonPath}");
                    return;
                }

                foreach (var kvp in rawDict)
                {
                    if (TryParsePlayerClass(kvp.Key, out var pClass))
                    {
                        _deckDict[pClass] = kvp.Value;
                    }
                    else
                    {
                        Debug.LogWarning($"[InitialDeckManager] Unknown PlayerClass: {kvp.Key}");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[InitialDeckManager] Failed to read deck file: {DeckJsonPath}\n{e}");
            }

            this.DebugLoadedDeckInfo();
        }

        public Dictionary<string, int> GetDeckFor(PlayerClass pClass)
        {
            return _deckDict.TryGetValue(pClass, out var deck) ? deck : new Dictionary<string, int>();
        }

        public static bool TryParsePlayerClass(string str, out PlayerClass result)
        {
            return Enum.TryParse(str, ignoreCase: true, out result);
        }
    }
}