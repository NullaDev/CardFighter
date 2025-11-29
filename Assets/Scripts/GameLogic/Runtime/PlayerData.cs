using System;
using System.Collections.Generic;
using Card;
using Item;
using Registry;
using Registry.Data;

namespace GameLogic.Runtime
{
    // This script manages the player's "in-game" information, such as deck, hp and gold.
    public class PlayerData
    {
        public static readonly PlayerData Instance = new();

        public PlayerClass PlayerClass { get; set; } = PlayerClass.GENERIC;
        public int Hp { get; set; }
        public int MaxHp { get; set; }
        public int InitialInGameCost { get; set; }
        public int MaxInGameCost { get; set; }
        public int MaxCarryCost { get; set; }
        public int InGameGold { get; set; }

        public readonly Dictionary<CardPrototype, int> HeldCards = new();
        public readonly CardOperationsInBattle CardOperations = new();
        public readonly List<HeldItem> HeldItems = new();
        
        public void InitFromConfig(PlayerClass playerClass, PlayerClassConfig config)
        {
            this.PlayerClass = playerClass;
            this.MaxHp = this.Hp = config.MaxHP;
            this.InitialInGameCost = config.InitialInGameCost;
            this.MaxInGameCost = config.MaxInGameCost;
            this.MaxCarryCost = config.MaxCarryCost;
            this.InGameGold = this.InitialInGameCost;
        }
        
        public void InitCardOperationsFromHeld()
        {
            CardOperations.Clear();
            
            if (HeldCards.ContainsKey(CommonCards.Move1))
                CardOperations.SetMoveSlot(CommonCards.Move1);
            if (HeldCards.ContainsKey(CommonCards.TurnBack))
                CardOperations.SetTurnSlot(CommonCards.TurnBack);

            foreach (var card in HeldCards.Keys)
            {
                if (card == CommonCards.Move1 || card == CommonCards.TurnBack)
                    continue;
                if (CardOperations.GetAllCards().Count >= 2 + CardOperationsInBattle.MaxCardCount)
                    break;
                CardOperations.AddCard(card);
            }
        }

        public void UpdateHp()
        {
            this.Hp = Math.Min(this.Hp, this.MaxHp);
            if (this.Hp <= 0)
            {
                // TODO
            }
        }
    }
}
