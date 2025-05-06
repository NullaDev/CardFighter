using Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RogueMap
{
    public class NodeInteract: MonoBehaviour, IPointerClickHandler
    {
        public MapNode Node;
        public void OnPointerClick(PointerEventData eventData)
        {
            var playerData = GameObject.Find("PlayerData").GetComponent<PlayerData>();
            playerData.currentStage = Node.Type.ToString();
        }
        
    }
}