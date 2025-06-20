namespace Registry
{
    public static class StaticDataManager
    {
        public static readonly CardDataManager CardDataManager = new();
        public static readonly InitialDeckManager InitialDeckManager = new();
        public static readonly HeldItemDataManager HeldItemDataManager = new();
        public static readonly StageDataManager StageDataManager = new();
        public static readonly BuffDisplayManager BuffDisplayManager = new();
        public static readonly RecipeDataManager RecipeDataManager = new();

        public static void LoadAll()
        {
            CardDataManager.LoadFromFile();
            InitialDeckManager.LoadFromFile();
            HeldItemDataManager.LoadFromFile();
            StageDataManager.LoadFromFile();
            BuffDisplayManager.LoadFromFile();
            RecipeDataManager.LoadFromFile();
            CommonCards.Init();
        }
    }
}