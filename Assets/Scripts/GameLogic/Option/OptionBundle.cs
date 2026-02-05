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
                .OrderBy(_ => MiscData.Instance.GlobalRandom.Next())
                .Take(remain)
                .ToList();

            var optionList = guaranteed.Concat(optional).ToList();
            if (optionList.Count == 0)
            {
                optionList.Add(new Option()
                {
                    Id = "empty",
                    Title = "没有合适选项",
                    Description = "所以只能什么都不做",
                    Conditions = new List<OptionCondition>(),
                    Actions = new List<OptionAction>(),
                    TargetSceneName = "RogueMap"
                });
            }
            return optionList;
        }
    }
}