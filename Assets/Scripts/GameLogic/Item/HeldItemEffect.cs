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

    /// <summary>战斗开始时增加玩家最大生命值（可选同时回复等量当前生命）。</summary>
    public class MaxHpBonusEffect : HeldItemEffect
    {
        public int Value { get; set; }
        /// <summary>为 true 时，当前生命也增加 Value（不超过新的 MaxHp）。</summary>
        public bool HealCurrent { get; set; } = false;
    }

    /// <summary>战斗开始时额外获得的初始费用。</summary>
    public class InitialCostBonusEffect : HeldItemEffect
    {
        public int Value { get; set; }
    }

    /// <summary>战斗开始时获得的护甲。</summary>
    public class StartingArmorEffect : HeldItemEffect
    {
        public int Value { get; set; }
    }

    /// <summary>战斗胜利后回复的生命值。</summary>
    public class HealAfterBattleEffect : HeldItemEffect
    {
        public int Value { get; set; }
    }
}