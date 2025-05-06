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
            Map = RogueMap.GenerateRandomMap(15);

            Rerender();
        }

        void Rerender()
        {
            var mapRender = this.render.GetComponent<RogueMapRender>();
            mapRender.RenderRogueMap(this.Map);
        }
    }
}