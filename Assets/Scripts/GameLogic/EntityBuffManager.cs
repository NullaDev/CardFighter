using System;
using System.Collections.Generic;

namespace GameLogic
{
    public static class EntityBuffManager
    {
        public enum BuffType
        {
            Positive,
            Neutral,
            Negative
        }
        
        public static readonly HashSet<string> PositiveBuffs = new()
        {
            Insight, Block, CounterAttack, Initiative, HoneSword, SuperArmor,
            Noble, HonestWord, FollowHeart, Practice, HiddenWeapon, GoodAtTools, KindHeart, Fearless
        };
        
        public static readonly HashSet<string> NeutralBuffs = new()
        {
            Rites, Music, Archery, Charioteering, Calligraphy, Mathematics,
            NobleUnarmed
        };
        
        public static readonly HashSet<string> NegativeBuffs = new()
        {
            Stunned, Vulnerable, Rooted, LockedFacing,
            NowhereToHide
        };
        
        public static readonly List<HashSet<string>> BuffConflictGroups = new()
        {
            new HashSet<string> { Rites, Music, Archery, Charioteering, Calligraphy, Mathematics }
        };
        
        public static readonly Dictionary<string, HashSet<string>> BuffImmunityGroups = new()
        {
            { SuperArmor, new HashSet<string> { Stunned, Rooted, LockedFacing } }
        };

        public static readonly HashSet<string> ToggleBuffs = new()
        {
            Rites, Music, Archery, Charioteering, Calligraphy, Mathematics, NobleUnarmed
        };
        
        public static readonly Dictionary<string, HashSet<string>> StackableBuffs = new()
        {
            { Noble , new HashSet<string> { NobleValue } },
            { Vulnerable, new HashSet<string> { VulnerableValue } }
        };
        
        public const string Insight = "insight";
        public const string Stunned = "stunned";
        public const string Block = "block";
            public const string BlockTimes = "block_times";
        public const string CounterAttack = "counter_attack";
            public const string CounterAttackValue = "counter_attack_value";
        public const string Initiative = "initiative";
        public const string HoneSword = "hone_sword";
            public const string HoneSwordValue = "hone_sword_value";
        public const string Vulnerable = "vulnerable";
            public const string VulnerableValue = "vulnerable_value";
        public const string SuperArmor = "super_armor";
        public const string Rooted = "rooted";
        public const string LockedFacing = "locked_facing";
        
        public const string Rites = "rites";
            public const string RitesPositiveValue = "rites_positive_value";
            public const string RitesNegativeValue = "rites_negative_value";
        public const string Music = "music";
            public const string MusicPositiveValue = "music_positive_value";
            public const string MusicNegativeValue = "music_negative_value";
            public const string Harmony = "harmony";
            public const string Chaos = "chaos";
            public const string HarmonyValue = "music_harmony_value";
            public const string ChaosValue = "music_chaos_value";
        public const string Archery = "archery";
            public const string ArcheryPositiveValue = "archery_positive_value";
            public const string ArcheryNegativeValue = "archery_negative_value";
        public const string Charioteering = "charioteering";
            public const string CharioteeringValue = "charioteering_value";
        public const string Calligraphy = "calligraphy";
            public const string CalligraphyPositiveValue = "calligraphy_positive_value";
            public const string CalligraphyNegativeValue = "calligraphy_negative_value";
        public const string Mathematics = "mathematics";
            public const string MathematicsPositiveValue = "mathematics_positive_value";
            public const string MathematicsNegativeValue = "mathematics_negative_value";
        
        public const string Noble = "noble";
            public const string NobleValue = "noble_value";
        public const string HonestWord = "honest_word";
            public const string HonestWordValue = "honest_word_value";
        public const string FollowHeart = "follow_heart";
        public const string Practice = "practice";
            public const string PracticeValue = "practice_value";
        public const string HiddenWeapon = "hidden_weapon";
        public const string NobleUnarmed = "noble_unarmed";
            public const string NobleUnarmedPositiveValue = "noble_unarmed_positive_value";
            public const string NobleUnarmedNegativeValue = "noble_unarmed_negative_value";
        public const string GoodAtTools = "good_at_tools";
            public const string GoodAtToolsValue = "good_at_tools_value";
        public const string NowhereToHide = "nowhere_to_hide";
            public const string NowhereToHideValue = "nowhere_to_hide_value";
        public const string KindHeart = "kind_heart";
            public const string KindHeartValue = "kind_heart_value";
        public const string Fearless = "fearless";
            public const string FearlessValue = "fearless_value";
            
        public static BuffType GetBuffType(string buffName)
        {
            if (PositiveBuffs.Contains(buffName))
                return BuffType.Positive;
            if (NeutralBuffs.Contains(buffName))
                return BuffType.Neutral;
            if (NegativeBuffs.Contains(buffName))
                return BuffType.Negative;
            throw new Exception($"Unknown buff name: {buffName}");
        }

    }
}