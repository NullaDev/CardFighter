using Render;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic.RogueMap
{
    public class RogueMapControl: MonoBehaviour
    {
        public GameObject render;

        public RogueMap Map;

        public void Start()
        {
            if (RogueMap.GlobalMap == null)
            {
                RogueMap.GlobalMap = RogueMap.GenerateRandomMap(14);
            }
            Map = RogueMap.GlobalMap;

            Rerender();
        }

        private void Rerender()
        {
            var mapRender = this.render.GetComponent<RogueMapRender>();
            mapRender.RenderRogueMap(this.Map);

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