using Render;
using UnityEngine;

namespace GameLogic.SceneControl
{
    public class OptionChooseControl : MonoBehaviour
    {
        public GameObject render;

        private void Awake()
        {
            render.GetComponent<OptionChooseRender>().Render();
        }
    }
}