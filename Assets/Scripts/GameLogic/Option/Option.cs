using System.Collections.Generic;
using System.Linq;
using Registry;

namespace GameLogic.Option
{
    public class Option
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<OptionCondition> Conditions { get; set; } = new();
        public string TargetSceneName { get; set; } = "RogueMap";
        public List<OptionAction> Actions { get; set; }
        
        public bool Passes(PlayerData playerData) =>
            Conditions == null || Conditions.Count == 0 || Conditions.All(c => c.IsMet(playerData));
    }
}