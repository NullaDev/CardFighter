using Render;
using UnityEngine;

namespace SceneControl
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