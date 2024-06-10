using System.Collections.Generic;
using Card;
using UnityEngine;

namespace Data
{
    public class FightingData : MonoBehaviour
    {
        public int CurrentCost;
        public int MaxCost;
        public List<CardInstance> CurrentDeck;

        public FightingData(PlayerData playerData)
        {
            CurrentDeck = playerData.DefaultDeck.CardList;
            MaxCost = playerData.MaxCost;
            CurrentCost = 1;
        }

        void Start()
        {
            
        }

        void Update()
        {
        
        }
    }
}
