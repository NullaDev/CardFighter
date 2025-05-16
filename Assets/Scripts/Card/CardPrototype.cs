using System.Collections.Generic;
using GameLogic;
using Newtonsoft.Json;
using UnityEngine;

namespace Card
{
    public class CardPrototype
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public string TextureName { get; set; }
        public string Desc { get; set; }
        public int Cost { get; set; }
        public List<CardBehavior> Behaviors;
        
        public static CardPrototype CreateFromJson(string jsonString)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new BehaviorConverter());
            
            return JsonConvert.DeserializeObject<CardPrototype>(jsonString, settings);
        }

        public PlayerClass ParseCardClass()
        {
            return this.Class switch
            {
                "ru" => PlayerClass.RU,
                "generic" or _ => PlayerClass.GENERIC
            };
        }
    }
}