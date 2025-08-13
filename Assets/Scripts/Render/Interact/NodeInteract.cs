using GameLogic.RogueMap;
using Registry;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Render.Interact
{
    public class NodeInteract: MonoBehaviour, IPointerClickHandler
    {
        public MapNode Node;
        public void OnPointerClick(PointerEventData eventData)
        {
            var mapControl = GameObject.Find("RogueMapControl").GetComponent<RogueMapControl>();
            mapControl.Map.SetPlayerNode(this.Node);

            var mapRender = GameObject.Find("Render").GetComponent<RogueMapRender>();
            mapRender.RerenderAccordingToPlayerPos(mapControl.Map);

            var playerData = PlayerData.Instance;
            var stageData = StaticDataManager.StageDataManager;
            if (this.Node.Type == NodeType.FIGHT)
            {
                playerData.CurrentNodeType = NodeType.FIGHT;
                playerData.CurrentStage = stageData.GetNormalStage(this.Node.LayerDifficulty);
                playerData.CurrentLayerDifficulty = this.Node.LayerDifficulty;
                SceneManager.LoadScene("Fighting");
            }
            else if (this.Node.Type == NodeType.ELITE_FIGHT)
            {
                playerData.CurrentNodeType = NodeType.ELITE_FIGHT;
                playerData.CurrentStage = stageData.GetEliteStage(this.Node.LayerDifficulty);
                playerData.CurrentLayerDifficulty = this.Node.LayerDifficulty;
                SceneManager.LoadScene("Fighting");
            }
            else if (this.Node.Type == NodeType.BOSS)
            {
                playerData.CurrentNodeType = NodeType.BOSS;
                //TODO
            }
            else if (this.Node.Type == NodeType.EVENT)
            {
                //TODO
            }
            else if (this.Node.Type == NodeType.REST)
            {
                playerData.CurrentNodeType = NodeType.REST;
                playerData.OptionBundle = StaticDataManager.OptionDataManager.GetBundle("rest");
                SceneManager.LoadScene("OptionChoose");
            }
        }
        
    }
}