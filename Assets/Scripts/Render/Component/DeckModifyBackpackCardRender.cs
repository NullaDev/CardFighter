using GameLogic.Card;
using UnityEngine;
using UnityEngine.UI;

namespace Render.Component
{
    public class DeckModifyBackpackCardRender : MonoBehaviour
    {
        private CardRender _cardRender;
        private Image _frame;
        private Text _count;
        
        void Awake()
        {
            this._cardRender = transform.Find("CardNoInteract").GetComponent<CardRender>();
            this._frame = this.transform.Find("Frame").GetComponent<Image>();
            this._count= this.transform.Find("Count").GetComponent<Text>();
            
            this._frame.enabled = false;
        }

        public void Render(CardInstance card, int count, bool shouldRenderFrame)
        {
            this._cardRender.RenderCard(card);
            this._count.text = count>=0? $"×{count}" : "×inf";
            this._frame.enabled = shouldRenderFrame;
        }
    }
}