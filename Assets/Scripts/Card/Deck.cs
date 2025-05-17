using System.Collections.Generic;
using GameLogic;
using Registry.Data;

namespace Card
{
    public class Deck
    {
        public readonly PlayerClass PlayerClass;
        public readonly List<CardInstance> CardList = new();

        public Deck(PlayerClass playerClass)
        {
            this.PlayerClass = playerClass;
        }

        public void AddPrototype(CardPrototype prototype)
        {
            this.CardList.Add(new CardInstance(prototype));
        }

        public static Deck FromFile()
        {
            // TODO parse json
            return new Deck(PlayerClass.RU);
        }

        public static void ToFile()
        {
            // TODO parse json
        }
    }
}