using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Registry.Data;
using UnityEngine;

namespace Registry
{
    public class RecipeDataManager
    {
        private bool _hasLoaded = false;
        private readonly List<RecipeTemplate> _recipes = new();

        public static readonly string RecipeFolderRoot = Path.Combine(Application.dataPath, "../GameData/Recipes");
        
        public void DebugLoadedItemInfo()
        {
            Debug.Log($"[RecipeDataManager] Loaded recipes: {_recipes.Count}");
        }

        public void LoadFromFile()
        {
            if (_hasLoaded) return;
            _hasLoaded = true;

            if (!Directory.Exists(RecipeFolderRoot))
            {
                Debug.LogError($"[RecipeDataManager] recipe folder not found in path: {RecipeFolderRoot}");
                return;
            }

            var jsonFiles = Directory.GetFiles(RecipeFolderRoot, "*.json", SearchOption.AllDirectories);
            if (jsonFiles.Length == 0)
            {
                Debug.LogWarning($"[RecipeDataManager] No item json files found in {RecipeFolderRoot}");
                return;
            }

            foreach (var file in jsonFiles)
            {
                try
                {
                    var jsonText = File.ReadAllText(file);
                    var list = JsonConvert.DeserializeObject<List<RecipeTemplate>>(jsonText);
                    if (list?.Count > 0)
                    {
                        _recipes.AddRange(list);
                    }
                    else
                    {
                        Debug.LogWarning($"[RecipeDataManager] Recipe file empty or invalid: {file}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[RecipeDataManager] Error loading recipe file: {e.Message}");
                }
            }

            DebugLoadedItemInfo();
        }

        [CanBeNull]
        public RecipeTemplate FindMatch(string slot1Id, string slot2Id)
        {
            return _recipes
                .Where(r => r.Slot1.Contains(slot1Id) && r.Slot2.Contains(slot2Id))
                .OrderByDescending(r => r.Priority)
                .FirstOrDefault();
        }

        [CanBeNull]
        public RecipeMatchResult TryGetFusionResult([CanBeNull] CardPrototype card1, [CanBeNull] CardPrototype card2)
        {
            if (card1 == null || card2 == null) return null;
            
            var matched = FindMatch(card1.ID, card2.ID);
            if (matched != null)
            {
                var resultCard = StaticDataManager.CardDataManager.Find(matched.Result);
                if (resultCard != null)
                {
                    return new RecipeMatchResult(resultCard, matched.ConsumeSlot1, matched.ConsumeSlot2);
                }
                Debug.LogWarning($"Recipe matched but result card not found: {matched.Result}");
                return null;
            }

            if (!card1.IsFusionCard && !card2.IsFusionCard)
            {
                return DefaultMergeTwoCards(card1, card2);
            }

            return null;
        }
        
        public static RecipeMatchResult DefaultMergeTwoCards(CardPrototype card1, CardPrototype card2)
        {
            var name1 = card1.Name;
            var name2 = card2.Name;
            var half1Len = Math.Max(2, Math.Min(3, (int)Math.Ceiling(name1.Length / 2.0)));
            var half2Len = Math.Max(2, Math.Min(3, (int)Math.Ceiling(name2.Length / 2.0)));
            var fusedName = name1[..Math.Min(half1Len, name1.Length)] + name2[Math.Max(0, name2.Length - half2Len)..];
            var fusionCard = new CardPrototype
            {
                ID = $"fusion_{card1.ID}+{card2.ID}",
                Name = fusedName,
                TextureName = "Arts/Cards/Misc/fusion",
                Desc = $"施放{card1.Name}和{card2.Name}",
                Cost = card1.Cost + card2.Cost + 1,
                IsFusionCard = true,
                Actions = card1.Actions.Concat(card2.Actions).ToList()
            };
            return new RecipeMatchResult(fusionCard, !card1.IsBuiltinCard, !card2.IsBuiltinCard);
        }
        
        public List<RecipeTemplate> All() => _recipes;
    }
    
    public class RecipeMatchResult
    {
        public CardPrototype ResultCard { get; set; }
        public bool ConsumeSlot1 { get; set; }
        public bool ConsumeSlot2 { get; set; }

        public RecipeMatchResult(CardPrototype card, bool consume1, bool consume2)
        {
            ResultCard = card;
            ConsumeSlot1 = consume1;
            ConsumeSlot2 = consume2;
        }
    }
}