using System.Collections.Generic;
using Card.Engine;
using Newtonsoft.Json;

namespace Registry.Data
{
    public class CardPrototype
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string TextureName { get; set; }
        public string Desc { get; set; }
        public int Cost { get; set; }
        [JsonIgnore] public bool IsFusionCard = false;
        public List<EntityAction> Actions { get; set; } = new();
        
        public static CardPrototype CreateFromJson(string jsonString)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new EntityActionConverter());
            return JsonConvert.DeserializeObject<CardPrototype>(jsonString, settings);
        }
    }
}