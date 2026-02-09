using System.Collections.Generic;
using Registry.Data;

namespace GameLogic.Item
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

    public class MaxHpBonusEffect : HeldItemEffect
    {
        public int Value { get; set; }
        public bool HealCurrent { get; set; } = false;
    }

    public class InitialCostBonusEffect : HeldItemEffect
    {
        public int Value { get; set; }
    }

    public class StartingArmorEffect : HeldItemEffect
    {
        public int Value { get; set; }
    }

    public class HealAfterBattleEffect : HeldItemEffect
    {
        public int Value { get; set; }
    }
}