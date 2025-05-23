using UnityEngine;
using UnityEngine.UI;

namespace Render
{
    public class DeckInitializeUIRender : MonoBehaviour
    {
        public Text countText;
        public Text costText;

        public void RenderCount(int count)
        {
            countText.text = $"当前卡牌数：{count}";
        }
        
        public void RenderCost(int cost, int max)
        {
            costText.text = $"剩余费用：{cost} / {max}";
        }
    }
}
