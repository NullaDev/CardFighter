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
        
        public void RenderCard(CardInstance card)
        {
            if (card.Prototype.TextureName != "")
            {
                var sprite = Resources.Load<Sprite>(card.Prototype.TextureName);
                _cardImage.sprite = sprite;
            }
            _cardCost.text = card.CurrentCost.ToString();
            _cardTitle.text = card.Prototype.Name;
            _cardDesc.text = card.Prototype.Desc;
        }
    }
}