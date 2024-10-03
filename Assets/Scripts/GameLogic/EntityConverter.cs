using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameLogic
{
    public class EntityConverter: JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Debug.Log("hello");
            var jsonObject = JObject.Load(reader);
            var type = jsonObject["Type"]?.ToString();
            EntityConfig entity = type switch
            {
                "passive" => new PassiveEntityConfig(),
                "simple_enemy" => new SimpleEnemyConfig(),
                "elite_enemy" => new EliteEntityConfig(),
                _ => throw new Exception("Unknown entity type")
            };

            serializer.Populate(jsonObject.CreateReader(), entity);
            return entity;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(EntityConfig);
        }
    }
}