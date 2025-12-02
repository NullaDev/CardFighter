using GameLogic.Runtime;
using JetBrains.Annotations;
using Registry.Data;
using SceneControl;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Render.Interact
{
    public class DeckModifyDeckInteract: MonoBehaviour, IPointerClickHandler
    {
        [CanBeNull] private CardPrototype _card = null;

        public void SetCard(CardPrototype card)
        {
            this._card = card;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            var playerData = PlayerData.Instance;
            var deckModify = GameObject.Find("DeckModify").GetComponent<DeckModifyControl>();
            if (this._card == null)
            {
                if (deckModify.ChosenCard != null)
                {
                    if (playerData.CardOperations.AddCard(deckModify.ChosenCard))
                    {
                        deckModify.ShrinkCard(deckModify.ChosenCard);
                        deckModify.ChosenCard = null;
                    }
                }
            }
            else
            {
                if (deckModify.ChosenCard == null)
                {
                    if (playerData.CardOperations.RemoveCard(_card))
                    {
                        deckModify.AddCard(_card);
                    }
                }
                else
                {
                    if (playerData.CardOperations.ReplaceCard(_card, deckModify.ChosenCard))
                    {
                        deckModify.ShrinkCard(deckModify.ChosenCard);
                        deckModify.AddCard(_card);
                        deckModify.ChosenCard = null;
                    }
                }
            }
            deckModify.Rerender();
        }
    }
}