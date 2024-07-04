using UnityEngine;

namespace Data
{
    // This script manages the "global" information of the player, such as class unlocks.
    public class SaveData : MonoBehaviour
    {
        public static SaveData Instance;

        // Some saved data
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
            LoadFromFile();
            DontDestroyOnLoad(gameObject);
        }

        private void LoadFromFile()
        {
            // TODO parse local json
            this.gold = 0;
        }
    
        private void SaveToFile()
        {
            // TODO write to local json
        }
    }
}
