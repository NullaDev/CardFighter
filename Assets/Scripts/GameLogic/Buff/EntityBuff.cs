using System;
using System.Collections.Generic;
using System.Linq;

namespace GameLogic.Buff
{
    public class EntityBuff
    {
        public string Name { get; set; }
        public int Duration { get; set; }
        public List<BuffEffectRule> EffectRules { get; set; } = new();

        public EntityBuff(string name, int duration)
        {
            this.Name = name;
            this.Duration = duration;
        }
        
        public T GetMiscParam<T>(string paramKey, T defaultValue = default)
        {
            foreach (var miscRule in this.EffectRules.OfType<MiscEffectRule>())
            {
                if (miscRule.Parameters.TryGetValue(paramKey, out var value))
                {
                    try
                    {
                        if (value is T tValue)
                            return tValue;
                        return (T)Convert.ChangeType(value, typeof(T));
                    }
                    catch
                    {
                        return defaultValue;
                    }
                }
            }

            return defaultValue;
        }
    }
}