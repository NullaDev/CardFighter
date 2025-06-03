using System.Collections.Generic;

namespace Registry.Data
{
    public class BuffData
    {
        public string BuffName { get; set; }
        public int Turn { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();
    }
}