using System;
using System.Collections.Generic;
using Card;
using Entity;
using GameLogic;
using Registry;
using Registry.Data;
using Unity.VisualScripting;

namespace Fighting
{
    public class FightingData
    {
        public int CurrentTurn;
        
        public int CurrentCost;
        public int MaxCost;
        public readonly List<CardInstance> AvailableCards = new();
        private CardPrototype _defaultMoveCard;
        private CardPrototype _defaultTurnCard;

        public static FightingData FromPlayerData(PlayerData playerData)
        {
            FightingData fightingData = new();
            fightingData.CurrentCost = playerData.InitialInGameCost;
            fightingData.MaxCost = playerData.MaxInGameCost;

            var cards = playerData.CardOperations.GetAllCards();
            fightingData._defaultMoveCard = cards[0];
            fightingData._defaultTurnCard = cards[1];
            foreach (var card in cards)
            {
                fightingData.AvailableCards.Add(new CardInstance(card));
            }
            return fightingData;
        }

        public void UpdatePlayerDeck(Player player)
        {
            for (var i = 0; i < AvailableCards.Count; i++)
            {
                var cardInstance = AvailableCards[i];
                if (player.HasBuff(EntityBuffManager.Charioteering))
                {
                    AvailableCards[i] = cardInstance.Prototype.ID switch
                    {
                        "move" => new CardInstance(CommonCards.Drive),
                        "turn_back" => new CardInstance(CommonCards.UTurn),
                        _ => AvailableCards[i]
                    };
                }
                else
                {
                    AvailableCards[i] = cardInstance.Prototype.ID switch
                    {
                        "drive" => new CardInstance(this._defaultMoveCard),
                        "u_turn" => new CardInstance(this._defaultTurnCard),
                        _ => AvailableCards[i]
                    };
                }
            }
        }

        public void TryAddCost(int value)
        {
            this.CurrentCost = Math.Min(this.CurrentCost+value, this.MaxCost);
        }

    }
}