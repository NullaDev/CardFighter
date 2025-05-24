using Render;
using UnityEngine;

namespace RogueMap
{
    public class RogueMapControl: MonoBehaviour
    {
        public GameObject render;

        public RogueMap Map;

        void Start()
        {
            if (RogueMap.GlobalMap == null)
            {
                RogueMap.GlobalMap = RogueMap.GenerateRandomMap(14);
            }
            Map = RogueMap.GlobalMap;

            Rerender();
        }

        void Rerender()
        {
            var mapRender = this.render.GetComponent<RogueMapRender>();
            mapRender.RenderRogueMap(this.Map);
        }
    }
}