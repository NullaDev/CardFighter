using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Card
{
    public class CardJsonConverter : JsonConverter
    {
        private Dictionary<string, Type> typeMap;

        public CardJsonConverter()
        {
            Type cardEntityType = typeof(BaseCardEntity);
            this.typeMap =
                Assembly.GetExecutingAssembly().GetTypes()
                    .Where(item => item.IsSubclassOf(cardEntityType) && !item.IsAbstract)
                    .ToDictionary(it => it.FullName);
        }


        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            JObject item = JObject.Load(reader);
            var itemStr = item["type"]?.ToString();
            if (itemStr == null) throw new KeyNotFoundException("not found type");
            return item.ToObject(typeMap[itemStr], serializer);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(BaseCardEntity).IsAssignableFrom(objectType);
        }
    }
}