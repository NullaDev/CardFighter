using System.Collections.Generic;
using Entity;
using GameLogic;
using Newtonsoft.Json;

namespace Registry.Data
{
    public class StageConfig
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Difficulty { get; set; }
        public int Size { get; set; }
        public int PlayerSpawnPos { get; set; }
        public string PlayerSpawnFacing { get; set; }
        public List<EntityConfig> Entities;
        
        public static StageConfig CreateFromJson(string jsonString)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new EntityConverter());
            
            return JsonConvert.DeserializeObject<StageConfig>(jsonString, settings);
        }
        
        public StageConfig Clone()
        {
            return new StageConfig
            {
                ID = this.ID,
                Name = this.Name,
                Type = this.Type,
                Difficulty = this.Difficulty,
                Size = this.Size,
                PlayerSpawnPos = this.PlayerSpawnPos,
                PlayerSpawnFacing = this.PlayerSpawnFacing,
                Entities = this.Entities?.ConvertAll(e => e.Clone())
            };
        }
    }
    
}