using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Registry;
using Registry.Data;
using Render;
using UnityEngine;

namespace GameLogic
{
    public class DeckModifyControl: MonoBehaviour
    {
        public GameObject render;
        private const int CardPerPage = 8;
        private int _currentPageIndex = 0;
        
        private readonly Dictionary<CardPrototype, int> _unusedCards = new();
        [CanBeNull] private CardPrototype _recipeSlot1 = null;
        [CanBeNull] private CardPrototype _recipeSlot2 = null;
        
        [CanBeNull] public CardPrototype ChosenCard = null;

        public void Test()
        {
            StaticDataManager.LoadAll();
            
            var playerData = PlayerData.Instance;
            playerData.HeldCards[CommonCards.Move1] = 1;
            playerData.HeldCards[CommonCards.TurnBack] = 1;
            
            playerData.HeldCards[StaticDataManager.CardDataManager.Find("punch")] = 1;
            playerData.HeldCards[StaticDataManager.CardDataManager.Find("kick")] = 2;
            playerData.HeldCards[StaticDataManager.CardDataManager.Find("sword")] = 3;
            playerData.HeldCards[StaticDataManager.CardDataManager.Find("broadsword")] = 3;
            playerData.HeldCards[StaticDataManager.CardDataManager.Find("spear")] = 3;
            playerData.HeldCards[StaticDataManager.CardDataManager.Find("bow")] = 3;
            playerData.HeldCards[StaticDataManager.CardDataManager.Find("axe")] = 3;
            playerData.HeldCards[StaticDataManager.CardDataManager.Find("hammer")] = 3;
            playerData.HeldCards[StaticDataManager.CardDataManager.Find("rush")] = 3;
        }

        private void Awake()
        {
            Test();
            var playerData = PlayerData.Instance;
            var playerDeck = playerData.CardOperations.GetAllCards();
            
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
    }
}