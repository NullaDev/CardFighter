using System.Collections.Generic;

namespace GameLogic.SaveLoad
{
    public class PlayerSaveData
    {
        public string PlayerClass;
        public int Hp;
        public int MaxHp;
        public int InitialInGameCost;
        public int MaxInGameCost;
        public int MaxCarryCost;
        public int InGameGold;

        public Dictionary<string, int> HeldCards;
        public List<string> CardOperations = new();
        public List<string> HeldItems;
    }
}