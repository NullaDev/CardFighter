using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

            CardBehavior effect = type switch
            {
                "damage" => new DamageBehavior(),
                "turn_back" => new TurnBackBehavior(),
                "move_forward" => new MoveForwardBehavior(),
                _ => throw new Exception("Unknown effect type")
            };

            serializer.Populate(jsonObject.CreateReader(), effect);
            return effect;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(CardBehavior);
        }
    }
}