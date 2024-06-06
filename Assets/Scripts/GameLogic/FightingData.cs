using System.Collections.Generic;
using Card;
using UnityEngine;

namespace GameLogic
{
    public class FightingData : MonoBehaviour
    {
        public List<CardInstance> CurrentDeck = new();

        public FightingData(PlayerData playerData)
        {
            this.CurrentDeck = playerData.DefaultDeck.CardList;
        }

        void Start()
        {
            
        }

        void Update()
        {
        
        }
    }
}
