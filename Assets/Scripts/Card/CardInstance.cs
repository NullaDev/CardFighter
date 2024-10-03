using System;
using System.Collections.Generic;
using Entity;
using Fighting;
using UnityEngine;

namespace Card
{
    public class CardInstance
    {
        public readonly CardPrototype Prototype;
        public readonly List<Action<FightingControl, EntityBase>> Effects = new();
        
        public int CurrentCost;
        public List<Action<CardInstance>> Buffs = new();

        public CardInstance(CardPrototype prototype)
        {
            this.Prototype = prototype;
            this.CurrentCost = this.Prototype.Cost;
            prototype.Behaviors.ForEach(b=>this.Effects.Add(b.Execute()));
        }
    }
}