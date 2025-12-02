using Card;
using SceneControl;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Render.Interact
{
    public class CardInteract: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public CardInstance CardInstance;
        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.localScale = new Vector3(1.1F, 1.1F, 1);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.localScale = Vector3.one;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var fightingControl = GameObject.Find("FightingControl").GetComponent<FightingControl>();
            fightingControl.PlayerUseCard(CardInstance);
        }
    }
}