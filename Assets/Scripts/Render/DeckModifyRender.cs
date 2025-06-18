using System.Collections.Generic;
using Card;
using GameLogic;
using Registry;
using Registry.Data;
using Render.Interact;
using UnityEngine;

namespace Render
{
    public class DeckModifyRender: MonoBehaviour
    {
        public GameObject BackpackCardPrefab;
        public GameObject BackpackCardGrid;
        public GameObject DeckCardPrefab;
        public GameObject DeckCardGrid;
        
        private readonly List<GameObject> _listBackpackCards = new();
        private readonly List<GameObject> _listDeckCards = new();
        
        public void RenderBackpackCards(List<(CardPrototype, int)> cards)
        {
            var deckModify = GameObject.Find("DeckModify").GetComponent<DeckModifyControl>();
            
            foreach (var card in _listBackpackCards)
            {
                GameObject.Destroy(card);
            }
            _listBackpackCards.Clear();
            
            foreach (var (cardPrototype, count) in cards)
            {
                var cardObject = GameObject.Instantiate(BackpackCardPrefab, BackpackCardGrid.transform);
                var interact = cardObject.GetComponent<DeckModifyBackpackInteract>();
                interact.SetCard(cardPrototype);
                var render = cardObject.GetComponent<DeckModifyBackpackCardRender>();
                render.Render(new CardInstance(cardPrototype), count, deckModify.ChosenCard == cardPrototype);
                _listBackpackCards.Add(cardObject);
            }
        }

        public void RenderDeckCards()
        {
            foreach (var card in _listDeckCards)
            {
                GameObject.Destroy(card);
            }
            _listDeckCards.Clear();

            var playerData = PlayerData.Instance;
            var allCards = playerData.CardOperations.GetAllCards();
            while (allCards.Count < 2 + CardOperationsInBattle.MaxCardCount)
            {
                allCards.Add(null);
            }
            
            foreach (var cardPrototype in allCards)
            {
                var card = GameObject.Instantiate(DeckCardPrefab, DeckCardGrid.transform);
                var cardRender = card.GetComponent<DeckModifyDeckCardRender>();
                var interact = card.GetComponent<DeckModifyDeckInteract>();
                if (cardPrototype != null)
                {
                    cardRender.RenderCard(new CardInstance(cardPrototype));
                    interact.SetCard(cardPrototype);
                }
                else
                {
                    cardRender.RenderEmpty();
                }
                _listDeckCards.Add(card);
            }
        }
    }
}