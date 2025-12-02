using Render.Component;
using SceneControl;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Render.Interact
{
    public class InitialDeckInteractAdd: MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            var cardID = this.transform.parent.transform.GetComponent<DeckInitializeCardRender>().CardID;
            var initialDeckControl = GameObject.Find("InitialDeckControl").GetComponent<InitialDeckControl>();
            initialDeckControl.TryAddCard(cardID);
        }
    }
}