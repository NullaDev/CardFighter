using System;
using GameLogic;
using GameLogic.Map;
using GameLogic.Runtime;
using GameLogic.SceneControl;
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
            var mapData = MapData.Instance;

            if (!mapData.MoveToNode(Node))
            {
                Debug.LogWarning("[NodeInteract] Invalid node click.");
                return;
            }
            
            var mapRender = GameObject.Find("Render").GetComponent<RogueMapRender>();
            mapRender.RerenderAccordingToPlayerPos(mapData.CurrentMap);
            
            var difficulty = Node.Layer;  // TODO
            switch (Node.Type)
            {
                case NodeType.FIGHT:
                    mapData.CurrentStageConfig = StaticDataManager.StageDataManager.GetNormalStage(difficulty);
                    SceneManager.LoadScene("Fighting");
                    break;
                case NodeType.ELITE_FIGHT:
                    mapData.CurrentStageConfig = StaticDataManager.StageDataManager.GetEliteStage(difficulty);
                    SceneManager.LoadScene("Fighting");
                    break;
                case NodeType.BOSS:
                    mapData.CurrentStageConfig = StaticDataManager.StageDataManager.GetBossStage();
                    SceneManager.LoadScene("Fighting");
                    break;
                case NodeType.EVENT:
                    mapData.OptionBundle = StaticDataManager.OptionDataManager.GetRandomEventBundle();
                    SceneManager.LoadScene("OptionChoose");
                    break;
                case NodeType.REST:
                    mapData.OptionBundle = StaticDataManager.OptionDataManager.GetBundle("rest");
                    SceneManager.LoadScene("OptionChoose");
                    break;
                default:
                    Debug.LogError("[NodeInteract] Unexpected NodeType");
                    break;
            }
        }
        
    }
}