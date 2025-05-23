using GameLogic;
using Registry;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Render
{
    public class BuffRender: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Image _buffBG;
        private Image _buffTexture;
        private GameObject _info;

        void Awake()
        {
            _buffBG = transform.Find("BuffBG").GetComponent<Image>();
            _buffTexture = transform.Find("Texture").GetComponent<Image>();
            _info = transform.Find("Info").gameObject;
            _info.SetActive(false);
        }

        public void RenderBuff(EntityBuff buff)
        {
            var buffInfo = StaticDataManager.BuffDisplayManager.Find(buff.Name);
            
            _buffBG.color = buffInfo.Positive ? Color.green : Color.red;
            if (buffInfo.TextureName != "")
            {
                var sprite = Resources.Load<Sprite>(buffInfo.TextureName);
                _buffTexture.sprite = sprite;
            }

            var infoText = _info.transform.Find("InfoText").GetComponent<Text>();
            var timeRemain = buff.Duration<0? "永久":$"{buff.Duration}回合";
            var effect = buffInfo.EffectText;
            foreach (var param in buff.Parameters)
            {
                var placeholder = "{" + param.Key + "}";
                if (effect.Contains(placeholder))
                {
                    effect = effect.Replace(placeholder, param.Value?.ToString());
                }
            }
            infoText.text = $"{buffInfo.Name}\n" +
                            $"剩余时间：{timeRemain}\n" +
                            $"{effect}\n" +
                            $"{buffInfo.ExtraText}";
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _info.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _info.SetActive(false);
        }
    }
}