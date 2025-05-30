using System;
using Entity;
using GameLogic;
using Registry.Data;
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

        public void RenderEntity(EntityConfig entity, BattleField map)
        {
            _entityImage.enabled = true;
            _downArrow.enabled = true;

            if (entity.TextureName != "")
            {
                var sprite = Resources.Load<Sprite>("Arts/Entities/" + entity.TextureName);
                if (entity is IHasFacing facingEntity)
                {
                    var facing = IHasFacing.ParseFacing(facingEntity.AppearFacing);
                    if (facing == EntityFacing.Default)
                    {
                        facing = FacingHelper.GetFacing(map.GetPlayerIndex() - entity.AppearPos);
                    }
                    
                    switch (facing)
                    {
                        case EntityFacing.Left:
                            _entityImage.transform.localScale = new Vector3(-1f, 1f, 1f);;
                            break;
                        case EntityFacing.Right:
                        case EntityFacing.Default:
                            _entityImage.transform.localScale = new Vector3(1f, 1f, 1f);;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                _entityImage.sprite = sprite;
            }
        }

    }
}