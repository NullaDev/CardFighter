using System.Collections.Generic;
using System.Linq;
using Registry;

namespace GameLogic.Option
{
    public class OptionBundle
    {
        public string Description { get; set; }

        public List<Option> GuaranteedOptions { get; set; } = new();
        public List<Option> OptionalOptions { get; set; } = new();
        
        public List<Option> GetOptions(PlayerData playerData, int maxLen = 3)
        {
            var guaranteed = GuaranteedOptions
                .Where(opt => opt.PlayerClass == "generic" || opt.PlayerClass == playerData.PlayerClass.ToString())
                .ToList();

            var optional = OptionalOptions
                .Where(opt => opt.PlayerClass == "generic" || opt.PlayerClass == playerData.PlayerClass.ToString())
                .OrderBy(_ => playerData.Random.Next())
                .Take(maxLen - guaranteed.Count)
                .ToList();

            return guaranteed.Concat(optional).ToList();
        }
    }
}