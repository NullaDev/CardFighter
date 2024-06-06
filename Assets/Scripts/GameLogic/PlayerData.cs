using Card;
using Data;
using UnityEngine;

namespace GameLogic
{
    public class PlayerData : MonoBehaviour
    {
        private static PlayerData Instance;

        public PlayerClass PlayerClass;
        public Deck DefaultDeck;

        public PlayerData(PlayerClass playerClass)
        {
            this.PlayerClass = playerClass;
            this.DefaultDeck = new Deck(this.PlayerClass);
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
