using Card;
using Registry;
using UnityEngine;
using UnityEngine.UI;

namespace Render
{
    public class DeckModifyDeckCardRender : MonoBehaviour
    {
        private CardRender _cardRender;
        private Image _cover;
        private Text _coverText;
        
        void Awake()
        {
            this._cardRender = transform.Find("CardNoInteract").GetComponent<CardRender>();
            this._cover = transform.Find("EmptySlotCover").GetComponent<Image>();
            this._coverText = transform.Find("EmptySlotText").GetComponent<Text>();
            
            this._cover.enabled = false;
            this._coverText.enabled = false;
        }

        public void RenderCard(CardInstance card)
        {
            this._cardRender.RenderCard(card, null);
            
            this._cover.enabled = false;
            this._coverText.enabled = false;
        }

        public void RenderEmpty()
        {
            this._cardRender.RenderCard(new CardInstance(CommonCards.DoNothing), null);
            
            this._cover.enabled = true;
            this._coverText.enabled = true;
        }
    }
}