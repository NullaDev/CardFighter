using System;
using GameLogic.Buff;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Item
{
    public class HeldItemEffectConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(HeldItemEffect);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            var type = obj["EffectType"]?.ToString();

            return type switch
            {
                "starting_buff" => new StartingBuffEffect
                {
                    Buffs = BuffEffectRule.ParseBuffs(obj["Buffs"], serializer)
                },
                "grant_card_on_obtain" => obj.ToObject<GrantCardOnObtainEffect>(serializer),
                "synthesis_free_card" => obj.ToObject<SynthesisFreeCardEffect>(serializer),
                "replace_card" => obj.ToObject<ReplaceCardEffect>(serializer),
                "misc" => obj.ToObject<MiscEffect>(serializer),
                _ => throw new Exception("Unknown HeldItemEffect type: " + type)
            };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

}