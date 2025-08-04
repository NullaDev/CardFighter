using Registry;
using Render;
using UnityEngine;

namespace GameLogic.SceneControl
{
    public class OptionChooseControl : MonoBehaviour
    {
        public GameObject render;

        private void Awake()
        {
            var playerData = PlayerData.Instance;
            render.GetComponent<OptionChooseRender>().Render(playerData);
        }
    }
}