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

        public PlayerClass PlayerClass;
        public Deck DefaultDeck;
        
        public int HP;
        public int maxHP;
        public int maxCost;

        public int gold;

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
            
            // TODO remove hard code
            this.PlayerClass = PlayerClass.FIGHTER;
            this.maxHP = this.HP = 10;
            this.maxCost = 5;
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
