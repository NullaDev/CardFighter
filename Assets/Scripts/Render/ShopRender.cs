using System.Collections.Generic;
using GameLogic.Runtime;
using Render.Component;
using Render.Interact;
using SceneControl;
using UnityEngine;
using UnityEngine.UI;

namespace Render
{
    public class ShopRender : MonoBehaviour
    {
        public GameObject ShopItemPrefab;
        public GameObject ShopItemGrid;
        public Text CurrentGoldText;
        
        private readonly List<GameObject> _listShopItems = new();

        public void RenderItems(List<ShopItem> shopList)
        {
            foreach (var item in _listShopItems)
            {
                GameObject.Destroy(item);
            }
            _listShopItems.Clear();
            
            for (var i = 0; i < shopList.Count; i++)
            {
                var itemObj = GameObject.Instantiate(ShopItemPrefab, ShopItemGrid.transform);
                _listShopItems.Add(itemObj);
                
                var interact = itemObj.GetComponent<ShopItemInteract>();
                interact.ShopItem = shopList[i];
                interact.indexAtShop = i;
                
                var render = itemObj.GetComponent<ShopItemRender>();
                render.Render();
            }
        }

        public void RenderGold()
        {
            CurrentGoldText.text = $"当前金币：{PlayerData.Instance.InGameGold}";
        }
    }
}