using System.Collections.Generic;
using System.Linq;
using GameLogic.Item;
using GameLogic.Runtime;
using Registry;
using Registry.Data;
using Render;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneControl
{
    public class ShopItem
    {
        public bool IsCard;
        public HeldItem HeldItem;
        public CardPrototype Card;
        public int Price;
        public bool IsSold;

        public static ShopItem FromEntry(ShopEntry shopEntry)
        {
            if (shopEntry.IsCard)
            {
                return new ShopItem()
                {
                    IsCard = true,
                    HeldItem = null,
                    Card = StaticDataManager.CardDataManager.Find(shopEntry.Name),
                    Price = shopEntry.Price,
                    IsSold = false,
                };
            }
            else
            {
                return new ShopItem()
                {
                    IsCard = false,
                    HeldItem = StaticDataManager.HeldItemDataManager.Find(shopEntry.Name),
                    Card = null,
                    Price = shopEntry.Price,
                    IsSold = false,
                };
            }
        }
    }
    
    public class ShopControl : MonoBehaviour
    {
        public GameObject render;
        public List<ShopItem> shopList;

        public static int GetBonusLevel()
        {
            var layer = MapData.Instance.CurrentLayer;
            var config = MapData.Instance.CurrentMap.Config;
            return (int)(config.BonusLevel.ShopStart + config.BonusLevel.ShopRamp * layer);
        }

        private void Awake()
        {
            this.shopList = StaticDataManager.ShopManager.GetShopEntries(GetBonusLevel(), MiscData.Instance.GlobalRandom)
                .Select(ShopItem.FromEntry)
                .ToList();
            Rerender();
        }

        public void Rerender()
        {
            render.GetComponent<ShopRender>().RenderItems(shopList);
            render.GetComponent<ShopRender>().RenderGold();
        }

        public void End()
        {
            MapData.Instance.CommitCurrentPosition();
            SceneManager.LoadScene("RogueMap");
        }
    }
}