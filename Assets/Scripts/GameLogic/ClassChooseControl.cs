using Card;
using Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic
{
    public class ClassChooseControl: MonoBehaviour
    {
        public void DirectEnterTestStage()
        {
            var playerData = GameObject.Find("PlayerData").GetComponent<PlayerData>();
            playerData.PlayerClass = PlayerClass.RU;
            playerData.MaxHp = playerData.Hp = 10;
            playerData.InitialCost = 1;
            playerData.MaxCost = 5;

            playerData.DefaultDeck = new Deck(playerData.PlayerClass);
            playerData.DefaultDeck.AddPrototype(CardData.Instance.Find("move"));
            playerData.DefaultDeck.AddPrototype(CardData.Instance.Find("turn_back"));
            playerData.DefaultDeck.AddPrototype(CardData.Instance.Find("focus_energy"));
            playerData.DefaultDeck.AddPrototype(CardData.Instance.Find("observe"));
            
            playerData.DefaultDeck.AddPrototype(CardData.Instance.Find("punch"));
            playerData.DefaultDeck.AddPrototype(CardData.Instance.Find("kick"));
            playerData.DefaultDeck.AddPrototype(CardData.Instance.Find("broadsword"));
            playerData.DefaultDeck.AddPrototype(CardData.Instance.Find("rites"));
            
            var stageData = GameObject.Find("StageData").GetComponent<StageData>();
            playerData.CurrentStage = stageData.MiscStages.Find(s=>s.ID.Equals("dummy"));
            
            SceneManager.LoadScene("Fighting");
        }
    }
}