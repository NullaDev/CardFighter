using Newtonsoft.Json;

namespace Registry.Data
{
    public class BuffInfo
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string TextureName { get; set; }
        public bool Positive { get; set; }
        public string EffectText { get; set; }
        public string ExtraText { get; set; }
        
        public static BuffInfo CreateFromJson(string jsonString)
        {
            return JsonConvert.DeserializeObject<BuffInfo>(jsonString);
        }
    }
}