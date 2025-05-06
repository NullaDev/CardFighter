using System.Collections.Generic;
using System.Linq;
using Card;
using GameLogic;
using UnityEngine;

namespace Render
{
    public class DeckRender: MonoBehaviour
    {
        public GameObject CardPrefab;
        public GameObject CardGrid;

        private List<GameObject> _listCards = new();

        public void RenderCards(List<CardInstance> playerCardsList)
        {
            foreach (var card in _listCards)
            {
                GameObject.Destroy(card);
            }
            _listCards.Clear();
            
            foreach (var cardInstance in playerCardsList)
            {
                var card = GameObject.Instantiate(CardPrefab, CardGrid.transform);
                var cardRender = card.GetComponent<CardRender>();
                cardRender.RenderCard(cardInstance);
                var cardInteract = card.GetComponent<CardInteract>();
                cardInteract.CardInstance = cardInstance;
                
                _listCards.Add(card);
            }
        }
    }
}