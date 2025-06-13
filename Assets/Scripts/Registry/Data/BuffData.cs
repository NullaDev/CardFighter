using System.Collections.Generic;
using GameLogic.Buff;

namespace Registry.Data
{
    public class BuffData
    {
        public string BuffName { get; set; }
        public int Turn { get; set; }
        public List<BuffEffectRule> Rules { get; set; } = new();
    }
}