using System;
using System.Collections.Generic;
using GameLogic.Buff;
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
            _buffBG.color = EntityBuffManager.GetBuffType(buff.Name) switch
            {
                EntityBuffManager.BuffType.Positive => Color.green,
                EntityBuffManager.BuffType.Neutral  => Color.yellow,
                EntityBuffManager.BuffType.Negative => Color.red,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            var buffInfo = StaticDataManager.BuffDisplayManager.Find(buff.Name);
            if (buffInfo.TextureName != "")
            {
                var sprite = Resources.Load<Sprite>(buffInfo.TextureName);
                _buffTexture.sprite = sprite;
            }

            var infoText = _info.transform.Find("InfoText").GetComponent<Text>();
            var timeRemain = buff.Duration<0? "永久":$"{buff.Duration}回合";
            var effect = buffInfo.EffectText;
            
            var displayParams = new Dictionary<string, object>();
            var causedDamageRuleCount = 0;
            var receivedDamageRuleCount = 0;
            var blockRuleCount = 0;

            foreach (var rule in buff.EffectRules)
            {
                switch (rule)
                {
                    case CausedDamageEffectRule causedRule:
                        causedDamageRuleCount++;
                        var causedKey = causedDamageRuleCount == 1 ? "value" : $"value{causedDamageRuleCount}";
                        displayParams[causedKey] = causedRule.Value;
                        break;

                    case ReceivedDamageEffectRule receivedRule:
                        receivedDamageRuleCount++;
                        var receivedKey = receivedDamageRuleCount == 1 ? "value" : $"value{receivedDamageRuleCount}";
                        displayParams[receivedKey] = receivedRule.Value;
                        break;

                    case BlockEffectRule blockRule:
                        blockRuleCount++;
                        var blockKey = blockRuleCount == 1 ? "block_times" : $"block_times{blockRuleCount}";
                        displayParams[blockKey] = blockRule.RemainingTimes;
                        break;

                    case MiscEffectRule miscRule:
                        foreach (var kvp in miscRule.Parameters)
                        {
                            displayParams[kvp.Key] = kvp.Value;
                        }
                        break;
                }
            }

            foreach (var param in displayParams)
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