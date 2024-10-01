using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Card
{
    public class BehaviorConverter: JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var type = jsonObject["Type"]?.ToString();
            CardBehavior behavior = type switch
            {
                "damage" => new DamageBehavior(),
                "turn_back" => new TurnBackBehavior(),
                "move_forward" => new MoveForwardBehavior(),
                "add_cost" => new AddCostBehavior(),
                _ => throw new Exception("Unknown effect type")
            };

            serializer.Populate(jsonObject.CreateReader(), behavior);
            return behavior;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(CardBehavior);
        }
    }
}