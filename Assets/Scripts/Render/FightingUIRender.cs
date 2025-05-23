using UnityEngine;
using UnityEngine.UI;

namespace Render
{
    public class FightingUIRender : MonoBehaviour
    {
        public Text turnText;
        public Text costText;

        public void RenderTurn(int turn)
        {
            turnText.text = $"回合：{turn}";
        }
        
        public void RenderCost(int current, int max)
        {
            costText.text = $"费用：{current} / {max}";
        }
        
    }
}
