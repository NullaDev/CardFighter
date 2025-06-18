using System.Collections.Generic;
using Card;
using Registry;
using UnityEngine;

namespace Render
{
    public class DeckInitialzeRender: MonoBehaviour
    {
        public GameObject DeckInitializeCardPrefab;
        public GameObject CardGrid;
        
        private readonly List<GameObject> _listCards = new();

        public void RenderCards(List<(string, int, int)> cards)
        {
            foreach (var card in _listCards)
            {
                GameObject.Destroy(card);
            }
            _listCards.Clear();
            
            foreach (var (cardID, cost, count) in cards)
            {
                var cardPrototype = StaticDataManager.CardDataManager.Find(cardID);
                if (cardPrototype == null)
                {
                    Debug.Log(cardID);
                }
                var cardObject = GameObject.Instantiate(DeckInitializeCardPrefab, CardGrid.transform);
                var render = cardObject.GetComponent<DeckInitializeCardRender>();
                render.Render(new CardInstance(cardPrototype), cost, count);
                _listCards.Add(cardObject);
            }
        }
    }
}