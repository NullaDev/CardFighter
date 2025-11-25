using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic.Runtime;
using Registry;

namespace GameLogic.Option
{
    public class OptionBundle
    {
        public string Description { get; set; }

        public List<Option> GuaranteedOptions { get; set; } = new();
        public List<Option> OptionalOptions { get; set; } = new();
        
        public List<Option> GetValidOptionsAccordingToPlayer(PlayerData playerData, int maxLen = 3)
        {
            var guaranteed = GuaranteedOptions.Where(o => o.Passes(playerData)).ToList();
            var remain = Math.Max(0, maxLen - guaranteed.Count);
            var optional = OptionalOptions
                .Where(o => o.Passes(playerData))
                .OrderBy(_ => playerData.Random.Next())
                .Take(remain)
                .ToList();

            return guaranteed.Concat(optional).ToList();
        }
    }
}