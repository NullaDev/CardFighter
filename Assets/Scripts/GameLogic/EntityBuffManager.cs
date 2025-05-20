using System.Collections.Generic;

namespace GameLogic
{
    public class EntityBuffManager
    {
        public static readonly List<HashSet<string>> BuffConflictGroups = new()
        {
            new HashSet<string> { Rites, Music, Archery, Charioteering, Calligraphy, Mathematics }
        };

        public static readonly HashSet<string> ToggleBuffs = new()
        {
            Rites, Music, Archery, Charioteering, Calligraphy, Mathematics
        };
        
        public static readonly Dictionary<string, HashSet<string>> StackableBuffs = new()
        {
            { Noble , new HashSet<string> { NobleValue } }
        };
        
        public const string Insight = "insight";
        public const string Stunned = "stunned";
        public const string Block = "block";
            public const string BlockTimes = "block_times";
        
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
        public const string Mathematics = "mathematics";
            public const string MathematicsPositiveValue = "mathematics_positive_value";
            public const string MathematicsNegativeValue = "mathematics_negative_value";
        
        public const string Noble = "noble";
            public const string NobleValue = "noble_value";
        
        public const string HonestWord = "honest_word";
            public const string HonestWordValue = "honest_word_value";

    }
}