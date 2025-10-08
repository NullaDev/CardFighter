using System;
using System.Collections.Generic;
using System.IO;
using Registry.Data;
using UnityEngine;

namespace Registry
{
    public class CardDataManager
    {
        private bool _hasLoaded = false;

        public static readonly string CardFolderRoot = Path.Combine(Application.dataPath, "../GameData/Cards");
        private readonly List<CardPrototype> _listCards = new();

        public void DebugLoadedCardInfo()
        {
            Debug.Log($"[CardDataManager] Loaded {this._listCards.Count} cards.");
            // foreach (var card in this._listCards)
            // {
            //     Debug.Log($"[CardDataManager] name: {card.Name}");
            // }
        }

        public void LoadFromFile()
        {
            if (_hasLoaded) return;
            this._hasLoaded = true;
            
            if (!Directory.Exists(CardFolderRoot))
            {
                Debug.LogError($"[CardDataManager] Card folder not found in path: {CardFolderRoot}");
                return;
            }

            var jsonFiles = Directory.GetFiles(CardFolderRoot, "*.json", SearchOption.AllDirectories);
            if (jsonFiles.Length == 0)
            {
                Debug.LogWarning($"[CardDataManager] No card json files found in {CardFolderRoot}");
                return;
            }
            
            foreach (var file in jsonFiles)
            {
                try
                {
                    var jsonText = File.ReadAllText(file);
                    var card = CardPrototype.CreateFromJson(jsonText);
                    if (card != null)
                        _listCards.Add(card);
                    else
                        Debug.LogWarning($"[CardDataManager] Failed to parse card file: {file}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[CardDataManager] Error loading {file}: {e.Message}");
                }
            }

            DebugLoadedCardInfo();
        }

        public CardPrototype Find(string cardName)
        {
            return this._listCards.Find(c => c.ID.Equals(cardName));
        }
    }
}