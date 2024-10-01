using System.Collections.Generic;
using Card;
using UnityEngine;
using Newtonsoft.Json;

namespace GameLogic
{
    public class StageConfig
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
        public int PlayerSpawnPos { get; set; }
        public string PlayerSpawnFacing { get; set; }
        public List<EnemyConfig> Mobs;
        
        public static StageConfig CreateFromJson(string jsonString)
        {
            return JsonConvert.DeserializeObject<StageConfig>(jsonString);
        }
    }
    
}