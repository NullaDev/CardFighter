using System;
using GameLogic.Option;

namespace GameLogic.Runtime
{
    public class MiscData
    {
        public static MiscData Instance = new();
        private MiscData() {}
        
        public readonly Random Random = new(Seed:19260817);

        public OptionBundle OptionBundle = null;
    }
}