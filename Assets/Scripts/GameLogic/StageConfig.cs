using System.Collections.Generic;
using Card;
using UnityEngine;
using Newtonsoft.Json;

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
            return JsonConvert.DeserializeObject<StageConfig>(jsonString);
        }
    }
    
}