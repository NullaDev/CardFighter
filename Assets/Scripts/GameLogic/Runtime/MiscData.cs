using GameLogic.Option;

namespace GameLogic.Runtime
{
    public class MiscData
    {
        public static MiscData Instance = new();
        private MiscData() {}
        
        public OptionBundle OptionBundle = null;
    }
}