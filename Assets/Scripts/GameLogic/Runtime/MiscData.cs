using System;
using GameLogic.Option;

namespace GameLogic.Runtime
{
    public class MiscData
    {
        public static MiscData Instance = new();
        private MiscData() {}
        
        public int Seed { get; private set; }
        private Random _random;
        public Random GlobalRandom
        {
            get { return _random ??= new Random(Seed); }
        }
        
        public void InitSeed(int? seed = null)
        {
            Seed = seed ?? new Random().Next();
            _random = null;
        }

        public OptionBundle OptionBundle = null;
        
        public bool InSavingMode = false;
    }
}