using Card;
using Registry.Data;

namespace Registry
{
    public static class CommonCards
    {
        public static CardPrototype DoNothing;
        public static CardPrototype TurnBack;
        public static CardPrototype Move1;

        public static void Init()
        {
            DoNothing = StaticDataManager.CardDataManager.Find("do_nothing");
            TurnBack = StaticDataManager.CardDataManager.Find("turn_back");
            Move1 = StaticDataManager.CardDataManager.Find("move");
        }
    }
}