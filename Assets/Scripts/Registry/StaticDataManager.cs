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
        public static readonly OptionDataManager OptionDataManager = new();
        public static readonly InitialConfigManager InitialConfigManager = new();
        public static readonly GlobalMapDataManager GlobalMapDataManager = new();
        public static readonly ShopManager ShopManager = new();

        public static void LoadAll()
        {
            CardDataManager.LoadFromFile();
            InitialDeckManager.LoadFromFile();
            HeldItemDataManager.LoadFromFile();
            StageDataManager.LoadFromFile();
            BuffDisplayManager.LoadFromFile();
            RecipeDataManager.LoadFromFile();
            OptionDataManager.LoadFromFile();
            InitialConfigManager.LoadFromFile();
            GlobalMapDataManager.LoadFromFile();
            ShopManager.LoadFromFile();
            CommonCards.Init();
        }
    }
}