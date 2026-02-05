using GameLogic.Item;
using UnityEngine;
using UnityEngine.UI;

namespace Render.Component
{
    public class HeldItemRender: MonoBehaviour
    {
        private Image _itemImage;
        private Text _itemTitle;
        private Text _itemDesc;
        
        void Awake()
        {
            _itemImage = transform.Find("ItemImage").GetComponent<Image>();
            _itemTitle = transform.Find("ItemTitle").GetComponent<Text>();
            _itemDesc = transform.Find("ItemDesc").GetComponent<Text>();
        }
        
        public void RenderItem(HeldItem item)
        {
            if (!string.IsNullOrEmpty(item.TextureName))
            {
                var sprite = TextureCache.GetSprite(item.TextureName);
                _itemImage.sprite = sprite;
            }
            else
            {
                _itemImage.sprite = null;
            }
            
            _itemTitle.text = item.Name;
            _itemDesc.text = item.EffectText;
        }
    }
}