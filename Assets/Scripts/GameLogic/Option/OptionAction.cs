using System;
using System.Collections.Generic;
using System.Linq;
using Registry;
using Registry.Data;

namespace GameLogic.Option
{
    public abstract class OptionAction
    {
        public string Type { get; set; }
    
        public abstract void Execute(PlayerData player);
    }
    
    public class GoldChangeAction : OptionAction
    {
        public ArithmeticOperator ArithmeticOperator { get; set; }
        public float Amount { get; set; }
    
        public override void Execute(PlayerData player)
        {
            player.InGameGold = (int)OperatorUtils.ApplyOperator(player.InGameGold, ArithmeticOperator, Amount);
            player.InGameGold = Math.Max(player.InGameGold, 0);
        }
    }
    
    public class HpChangeAction : OptionAction
    {
        public ArithmeticOperator ArithmeticOperator { get; set; }
        public float Amount { get; set; }
    
        public override void Execute(PlayerData player)
        {
            player.Hp = (int)OperatorUtils.ApplyOperator(player.Hp, ArithmeticOperator, Amount);
            player.UpdateHp();
        }
    }
    
    public class HpRestoreAction : OptionAction
    {
        public ArithmeticOperator ArithmeticOperator { get; set; }
        public float Amount { get; set; }
    
        public override void Execute(PlayerData player)
        {
            player.Hp += ArithmeticOperator switch
            {
                ArithmeticOperator.Add => (int)Amount,
                ArithmeticOperator.Multiply or ArithmeticOperator.Divide =>
                    (int)OperatorUtils.ApplyOperator(player.MaxHp, ArithmeticOperator, Amount),
                _ => throw new Exception("Inappropriate operator")
            };
            player.UpdateHp();
        }
    }
    
    public class MaxHpChangeAction  : OptionAction
    {
        public ArithmeticOperator ArithmeticOperator { get; set; }
        public float Amount { get; set; }
    
        public override void Execute(PlayerData player)
        {
            player.MaxHp = (int)OperatorUtils.ApplyOperator(player.MaxHp, ArithmeticOperator, Amount);
            player.UpdateHp();
        }
    }
    
    public class CardGainAction : OptionAction
    {
        public List<string> CardIDs { get; set; }
        public int Count { get; set; }
        public bool Random { get; set; }

        public override void Execute(PlayerData player)
        {
            var gained = new List<CardPrototype>();

            for (var i = 0; i < Count; i++)
            {
                var cardId = Random ? CardIDs[player.Random.Next(CardIDs.Count)] : CardIDs[i % CardIDs.Count];
                gained.Add(StaticDataManager.CardDataManager.Find(cardId));
            }

            foreach (var card in gained.Where(card => !player.HeldCards.TryAdd(card, 1)))
            {
                player.HeldCards[card]++;
            }
        }
    }
    
    public class ItemGainAction : OptionAction
    {
        public List<string> ItemIDs { get; set; } = new();
        public int Count { get; set; }
        public bool Random { get; set; }

        public override void Execute(PlayerData player)
        {
            if (ItemIDs.Count == 0)
                return;
            for (var i = 0; i < Count; i++)
            {
                var itemId = Random ? ItemIDs[player.Random.Next(ItemIDs.Count)] : ItemIDs[i % ItemIDs.Count];
                var item = StaticDataManager.HeldItemDataManager.Find(itemId);
                item?.PlayerTryObtain(player);
            }
        }
    }
    
    public class CardRandomLoseAction : OptionAction
    {
        public int Count { get; set; }

        public override void Execute(PlayerData player)
        {
            var flatList = player.HeldCards
                .Where(kv => !kv.Key.IsBuiltinCard)
                .SelectMany(kv => Enumerable.Repeat(kv.Key, kv.Value))
                .ToList();

            if (flatList.Count <= 0) return;

            for (var i = 0; i < Count && flatList.Count > 0; i++)
            {
                var index = player.Random.Next(flatList.Count);
                var card = flatList[index];
                flatList.RemoveAt(index);

                if (player.HeldCards.ContainsKey(card))
                {
                    player.HeldCards[card]--;
                    if (player.HeldCards[card] <= 0)
                    {
                        player.HeldCards.Remove(card);
                    }
                }
            }

            player.CardOperations.Clear();
        }
    }
    
    public class ItemRandomLoseAction : OptionAction
    {
        public int Count { get; set; }

        public override void Execute(PlayerData player)
        {
            for (var i = 0; i < Count && player.HeldItems.Count > 0; i++)
            {
                var index = player.Random.Next(player.HeldItems.Count);
                player.HeldItems.RemoveAt(index);
            }
        }
    }
}