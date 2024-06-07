using Card;
using GameLogic;
using UnityEngine;

namespace Data
{
    public class PlayerData : MonoBehaviour
    {
        private static PlayerData Instance;

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
    }
}
