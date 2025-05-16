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
        
        public static readonly Dictionary<string, List<string>> StackableBuffs = new()
        {
            { Noble , new List<string> { NobleValue } }
        };
        
        public const string Insight = "insight";
        
        public const string Rites = "rites";
        public const string Music = "music";
        public const string Archery = "archery";
        public const string Charioteering = "charioteering";
        public const string Calligraphy = "calligraphy";
        public const string Mathematics = "mathematics";
        
        public const string Noble = "noble";
        public const string NobleValue = "noble_value";

    }
}