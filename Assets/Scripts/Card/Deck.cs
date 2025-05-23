using System.Collections.Generic;
using Registry;
using Registry.Data;

namespace Card
{
    public class Deck
    {
        public readonly PlayerClass PlayerClass;
        public readonly List<CardPrototype> CardList = new();

        public Deck(PlayerClass playerClass)
        {
            this.PlayerClass = playerClass;
        }

        public void AddPrototype(CardPrototype prototype)
        {
            this.CardList.Add(prototype);
        }

        public static Deck FromFile()
        {
            // TODO parse json
            return new Deck(PlayerClass.GENERIC);
        }

        public static void ToFile()
        {
            // TODO parse json
        }
    }
}