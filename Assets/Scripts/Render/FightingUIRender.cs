using GameLogic.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace Render
{
    public class FightingUIRender : MonoBehaviour
    {
        public Text turnText;
        public Text costText;
        public Text dmgModifierText;

        public void RenderTurn(int turn)
        {
            turnText.text = $"回合：{turn}";
        }
        
        public void RenderCost(int current, int max)
        {
            costText.text = $"费用：{current} / {max}";
        }
        
        public void RenderModifier(float modifier)
        {
            dmgModifierText.text = $"伤害：x{MapData.Instance.CurrentMapAttackModifier:F2}";
        }
        
    }
}
