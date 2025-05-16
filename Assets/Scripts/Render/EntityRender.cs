using System;
using Card;
using Entity;
using Fighting;
using GameLogic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Render
{
    public class EntityRender : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Image _entityImage;
        private Image _hpBack;
        private Image _hpFront;
        private Text _hpText;
        private GameObject _thinking;

        private CardInstance _cardToUse = null;
        
        void Awake()
        {
            _entityImage = transform.Find("EntityImage").GetComponent<Image>();

            var hpBar = transform.Find("HPBar").gameObject;
            if (hpBar != null)
            {
                _hpBack = hpBar.transform.Find("HPBack").GetComponent<Image>();
                _hpFront = hpBar.transform.Find("HPFront").GetComponent<Image>();
                _hpText = hpBar.transform.Find("HPText").GetComponent<Text>();
            }

            _thinking = transform.Find("Thinking").gameObject;
            _thinking.SetActive(false);
        }

        public void RenderEmpty()
        {
            _entityImage.enabled = false;
            _hpBack.enabled = false;
            _hpFront.enabled = false;
            _hpText.enabled = false;
        }

        public void RenderEntity(EntityBase entity)
        {
            _entityImage.enabled = true;

            if (entity.TextureName != "")
            {
                var sprite = Resources.Load<Sprite>(entity.TextureName);
                _entityImage.sprite = sprite;
                if (entity.Facing == EntityFacing.LEFT)
                {
                    _entityImage.transform.localScale = new Vector3(-1f, 1f, 1f);;
                }
                else
                {
                    _entityImage.transform.localScale = new Vector3(1f, 1f, 1f);;
                }
            }

            RenderHpBar(entity);

            if (entity is Enemy enemy)
            {
                if (enemy.NextTurnCard != null)
                {
                    Console.WriteLine(enemy.NextTurnCard.Prototype.Name);
                    this._cardToUse = enemy.NextTurnCard;
                }
            }
        }
        
        private void RenderHpBar(EntityBase entity)
        {
            _hpBack.enabled = true;
            _hpFront.enabled = true;
            _hpText.enabled = true;
            
            _hpText.text = entity.HP + "/" + entity.MaxHP;
            var newWidth = _hpBack.rectTransform.rect.width * entity.HP / entity.MaxHP;
            _hpFront.rectTransform.sizeDelta = new Vector2(newWidth, _hpFront.rectTransform.sizeDelta.y);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (this._cardToUse == null) return;
            
            var map = GameObject.Find("FightingControl").GetComponent<FightingControl>().BattleField;
            var player = map.GetPlayerFromMap();
            if (!player.HasBuff(EntityBuffManager.Insight)) return;
            
            _thinking.SetActive(true);
            
            var render = _thinking.transform.Find("CardNoInteract").GetComponent<CardRender>();
            render.RenderCard(this._cardToUse);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _thinking.SetActive(false);
        }
    }
}