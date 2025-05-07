using Data;
using Render;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RogueMap
{
    public class NodeInteract: MonoBehaviour, IPointerClickHandler
    {
        public MapNode Node;
        public void OnPointerClick(PointerEventData eventData)
        {
            var mapControl = GameObject.Find("RogueMapControl").GetComponent<RogueMapControl>();
            mapControl.Map.SetPlayerNode(this.Node);

            var mapRender = GameObject.Find("Render").GetComponent<RogueMapRender>();
            mapRender.ReRenderAccordingToPlayerPos(mapControl.Map);
            
            var playerData = GameObject.Find("PlayerData").GetComponent<PlayerData>();
            // TODO
            playerData.currentStage = Node.Type.ToString();
        }
        
    }
}