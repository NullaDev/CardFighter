using System;
using System.Collections.Generic;
using Fighting;

namespace Card
{
    public class CardInstance
    {
        private readonly CardPrototype _prototype;
        public readonly List<Action<Map>> Effects = new();
        
        public int CurrentCost;
        public List<Action<CardInstance>> Buffs = new();

        public CardInstance(CardPrototype prototype)
        {
            CardPrototype prototype1;
            this._prototype = prototype;
            this.CurrentCost = this._prototype.Cost;
            prototype.Behaviors.ForEach(b=>this.Effects.Add(b.Execute()));
        }
    }
}