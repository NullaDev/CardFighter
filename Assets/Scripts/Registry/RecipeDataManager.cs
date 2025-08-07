using System;
using System.Collections.Generic;
using System.Linq;
using Card.Engine;
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

        public const string RecipeFilePath = "Recipes/recipe_list";

        public void LoadFromFile()
        {
            if (_hasLoaded) return;
            _hasLoaded = true;

            TextAsset jsonFile = Resources.Load<TextAsset>(RecipeFilePath);
            if (jsonFile == null)
            {
                Debug.LogError("Recipe JSON file not found at path: " + RecipeFilePath);
                return;
            }

            _recipes.AddRange(JsonConvert.DeserializeObject<List<RecipeTemplate>>(jsonFile.text));
            Debug.Log("Loaded " + _recipes.Count + " recipe templates.");
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
                return new RecipeMatchResult(fusionCard, true, true);
            }

            return null;
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