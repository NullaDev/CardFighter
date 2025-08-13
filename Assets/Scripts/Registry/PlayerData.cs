using System;
using System.Collections.Generic;
using Card;
using GameLogic.Option;
using GameLogic.RogueMap;
using Item;
using JetBrains.Annotations;
using Registry.Data;

namespace Registry
{
    // This script manages the player's "in-game" information, such as deck, hp and gold.
    public class PlayerData
    {
        public static readonly PlayerData Instance = new();

        public PlayerClass PlayerClass { get; set; } = PlayerClass.GENERIC;
        public int Hp { get; set; } = 20;
        public int MaxHp { get; set; } = 20;
        public int InitialInGameCost { get; set; } = 1;
        public int MaxInGameCost { get; set; } = 5;
        public int MaxCarryCost { get; set; } = 10;
        public int InGameGold { get; set; } = 0;

        public readonly Dictionary<CardPrototype, int> HeldCards = new();
        public readonly CardOperationsInBattle CardOperations = new();
        public readonly List<HeldItem> HeldItems = new();
        public readonly Random Random = new();

        public NodeType CurrentNodeType = NodeType.FIGHT;
        public int CurrentLayerDifficulty = 0;
        [CanBeNull] public StageConfig CurrentStage = null;
        [CanBeNull] public OptionBundle OptionBundle = null;
        
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
