using UnityEngine;
using UnityEngine.UI;

namespace Render
{
    public class UIRender : MonoBehaviour
    {
        public Text turnText;

        public void RenderTurn(int turn)
        {
            turnText.text = "回合："+turn;
        }
        
    }
}
