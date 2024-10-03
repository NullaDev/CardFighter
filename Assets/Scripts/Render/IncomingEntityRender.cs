using Entity;
using GameLogic;
using UnityEngine;
using UnityEngine.UI;

namespace Render
{
    public class IncomingEntityRender: MonoBehaviour
    {
        private Image _entityImage;
        private Image _downArrow;

        void Awake()
        {
            _entityImage = transform.Find("EntityImage").GetComponent<Image>();
            _downArrow = transform.Find("DownArrow").GetComponent<Image>();
        }

        public void RenderEmpty()
        {
            _entityImage.enabled = false;
            _downArrow.enabled = false;
        }

        public void RenderEntity(EntityConfig entity)
        {
            _entityImage.enabled = true;
            _downArrow.enabled = true;

            if (entity.TextureName != "")
            {
                var sprite = Resources.Load<Sprite>("Arts/Entities/" + entity.TextureName);
                _entityImage.sprite = sprite;
            }
        }

    }
}