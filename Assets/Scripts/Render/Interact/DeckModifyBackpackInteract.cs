using GameLogic;
using Registry;
using Registry.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Render.Interact
{
    public class DeckModifyBackpackInteract: MonoBehaviour, IPointerClickHandler
    {
        private CardPrototype _card;

        public void SetCard(CardPrototype card)
        {
            this._card = card;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (this._card == null) return;
            var deckModify = GameObject.Find("DeckModify").GetComponent<DeckModifyControl>();
            deckModify.ChosenCard = deckModify.ChosenCard == this._card ? null : this._card;
            deckModify.Rerender();
        }
    }
}