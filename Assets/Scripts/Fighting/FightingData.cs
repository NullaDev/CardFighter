using System;
using System.Collections.Generic;
using Card;
using Data;

namespace Fighting
{
    public class FightingData
    {
        public int CurrentTurn;
        
        public int CurrentCost;
        public int MaxCost;
        public List<CardInstance> CurrentDeck;

        public static FightingData FromPlayerData(PlayerData playerData)
        {
            FightingData fightingData = new();
            fightingData.CurrentCost = playerData.InitialCost;
            fightingData.MaxCost = playerData.MaxCost;
            fightingData.CurrentDeck = playerData.DefaultDeck.CardList;
            return fightingData;
        }

        public void AddCost(int value)
        {
            this.CurrentCost = Math.Min(this.CurrentCost+value, this.MaxCost);
        }

    }
}