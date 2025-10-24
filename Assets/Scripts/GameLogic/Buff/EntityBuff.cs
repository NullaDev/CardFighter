using System;
using System.Collections.Generic;
using System.Linq;
using Registry.Data;
using UnityEngine;

namespace GameLogic.Buff
{
    public class EntityBuff
    {
        public string Name { get; set; }
        public int Duration { get; set; }
        public EntityBuffManager.BuffType BuffType { get; set; }
        public List<BuffEffectRule> EffectRules { get; set; } = new();

        public EntityBuff(BuffData data)
        {
            this.Name = data.BuffName;
            this.Duration = data.Turn;
            this.BuffType = EntityBuffManager.FromString(data.BuffType);
            this.EffectRules = data.Rules?.Select(r => r.Clone()).ToList() ?? new List<BuffEffectRule>();
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