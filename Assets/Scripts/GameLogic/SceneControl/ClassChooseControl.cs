using Registry;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic.SceneControl
{
    public class ClassChooseControl: MonoBehaviour
    {
        private void Awake()
        {
            StaticDataManager.LoadAll();
        }

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

            playerData.CardOperations.Clear();
            playerData.CardOperations.SetMoveSlot(CommonCards.Move1);
            playerData.CardOperations.SetTurnSlot(CommonCards.TurnBack);
            
            playerData.CardOperations.AddCard(StaticDataManager.CardDataManager.Find("focus_energy"));
            playerData.CardOperations.AddCard(StaticDataManager.CardDataManager.Find("observe"));
            playerData.CardOperations.AddCard(StaticDataManager.CardDataManager.Find("archery"));
            playerData.CardOperations.AddCard(StaticDataManager.CardDataManager.Find("fourfold_strike"));
            playerData.CardOperations.AddCard(StaticDataManager.CardDataManager.Find("teacher_say"));
            playerData.CardOperations.AddCard(StaticDataManager.CardDataManager.Find("stone_probe"));
            
            var stageData = StaticDataManager.StageDataManager;
            // playerData.CurrentStage = stageData.MiscStages.Find(s=>s.ID.Equals("dummy"));
            playerData.CurrentStage = stageData.NormalStages[2].Find(s=>s.ID.Equals("village_challenge_2"));
            
            SceneManager.LoadScene("Fighting");
        }
    }
}