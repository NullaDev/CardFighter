using GameLogic;
using GameLogic.SceneControl;
using Render.Component;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Render.Interact
{
    public class InitialDeckInteractMinus: MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            var cardID = this.transform.parent.transform.GetComponent<DeckInitializeCardRender>().CardID;
            var initialDeckControl = GameObject.Find("InitialDeckControl").GetComponent<InitialDeckControl>();
            initialDeckControl.TryMinusCard(cardID);
        }
    }
}