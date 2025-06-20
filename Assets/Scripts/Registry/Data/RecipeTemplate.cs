using System.Collections.Generic;

namespace Registry.Data
{
    public class RecipeTemplate
    {
        public List<string> Slot1 { get; set; }
        public List<string> Slot2 { get; set; }
        public string Result { get; set; }
        public int Priority { get; set; }
        public bool ConsumeSlot1 { get; set; } = true;
        public bool ConsumeSlot2 { get; set; } = true;
    }
}