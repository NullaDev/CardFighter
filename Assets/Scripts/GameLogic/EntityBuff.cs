using System.Collections.Generic;

namespace GameLogic
{
    public class EntityBuff
    {
        public string Name { get; set; }
        public int Duration { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();

        public EntityBuff(string name, int duration)
        {
            this.Name = name;
            this.Duration = duration;
        }
        
        public EntityBuff SetParam(string key, object value)
        {
            this.Parameters[key] = value;
            return this;
        }
        
        public T GetParam<T>(string key, T defaultValue = default)
        {
            if (Parameters.TryGetValue(key, out var value) && value is T tValue)
            {
                return tValue;
            }
            return defaultValue;
        }
    }
}