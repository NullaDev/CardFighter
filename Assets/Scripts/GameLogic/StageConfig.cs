using System.Collections.Generic;
using Card;
using UnityEngine;

namespace GameLogic
{
    public class StageConfig
    {
        public string ID;
        public string Name;
        public int Size;
        public int PlayerSpawnPos;
        public List<EnemyConfig> Mobs;
        
        public static StageConfig CreateFromJson(string jsonString)
        {
            return JsonUtility.FromJson<StageConfig>(jsonString);
        }
    }
    
}