using Card;
using GameLogic;
using UnityEngine;

namespace Data
{
    public class PlayerData : MonoBehaviour
    {
        public static PlayerData Instance;

        public PlayerClass PlayerClass;
        public Deck DefaultDeck;
        
        public int HP;
        public int MaxHP;
        public int MaxCost;

        public PlayerData(PlayerClass playerClass)
        {
            this.PlayerClass = playerClass;
            this.DefaultDeck = new Deck(playerClass);
        }

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
