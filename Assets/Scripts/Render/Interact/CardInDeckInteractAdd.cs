using GameLogic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Render.Interact
{
    public class CardInDeckInteractAdd: MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            var cardID = this.transform.parent.transform.GetComponent<CardInDeckRender>().CardID;
            var initialDeckControl = GameObject.Find("InitialDeckControl").GetComponent<InitialDeckControl>();
            initialDeckControl.TryAddCard(cardID);
        }
    }
}