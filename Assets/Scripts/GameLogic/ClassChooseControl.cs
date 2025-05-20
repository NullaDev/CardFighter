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
        
        public void DirectEnterTestStage()
        {
            var playerData = GameObject.Find("PlayerData").GetComponent<PlayerData>();
            playerData.PlayerClass = PlayerClass.RU;
            playerData.MaxHp = playerData.Hp = 10;
            playerData.InitialCost = 1;
            playerData.MaxCost = 5;

            playerData.DefaultDeck = new Deck(playerData.PlayerClass);
            playerData.DefaultDeck.AddPrototype(StaticDataManager.CardDataManager.Find("move"));
            playerData.DefaultDeck.AddPrototype(StaticDataManager.CardDataManager.Find("turn_back"));
            playerData.DefaultDeck.AddPrototype(StaticDataManager.CardDataManager.Find("focus_energy"));
            playerData.DefaultDeck.AddPrototype(StaticDataManager.CardDataManager.Find("observe"));
            
            playerData.DefaultDeck.AddPrototype(StaticDataManager.CardDataManager.Find("punch"));
            playerData.DefaultDeck.AddPrototype(StaticDataManager.CardDataManager.Find("bow"));
            playerData.DefaultDeck.AddPrototype(StaticDataManager.CardDataManager.Find("music"));
            playerData.DefaultDeck.AddPrototype(StaticDataManager.CardDataManager.Find("charioteering"));
            
            var stageData = StaticDataManager.StageDataManager;
            playerData.CurrentStage = stageData.MiscStages.Find(s=>s.ID.Equals("dummy"));
            
            SceneManager.LoadScene("Fighting");
        }
    }
}