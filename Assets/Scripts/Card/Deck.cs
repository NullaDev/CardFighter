using System.Collections.Generic;
using Data;

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

        public static Deck FromFile()
        {
            // TODO parse json
            return new Deck(PlayerClass.FIGHTER);
        }

        public static void ToFile()
        {
            // TODO parse json
        }
    }
}