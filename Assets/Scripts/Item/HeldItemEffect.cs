using System.Collections.Generic;
using Registry.Data;

namespace Item
{
    public abstract class HeldItemEffect
    {
        public string EffectType { get; set; }
    }
    
    public class StartingBuffEffect : HeldItemEffect
    {
        public List<BuffData> Buffs { get; set; } = new();
    }

    public class GrantCardOnObtainEffect : HeldItemEffect
    {
        public List<string> CardIDs { get; set; } = new();
        public bool IsRandom { get; set; } = false;
        public int Count { get; set; } = 1;
    }

    public class RecipeFreeCardEffect : HeldItemEffect
    {
        public List<string> CardIDs { get; set; } = new();
    }

    public class ReplaceCardEffect : HeldItemEffect
    {
        public Dictionary<string, string> ReplacementMap { get; set; } = new();
    }
    
    public class MiscEffect : HeldItemEffect
    {
        public Dictionary<string, object> Parameters { get; set; } = new();
    }
}