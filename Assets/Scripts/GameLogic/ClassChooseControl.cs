using Registry;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic
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
            playerData.MaxHp = playerData.Hp = 10;
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
            
            playerData.CardOperations.AddPrototype(StaticDataManager.CardDataManager.Find("focus_energy"));
            playerData.CardOperations.AddPrototype(StaticDataManager.CardDataManager.Find("observe"));
            playerData.CardOperations.AddPrototype(StaticDataManager.CardDataManager.Find("must_have_teacher"));
            playerData.CardOperations.AddPrototype(StaticDataManager.CardDataManager.Find("noble_word"));
            playerData.CardOperations.AddPrototype(StaticDataManager.CardDataManager.Find("mathematics"));
            playerData.CardOperations.AddPrototype(StaticDataManager.CardDataManager.Find("lift_city_bar"));
            
            var stageData = StaticDataManager.StageDataManager;
            playerData.CurrentStage = stageData.MiscStages.Find(s=>s.ID.Equals("hurt"));
            
            SceneManager.LoadScene("Fighting");
        }
    }
}