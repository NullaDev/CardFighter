using System;
using System.Collections.Generic;
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
        private const string DeckJsonPath = "InitialDeck/data";
        private readonly Dictionary<PlayerClass, Dictionary<string, int>> _deckDict = new();
        
        public void DebugLoadedDeckInfo()
        {
            Debug.Log("Loading cards, total number:" + this._deckDict.Count);
            foreach (var kv in this._deckDict)
            {
                Debug.Log("name: " + kv.Key.ToString() + ", cards: " + kv.Value.Count);
            }
        }

        public void LoadFromFile()
        {
            if (_hasLoaded) return;
            _hasLoaded = true;

            var deckAsset = Resources.Load<TextAsset>(DeckJsonPath);
            if (deckAsset == null)
            {
                Debug.LogError("Failed to load InitialDeck.json from Resources.");
                return;
            }

            var rawDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(deckAsset.text);
            foreach (var kvp in rawDict)
            {
                if (TryParsePlayerClass(kvp.Key, out var pClass))
                {
                    _deckDict[pClass] = kvp.Value;
                }
                else
                {
                    Debug.LogWarning($"Unknown PlayerClass: {kvp.Key}");
                }
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