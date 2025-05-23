using System.Collections.Generic;
using Card;
using Registry.Data;

namespace Registry
{
    // This script manages the player's "in-game" information, such as deck, hp and gold.
    public class PlayerData
    {
        public static readonly PlayerData Instance = new();

        public PlayerClass PlayerClass { get; set; } = PlayerClass.GENERIC;
        public int Hp { get; set; } = 10;
        public int MaxHp { get; set; } = 10;
        public int InitialInGameCost { get; set; } = 1;
        public int MaxInGameCost { get; set; } = 5;
        public int MaxCarryCost { get; set; } = 15;
        public int InGameGold { get; set; } = 0;

        public readonly List<CardPrototype> ListCard = new();
        public Deck DefaultDeck;

        public StageConfig CurrentStage = null;
    }
}
