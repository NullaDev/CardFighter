using System;
using System.Collections.Generic;

namespace Card
{
    public class CardInstance
    {
        private readonly CardPrototype prototype;
        public int currentCost;
        public List<Action<CardInstance>> buffs = new();

        public CardInstance(CardPrototype prototype)
        {
            this.prototype = prototype;
            this.currentCost = this.prototype.Cost;
        }
    }
}