using GameLogic.Runtime;
using SceneControl;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Render.Interact
{
    public class ShopItemInteract: MonoBehaviour, IPointerClickHandler
    {
        public ShopItem ShopItem;
        public int indexAtShop = -1;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (this.ShopItem.IsSold) return;
            
            var playerData = PlayerData.Instance;
            if (playerData.InGameGold < this.ShopItem.Price) return;

            if (this.ShopItem.IsCard)
            {
                if (!playerData.HeldCards.TryAdd(this.ShopItem.Card, 1))
                {
                    playerData.HeldCards[this.ShopItem.Card]++;
                }
            }
            else
            {
                if (playerData.HeldItems.Contains(this.ShopItem.HeldItem)) return;
                this.ShopItem.HeldItem.PlayerTryObtain(playerData);
            }
            playerData.InGameGold -= this.ShopItem.Price;
            this.ShopItem.IsSold = true;
            
            var shopControl = GameObject.Find("ShopControl").GetComponent<ShopControl>();
            shopControl.shopList[indexAtShop].IsSold = true;
            shopControl.Rerender();
        }
    }
}