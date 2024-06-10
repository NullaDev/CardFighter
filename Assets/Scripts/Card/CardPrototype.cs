using System.Collections.Generic;
using System.IO;
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
            Debug.Log(jsonString);
            return JsonUtility.FromJson<CardPrototype>(jsonString);
        }
    }
}