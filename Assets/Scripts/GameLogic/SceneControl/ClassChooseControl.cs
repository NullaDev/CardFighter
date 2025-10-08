using Registry;
using Render;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic.SceneControl
{
    public class ClassChooseControl: MonoBehaviour
    {
        public void ChooseClassRU()
        {
            var playerData = PlayerData.Instance;
            playerData.PlayerClass = PlayerClass.RU;
            playerData.MaxHp = playerData.Hp = 20;
            playerData.InitialInGameCost = 1;
            playerData.MaxInGameCost = 5;
            
            SceneManager.LoadScene("DeckInitialize");
        }
        
        public void DirectEnterTestStage()
        {
            var playerData = PlayerData.Instance;
            playerData.MaxHp = playerData.Hp = 255;
            playerData.HeldItems.Add(StaticDataManager.HeldItemDataManager.Find("telescope_debug"));

            playerData.CardOperations.Clear();
            playerData.CardOperations.SetMoveSlot(CommonCards.Move1);
            playerData.CardOperations.SetTurnSlot(CommonCards.TurnBack);
            
            playerData.CardOperations.AddCard(StaticDataManager.CardDataManager.Find("focus_energy"));
            playerData.CardOperations.AddCard(StaticDataManager.CardDataManager.Find("observe"));
            playerData.CardOperations.AddCard(StaticDataManager.CardDataManager.Find("archery"));
            playerData.CardOperations.AddCard(StaticDataManager.CardDataManager.Find("fourfold_strike"));
            playerData.CardOperations.AddCard(StaticDataManager.CardDataManager.Find("study_and_practice"));
            playerData.CardOperations.AddCard(StaticDataManager.CardDataManager.Find("throw_stone"));
            
            var stageData = StaticDataManager.StageDataManager;
            // playerData.CurrentStage = stageData.MiscStages.Find(s=>s.ID.Equals("dummy"));
            // playerData.CurrentStage = stageData.NormalStages[3].Find(s=>s.ID.Equals("yin_yang_altar_3"));
            playerData.CurrentStage = stageData.EliteStages[0].Find(s=>s.ID.Equals("sunzi_fake_0"));
            
            SceneManager.LoadScene("Fighting");
        }
    }
}