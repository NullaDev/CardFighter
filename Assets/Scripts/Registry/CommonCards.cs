using Registry.Data;

namespace Registry
{
    public static class CommonCards
    {
        public static CardPrototype DoNothing;
        public static CardPrototype TurnBack;
        public static CardPrototype UTurn;
        public static CardPrototype Move1;
        public static CardPrototype Drive;
        
        public static void Init()
        {
            DoNothing = StaticDataManager.CardDataManager.Find("do_nothing");
            TurnBack = StaticDataManager.CardDataManager.Find("turn_back");
            UTurn = StaticDataManager.CardDataManager.Find("u_turn");
            Move1 = StaticDataManager.CardDataManager.Find("move");
            Drive = StaticDataManager.CardDataManager.Find("drive");
        }
    }
}