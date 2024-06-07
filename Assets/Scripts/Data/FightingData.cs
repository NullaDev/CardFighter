using System.Collections.Generic;
using Card;
using UnityEngine;

namespace Data
{
    public class FightingData : MonoBehaviour
    {
        public List<CardInstance> CurrentDeck;

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
