using System.Linq;
using GameLogic;
using GameLogic.Runtime;
using Registry;
using UnityEngine;
using UnityEngine.UI;

namespace Render
{
    public class RogueMapUIRender : MonoBehaviour
    {
        public Text PlayerHPText;
        public Text HeldCardTypeText;
        public Text HeldCardNumberText;
        public Text HeldItemNumberText;

        public void Render()
        {
            var playerData = PlayerData.Instance;
            var hpText = $"剩余血量：{playerData.Hp}/{playerData.MaxHp}";
            PlayerHPText.text = hpText;

            var cardTypeText = $"携带卡牌种类：{playerData.HeldCards.Keys.Count}";
            var cardNumberText = $"携带卡牌数量：{playerData.HeldCards.Values.Sum()}";
            HeldCardTypeText.text = cardTypeText;
            HeldCardNumberText.text = cardNumberText;
            
            var itemNumberText = $"携带物品数量：{playerData.HeldItems.Count}";
            HeldItemNumberText.text = itemNumberText;
        }
    }
}