using Card;
using GameLogic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data
{
    // This script manages the player's "in-game" information, such as deck, hp and gold.
    public class PlayerData : MonoBehaviour
    {
        public static PlayerData Instance;

        public PlayerClass PlayerClass { get; set; }
        public int Hp { get; set; }
        public int MaxHp { get; set; }
        public int InitialCost { get; set; }
        public int MaxCost { get; set; }

        public int InGameGold { get; set; }
        public Deck DefaultDeck;

        void Start()
        {
        
        }

        void Update()
        {
        
        }
    
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        private void LoadFromFile()
        {
            // TODO parse local json
        }
    
        private void SaveToFile()
        {
            // TODO write to local json
        }
    }
}
