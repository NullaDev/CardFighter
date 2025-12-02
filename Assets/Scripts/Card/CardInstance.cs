using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic.Buff;
using GameLogic.Entity;
using Registry.Data;
using SceneControl;

namespace Card
{
    public class CardInstance
    {
        public readonly CardPrototype Prototype;
        public readonly List<Action<FightingControl, EntityBase>> Effects = new();

        public int GetCurrentCost(Player player)
        {
            var baseCost = this.Prototype.Cost;
            var additiveModifier = 0.0;
            var multipleModifier = 1.0;

            foreach (var buff in player.Buffs.ToList())
            {
                foreach (var rule in buff.EffectRules.ToList())
                {
                    if (rule is CardCostEffectRule costRule)
                    {
                        if (!costRule.AffectAllCards && !costRule.AffectedCardIds.Contains(this.Prototype.ID))
                            continue;
                        
                        if (costRule is IBuffFilterEffect buffFilter && !buffFilter.BuffSatisfied(player))
                            continue;

                        IOperatorEffect.ApplyBuffEffect(ref baseCost, ref additiveModifier, ref multipleModifier, costRule);
                    }
                }
            }

            var result = (int)((baseCost + additiveModifier) * multipleModifier);
            return Math.Max(0, result);
        }


        public CardInstance(CardPrototype prototype)
        {
            this.Prototype = prototype;
            prototype.Actions.ForEach(b=>this.Effects.Add(b.Execute));
        }
    }
    
}