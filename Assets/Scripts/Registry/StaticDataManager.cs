namespace Registry
{
    public static class StaticDataManager
    {
        public static readonly CardDataManager CardDataManager = new();
        public static readonly StageDataManager StageDataManager = new();
        public static readonly BuffDataManager BuffDataManager = new();

        public static void LoadAll()
        {
            CardDataManager.LoadFromFile();
            StageDataManager.LoadFromFile();
            BuffDataManager.LoadFromFile();
            CommonCards.Init();
        }
    }
}