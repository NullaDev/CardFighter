using Card;
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

            playerData.DefaultDeck = new Deck(playerData.PlayerClass);
            playerData.DefaultDeck.AddPrototype(StaticDataManager.CardDataManager.Find("move"));
            playerData.DefaultDeck.AddPrototype(StaticDataManager.CardDataManager.Find("turn_back"));
            playerData.DefaultDeck.AddPrototype(StaticDataManager.CardDataManager.Find("focus_energy"));
            playerData.DefaultDeck.AddPrototype(StaticDataManager.CardDataManager.Find("observe"));
            
            playerData.DefaultDeck.AddPrototype(StaticDataManager.CardDataManager.Find("lift_gate"));
            playerData.DefaultDeck.AddPrototype(StaticDataManager.CardDataManager.Find("noble_word"));
            playerData.DefaultDeck.AddPrototype(StaticDataManager.CardDataManager.Find("from_a_distance"));
            playerData.DefaultDeck.AddPrototype(StaticDataManager.CardDataManager.Find("iron_slam"));
            
            var stageData = StaticDataManager.StageDataManager;
            playerData.CurrentStage = stageData.MiscStages.Find(s=>s.ID.Equals("dummy"));
            
            SceneManager.LoadScene("Fighting");
        }
    }
}