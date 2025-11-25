using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic.Runtime;
using Item;
using JetBrains.Annotations;
using Registry;
using Registry.Data;
using Render;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic.SceneControl
{
    public class DeckModifyControl: MonoBehaviour
    {
        public GameObject render;
        
        private const int CardPerPage = 8;
        private int _currentPageIndex = 0;
        
        private readonly Dictionary<CardPrototype, int> _unusedCards = new();
        [CanBeNull] public CardPrototype RecipeSlot1 = null;
        [CanBeNull] public CardPrototype RecipeSlot2 = null;
        [CanBeNull] public CardPrototype ChosenCard = null;

        private void Awake()
        {
            var playerData = PlayerData.Instance;
            var playerDeck = playerData.CardOperations.GetAllCards();
            
            _unusedCards.Add(CommonCards.Move1, -1);
            _unusedCards.Add(CommonCards.TurnBack, -1);
            
            foreach (var cardPrototype in playerData.HeldCards.Keys)
            {
                var countInDeck = playerDeck.Count(card => card == cardPrototype);
                var remainingCount = playerData.HeldCards[cardPrototype] - countInDeck;

                if (remainingCount > 0)
                {
                    _unusedCards.Add(cardPrototype, remainingCount);
                }
            }

            this.render = GameObject.Find("Render");
            Rerender();
        }

        public void ShrinkCard(CardPrototype card)
        {
            if (card.IsBuiltinCard)
                return;
            
            if (_unusedCards.TryGetValue(card, out var count))
            {
                if (count <= 1)
                {
                    _unusedCards.Remove(card);
                }
                else
                {
                    _unusedCards[card] = count - 1;
                }
            }
        }

        public void AddCard(CardPrototype card)
        {
            if (card.IsBuiltinCard)
                return;
            
            if (!_unusedCards.TryAdd(card, 1))
            {
                _unusedCards[card]++;
            }
        }

        public void Rerender()
        {
            var cardRender = render.GetComponent<DeckModifyRender>();
            var backPackCards = GetCurrentPageCards();
            if (backPackCards.Count == 0 && _currentPageIndex > 0)
            {
                _currentPageIndex--;
                backPackCards = GetCurrentPageCards();
            }
            cardRender.RenderBackpackCards(backPackCards);
            cardRender.RenderDeckCards();
            cardRender.RenderSlots(RecipeSlot1, RecipeSlot2, 
                StaticDataManager.RecipeDataManager.TryGetFusionResult(this.RecipeSlot1, this.RecipeSlot2)?.ResultCard);
        }
        
        private List<(CardPrototype cardPrototype, int cardCount)> GetCurrentPageCards()
        {
            return _unusedCards
                .Skip(_currentPageIndex * CardPerPage)
                .Take(CardPerPage)
                .Select(kv => (kv.Key, kv.Value))
                .ToList();
        }
        
        private int GetTotalPage()
        {
            return (int)Math.Ceiling((double)this._unusedCards.Count / CardPerPage);
        }
        
        public void NextPage()
        {
            this._currentPageIndex = Math.Min(this._currentPageIndex + 1, GetTotalPage() - 1);
            Rerender();
        }
        
        public void PreviousPage()
        {
            this._currentPageIndex = Math.Max(this._currentPageIndex - 1, 0);
            Rerender();
        }

        public void ClickSlot1()
        {
            if (RecipeSlot1 == null)
            {
                if (ChosenCard != null)
                {
                    ShrinkCard(ChosenCard);
                    RecipeSlot1 = ChosenCard;
                    ChosenCard = null;
                }
            }
            else
            {
                if (ChosenCard == null)
                {
                    AddCard(RecipeSlot1);
                    RecipeSlot1 = null;
                }
                else
                {
                    ShrinkCard(ChosenCard);
                    AddCard(RecipeSlot1);
                    RecipeSlot1 = ChosenCard;
                    ChosenCard = null;
                }
            }
            Rerender();
        }
        
        public void ClickSlot2()
        {
            if (RecipeSlot2 == null)
            {
                if (ChosenCard != null)
                {
                    ShrinkCard(ChosenCard);
                    RecipeSlot2 = ChosenCard;
                    ChosenCard = null;
                }
            }
            else
            {
                if (ChosenCard == null)
                {
                    AddCard(RecipeSlot2);
                    RecipeSlot2 = null;
                }
                else
                {
                    ShrinkCard(ChosenCard);
                    AddCard(RecipeSlot2);
                    RecipeSlot2 = ChosenCard;
                    ChosenCard = null;
                }
            }
            Rerender();
        }

        public void ClickSlotResult()
        {
            if (RecipeSlot1 == null || RecipeSlot2 == null)
                return;
            
            var result = StaticDataManager.RecipeDataManager.TryGetFusionResult(RecipeSlot1, RecipeSlot2);
            if (result == null) return;

            var playerData = PlayerData.Instance;
            if (result.ConsumeSlot1 && !IsCardFree(RecipeSlot1.ID) && !RecipeSlot1.IsBuiltinCard)
            {
                if (playerData.HeldCards.ContainsKey(RecipeSlot1))
                {
                    playerData.HeldCards[RecipeSlot1]--;
                    if (playerData.HeldCards[RecipeSlot1] <= 0)
                    {
                        playerData.HeldCards.Remove(RecipeSlot1);
                        if (playerData.CardOperations.HasCard(RecipeSlot1))
                        {
                            playerData.CardOperations.RemoveCard(RecipeSlot1);
                        }
                    }
                }
            }
            else
            {
                AddCard(RecipeSlot1);
            }
            
            if (result.ConsumeSlot2 && !IsCardFree(RecipeSlot2.ID) && !RecipeSlot2.IsBuiltinCard)
            {
                if (playerData.HeldCards.ContainsKey(RecipeSlot2))
                {
                    playerData.HeldCards[RecipeSlot2]--;
                    if (playerData.HeldCards[RecipeSlot2] <= 0)
                    {
                        playerData.HeldCards.Remove(RecipeSlot2);
                        if (playerData.CardOperations.HasCard(RecipeSlot2))
                        {
                            playerData.CardOperations.RemoveCard(RecipeSlot2);
                        }
                    }
                }
            }
            else
            {
                AddCard(RecipeSlot2);
            }
            
            RecipeSlot1 = null;
            RecipeSlot2 = null;
            if (!playerData.HeldCards.TryAdd(result.ResultCard, 1))
            {
                playerData.HeldCards[result.ResultCard]++;
            }
            AddCard(result.ResultCard);

            Rerender();
        }
        
        private bool IsCardFree(string cardId)
        {
            return PlayerData.Instance.HeldItems
                .SelectMany(item => item.Effects)
                .OfType<RecipeFreeCardEffect>()
                .Any(effect => effect.CardIDs.Contains(cardId));
        }

        public void ClickReturn()
        {
            SceneManager.LoadScene("RogueMap");
        }
        
    }
}