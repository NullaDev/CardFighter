using Card;
using UnityEngine;
using UnityEngine.UI;

namespace Render
{
    public class CardRender: MonoBehaviour
    {
        private Image _cardImage;
        private Text _cardCost;
        private Text _cardTitle;
        private Text _cardDesc;
        
        void Awake()
        {
            _cardImage = transform.Find("CardImage").GetComponent<Image>();
            _cardCost = transform.Find("CardCostText").GetComponent<Text>();
            _cardTitle = transform.Find("CardTitle").GetComponent<Text>();
            _cardDesc = transform.Find("CardDesc").GetComponent<Text>();
        }
        
        public void RenderCard(CardInstance card, int? cost)
        {
            if (card.Prototype.TextureName != "")
            {
                var sprite = Resources.Load<Sprite>(card.Prototype.TextureName);
                _cardImage.sprite = sprite;
            }
            else
            {
                _cardImage.sprite = null;
            }

            var actualCost = cost.GetValueOrDefault(card.Prototype.Cost);
            if (actualCost > card.Prototype.Cost)
                _cardCost.color = Color.red;
            else if (actualCost < card.Prototype.Cost)
                _cardCost.color = Color.green;
            _cardCost.text = actualCost.ToString();
            _cardTitle.text = card.Prototype.Name;
            _cardDesc.text = card.Prototype.Desc;
        }
    }
}