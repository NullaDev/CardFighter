using Card;
using UnityEngine;
using UnityEngine.UI;

namespace Render
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
            this._count.text = $"×{count}";
            this._frame.enabled = shouldRenderFrame;
        }
    }
}