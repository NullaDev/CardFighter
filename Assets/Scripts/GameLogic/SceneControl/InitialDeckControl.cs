using System;
using System.Collections.Generic;
using System.Linq;
using Registry;
using Render;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic.SceneControl
{
    public class InitialDeckControl: MonoBehaviour
    {
        public GameObject render;
        private const int CardPerPage = 12;
        private int _currentPageIndex = 0;

        private int _maxCost;
        private readonly Dictionary<string, int> _allCards = new();
        private readonly Dictionary<string, int> _carryCards = new();
        
        private void Awake()
        {
            var playerData = PlayerData.Instance;
            _maxCost = PlayerData.Instance.MaxCarryCost;
            foreach (var kv in StaticDataManager.InitialDeckManager.GetDeckFor(PlayerClass.GENERIC))
            {
                _allCards[kv.Key] = kv.Value;
            }

            if (playerData.PlayerClass != PlayerClass.GENERIC)
            {
                foreach (var kv in StaticDataManager.InitialDeckManager.GetDeckFor(playerData.PlayerClass))
                {
                    _allCards[kv.Key] = kv.Value;
                }
            }
            
            _carryCards.Add(CommonCards.Move1.ID, 1);
            _carryCards.Add(CommonCards.TurnBack.ID, 1);
            
            Debug.Log($"{_allCards.Count} cards loaded form class generic and {playerData.PlayerClass}");
            Debug.Log($"{GetTotalPage()} pages in total");
            Rerender();
        }

        void Rerender()
        {
            var cardRender = render.GetComponent<DeckInitialzeRender>();
            cardRender.RenderCards(GetCurrentPageCards());
            var uiRender = render.GetComponent<DeckInitializeUIRender>();
            uiRender.RenderCost(_maxCost-CalcTotalCost(), _maxCost);
            uiRender.RenderCount(_carryCards.Count);
        }
        
        private List<(string CardName, int Cost, int CarryCount)> GetCurrentPageCards()
        {
            var startIndex = _currentPageIndex * CardPerPage;

            var pageCards = _allCards.Keys
                .Skip(startIndex)
                .Take(CardPerPage);

            var result = pageCards
                .Select(cardName => (
                    CardName: cardName,
                    Cost: _allCards[cardName],
                    CarryCount: _carryCards.GetValueOrDefault(cardName, 0)
                ))
                .ToList();

            return result;
        }

        private int CalcTotalCost()
        {
            return _carryCards.Sum(card => _allCards[card.Key] * card.Value);
        }
        
        public void TryAddCard(string cardID)
        {
            if (cardID == CommonCards.Move1.ID || cardID == CommonCards.TurnBack.ID)
                return;
            if (_maxCost < CalcTotalCost() + _allCards[cardID])
                return;
            _carryCards[cardID] = _carryCards.GetValueOrDefault(cardID) + 1;
            Rerender();
        }

        public void TryMinusCard(string cardID)
        {
            if (cardID == CommonCards.Move1.ID || cardID == CommonCards.TurnBack.ID)
                return;
            if (_carryCards.ContainsKey(cardID))
            {
                var count = _carryCards[cardID];
                if (count > 1)
                    _carryCards[cardID] -= 1;
                else
                    _carryCards.Remove(cardID);
            }
            Rerender();
        }

        private int GetTotalPage()
        {
            return (int)Math.Ceiling((double)this._allCards.Count / CardPerPage);
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

        public void EnterGame()
        {
            var playerData = PlayerData.Instance;
            playerData.HeldCards.Clear();
            foreach (var (cardID, count) in _carryCards)
            {
                var cardPrototype = StaticDataManager.CardDataManager.Find(cardID);
                if (cardPrototype != null)
                {
                    playerData.HeldCards[cardPrototype] = count;
                }
                else
                {
                    Debug.LogWarning($"Card ID {cardID} not found in CardManager.");
                }
            }
            playerData.InitCardOperationsFromHeld();
            SceneManager.LoadScene("RogueMap");
        }

        public void Return()
        {
            SceneManager.LoadScene("ClassChoose");
        }
    }
}