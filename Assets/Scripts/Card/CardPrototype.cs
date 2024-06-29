using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Card
{
    public class CardPrototype
    {
        public string ID;
        public string Name;
        public string Desc;
        public int Cost;
        public List<string> Effects;
        
        public static CardPrototype CreateFromJson(string jsonString)
        {
            return JsonConvert.DeserializeObject<CardPrototype>(jsonString);
        }
    }
}