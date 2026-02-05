using GameLogic.Card;
using GameLogic.Runtime;
using Render.Interact;
using UnityEngine;
using UnityEngine.UI;

namespace Render.Component
{
    public class ShopItemRender : MonoBehaviour
    {
        private Transform _cover;
        private Transform _card;
        private Transform _item;
        
        private CardRender _cardRender;
        private HeldItemRender _itemRender;
        private Text _price;
        
        void Awake()
        {
            this._cover = transform.Find("Cover");
            this._card = transform.Find("CardNoInteract");
            this._item = transform.Find("HeldItem");
            
            this._cardRender = transform.Find("CardNoInteract").GetComponent<CardRender>();
            this._itemRender = transform.Find("HeldItem").GetComponent<HeldItemRender>();
            _price = transform.Find("Price").GetComponent<Text>();
        }

        public void Render()
        {
            var interact = transform.GetComponent<ShopItemInteract>();
            
            _cover.gameObject.SetActive(interact.ShopItem.IsSold);

            if (interact.ShopItem.IsCard)
            {
                _card.gameObject.SetActive(true);
                _item.gameObject.SetActive(false);
                _cardRender.RenderCard(new CardInstance(interact.ShopItem.Card));
                
            }
            else
            {
                _card.gameObject.SetActive(false);
                _item.gameObject.SetActive(true);
                _itemRender.RenderItem(interact.ShopItem.HeldItem);
            }

            var price = interact.ShopItem.Price;
            _price.text = price.ToString();
            _price.color = interact.ShopItem.IsSold ? Color.black : price <= PlayerData.Instance.InGameGold? Color.green : Color.red;
        }
    }
}