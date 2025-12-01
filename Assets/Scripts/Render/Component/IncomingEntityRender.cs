using System;
using GameLogic;
using GameLogic.Entity;
using Registry.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Render.Component
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

        public void RenderEntity(EntityConfig entity, BattleField map)
        {
            _entityImage.enabled = true;
            _downArrow.enabled = true;

            var sprite = TextureCache.GetSprite("Arts/Entities/" + entity.TextureName);
            if (sprite != null)
            {
                if (entity is IFacingConfig facingEntity)
                {
                    var facing = IFacingConfig.ParseFacing(facingEntity.AppearFacing);
                    if (facing == EntityFacing.Default)
                    {
                        facing = FacingHelper.GetFacing(map.GetPlayerIndex() - entity.AppearPos);
                    }

                    _entityImage.transform.localScale = facing switch
                    {
                        EntityFacing.Left => new Vector3(-1f, 1f, 1f),
                        EntityFacing.Right or EntityFacing.Default => new Vector3(1f, 1f, 1f),
                        _ => throw new NotImplementedException()
                    };
                }
                _entityImage.sprite = sprite;
            }
        }

    }
}