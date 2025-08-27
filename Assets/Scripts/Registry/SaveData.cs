namespace Registry
{
    // This script manages the "global" information of the player, such as class unlocks.
    public class SaveData
    {
        public static readonly SaveData Instance = new();

        // Some saved data
        public int Gold;

        private void LoadFromFile()
        {
            // TODO parse local json
            this.Gold = 0;
        }
    
        private void SaveToFile()
        {
            // TODO write to local json
        }
    }
}
