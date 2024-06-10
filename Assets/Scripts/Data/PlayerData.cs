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
            this.MaxHP = this.HP = 10;
            this.MaxCost = 5;
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
