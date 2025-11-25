using GameLogic.Map;
using GameLogic.Runtime;
using Render;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic.SceneControl
{
    public class RogueMapControl: MonoBehaviour
    {
        public GameObject render;
        
        public void Start()
        {
            MapData.Instance.Initialize();
            Rerender();
        }

        private void Rerender()
        {
            var mapRender = this.render.GetComponent<RogueMapRender>();
            mapRender.RenderRogueMap(MapData.Instance.CurrentMap);

            var uiRender = this.render.GetComponent<RogueMapUIRender>();
            uiRender.Render();
        }

        public void ClickModify()
        {
            SceneManager.LoadScene("DeckModify");
        }
        
        public void ClickReturn()
        {
            SceneManager.LoadScene("ClassChoose");
        }
    }
}