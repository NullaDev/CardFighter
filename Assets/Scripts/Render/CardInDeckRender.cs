using Card;
using UnityEngine;
using UnityEngine.UI;

namespace Render
{
    public class CardInDeckRender : MonoBehaviour
    {
        public string CardID { get; set; }
        private CardRender _cardRender;
        private Text _cost;
        private Text _carryCount;
        
        void Awake()
        {
            this._cardRender = transform.Find("CardNoInteract").GetComponent<CardRender>();
            _cost = transform.Find("Cost").GetComponent<Text>();;
            _carryCount = transform.Find("CarryCount").GetComponent<Text>();
        }

        public void Render(CardInstance card, int cost, int carryCount)
        {
            this.CardID = card.Prototype.ID;
            this._cardRender.RenderCard(card, null);
            this._cost.text = $"携带费用：{cost}";
            this._carryCount.text = $"×{carryCount}";
        }
    }
}