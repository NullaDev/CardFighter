using System;
using System.Collections.Generic;
using Card;
using Entity;
using GameLogic;
using Registry;

namespace Fighting
{
    public class FightingData
    {
        public int CurrentTurn;
        
        public int CurrentCost;
        public int MaxCost;
        public readonly List<CardInstance> AvailableCards = new();

        public static FightingData FromPlayerData(PlayerData playerData)
        {
            FightingData fightingData = new();
            fightingData.CurrentCost = playerData.InitialCost;
            fightingData.MaxCost = playerData.MaxCost;
            foreach (var card in playerData.DefaultDeck.CardList)
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
                        "turn_back" => new CardInstance(CommonCards.UTurn)
                        {
                            CurrentCost = player.GetBuff(EntityBuffManager.Charioteering).GetParam<int>(EntityBuffManager.CharioteeringValue)
                        },
                        _ => AvailableCards[i]
                    };
                }
                else
                {
                    AvailableCards[i] = cardInstance.Prototype.ID switch
                    {
                        "drive" => new CardInstance(CommonCards.Move1),
                        "u_turn" => new CardInstance(CommonCards.TurnBack),
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