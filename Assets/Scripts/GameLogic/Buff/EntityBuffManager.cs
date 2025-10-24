using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.Buff
{
    public static class EntityBuffManager
    {
        public enum BuffType
        {
            Positive,
            Neutral,
            Negative
        }

        public static BuffType FromString(string buffName)
        {
            if (Enum.TryParse<BuffType>(buffName, true, out var buffType))
            {
                return buffType;
            }
            else
            {
                throw new ArgumentException($"Invalid BuffType: {buffName}");
            }
        }
        
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
            { Noble , new HashSet<string> { GenericValueKey } },
            { Vulnerable, new HashSet<string> { GenericValueKey } }
        };
        
        public const string Insight = "insight";
        public const string Stunned = "stunned";
        public const string Block = "block";
        public const string CounterAttack = "counter_attack";
        public const string Initiative = "initiative";
        public const string Vulnerable = "vulnerable";
        public const string SuperArmor = "super_armor";
        public const string Rooted = "rooted";
        public const string LockedFacing = "locked_facing";
        
        public const string Rites = "rites";
        public const string Music = "music";
            public const string MusicPositiveValue = "music_positive_value";
            public const string MusicNegativeValue = "music_negative_value";
            public const string Harmony = "harmony";
            public const string Chaos = "chaos";
        public const string Archery = "archery";
        public const string Charioteering = "charioteering";
        public const string Calligraphy = "calligraphy";
            public const string CalligraphyPositiveValue = "calligraphy_positive_value";
            public const string CalligraphyNegativeValue = "calligraphy_negative_value";
        public const string Mathematics = "mathematics";
        
        public const string Noble = "noble";
        public const string FollowHeart = "follow_heart";
        public const string Practice = "practice";
            public const string PracticeValue = "practice_value";
        public const string NobleUnarmed = "noble_unarmed";

        public const string GenericValueKey = "value";

    }
}