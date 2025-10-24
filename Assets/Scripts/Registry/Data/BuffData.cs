using System.Collections.Generic;
using GameLogic.Buff;

namespace Registry.Data
{
    public class BuffData
    {
        public string BuffName { get; set; }
        public string BuffType { get; set; }
        public int Turn { get; set; }
        public List<string> ImmunityTo { get; set; }
        public List<string> ConflictsWith { get; set; }
        public bool IsToggle { get; set; }
        public bool IsStackable { get; set; }
        public List<string> StackableParams { get; set; }
        public List<BuffEffectRule> Rules { get; set; } = new();
    }
}