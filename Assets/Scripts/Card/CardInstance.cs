using System;
using System.Collections.Generic;
using Entity;
using Fighting;
using GameLogic;
using Registry;
using Registry.Data;
using UnityEngine;

namespace Card
{
    public class CardInstance
    {
        public readonly CardPrototype Prototype;
        public readonly List<Action<FightingControl, EntityBase>> Effects = new();

        public int GetCurrentCost(Player player)
        {
            if (player.HasBuff(EntityBuffManager.HiddenWeapon))
            {
                return 0;
            }
            if (this.Prototype == CommonCards.UTurn)
            {
                if (player.HasBuff(EntityBuffManager.Charioteering))
                {
                    return player.GetBuff(EntityBuffManager.Charioteering)
                        .GetParam<int>(EntityBuffManager.CharioteeringValue);
                }
                else
                {
                    return 0;
                }
            }
            return this.Prototype.Cost;
        }

        public CardInstance(CardPrototype prototype)
        {
            this.Prototype = prototype;
            prototype.Actions.ForEach(b=>this.Effects.Add(b.Execute));
        }
    }
    
}