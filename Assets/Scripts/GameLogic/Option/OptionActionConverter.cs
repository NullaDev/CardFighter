using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GameLogic.Option
{
    public class OptionActionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(OptionAction).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var type = jo["Type"]?.ToString();
            OptionAction action = type switch
            {
                "gold_change" => new GoldChangeAction(),
                "hp_change" => new HpChangeAction(),
                "hp_restore" => new HpRestoreAction(),
                "max_hp_change" => new MaxHpChangeAction(),
                "card_gain" => new CardGainAction(),
                "item_gain" => new ItemGainAction(),
                "card_random_lose" => new CardRandomLoseAction(),
                "item_random_lose" => new ItemRandomLoseAction(),
                _ => throw new JsonSerializationException($"Unknown Type: {type}")
            };

            serializer.Populate(jo.CreateReader(), action);
            return action;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}