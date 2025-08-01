using System.Collections.Generic;

namespace GameLogic.Option
{
    public class Option
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PlayerClass { get; set; } = "generic";

        public List<OptionAction> Actions { get; set; }
    }
}