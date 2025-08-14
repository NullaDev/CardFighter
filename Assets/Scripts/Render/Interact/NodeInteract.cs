using System;
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
            playerData.CurrentNodeType = this.Node.Type;
            switch (playerData.CurrentNodeType)
            {
                case NodeType.FIGHT:
                    playerData.CurrentStage = stageData.GetNormalStage(this.Node.LayerDifficulty);
                    playerData.CurrentLayerDifficulty = this.Node.LayerDifficulty;
                    SceneManager.LoadScene("Fighting");
                    break;
                case NodeType.ELITE_FIGHT:
                    playerData.CurrentStage = stageData.GetEliteStage(this.Node.LayerDifficulty);
                    playerData.CurrentLayerDifficulty = this.Node.LayerDifficulty;
                    SceneManager.LoadScene("Fighting");
                    break;
                case NodeType.BOSS:
                    playerData.CurrentStage = stageData.GetBossStage();
                    SceneManager.LoadScene("Fighting");
                    break;
                case NodeType.EVENT:
                    playerData.OptionBundle = StaticDataManager.OptionDataManager.GetRandomEventBundle();
                    SceneManager.LoadScene("OptionChoose");
                    break;
                case NodeType.REST:
                    playerData.OptionBundle = StaticDataManager.OptionDataManager.GetBundle("rest");
                    SceneManager.LoadScene("OptionChoose");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
    }
}