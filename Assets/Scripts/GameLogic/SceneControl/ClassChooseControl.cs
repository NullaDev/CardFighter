using System.Linq;
using GameLogic.Runtime;
using Registry;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic.SceneControl
{
    public class ClassChooseControl: MonoBehaviour
    {
        public void ChooseClassRU()
        {
            var config = StaticDataManager.InitialConfigManager.GetConfigFor(PlayerClass.RU);
            PlayerData.Instance.InitFromConfig(PlayerClass.RU, config);
            SceneManager.LoadScene("DeckInitialize");
        }
        
        public void DirectEnterTestStage()
        {
            var debug = StaticDataManager.InitialConfigManager.DebugConfig;
            
            var config = StaticDataManager.InitialConfigManager.GetConfigFor(PlayerClass.GENERIC);
            var playerData = PlayerData.Instance;
            PlayerData.Instance.InitFromConfig(PlayerClass.GENERIC, config);
            playerData.MaxHp = playerData.Hp = 255;
            
            foreach (var item in debug.DebugItems)
            {
                playerData.HeldItems.Add(StaticDataManager.HeldItemDataManager.Find(item));
            }
            
            playerData.CardOperations.Clear();
            playerData.CardOperations.SetMoveSlot(CommonCards.Move1);
            playerData.CardOperations.SetTurnSlot(CommonCards.TurnBack);
            foreach (var card in debug.DebugCards)
            {
                playerData.CardOperations.AddCard(StaticDataManager.CardDataManager.Find(card));
            }
            
            var stageData = StaticDataManager.StageDataManager;
            var stageList = debug.DebugStageType switch
            {
                "normal" => stageData.NormalStages.SelectMany(kv => kv.Value).ToList(),
                "elite" => stageData.EliteStages.SelectMany(kv => kv.Value).ToList(),
                "boss" => stageData.BossStages,
                _ => stageData.MiscStages,
            };
            MapData.Instance.CurrentStageConfig = stageList.Find(s=>s.ID.Equals(debug.DebugStageID));
            
            SceneManager.LoadScene("Fighting");
        }
    }
}