using System.Collections.Generic;
using Registry;
using Registry.Data;

namespace Card
{
    public class CardOperationsInBattle
    {
        public CardPrototype MoveSlot { get; private set; } = CommonCards.Move1;
        public CardPrototype TurnSlot { get; private set; } = CommonCards.TurnBack;

        private readonly List<CardPrototype> _cardList = new();
        public const int MaxCardCount = 6;
        
        public void SetMoveSlot(CardPrototype prototype)
        {
            if (prototype != null)
            {
                MoveSlot = prototype;
            }
        }

        public void SetTurnSlot(CardPrototype prototype)
        {
            if (prototype != null)
            {
                TurnSlot = prototype;
            }
        }
        
        public void Clear()
        {
            MoveSlot = CommonCards.Move1;
            TurnSlot = CommonCards.TurnBack;
            _cardList.Clear();
        }

        public void AddPrototype(CardPrototype prototype)
        {
            if (prototype != null && _cardList.Count < MaxCardCount)
            {
                _cardList.Add(prototype);
            }
        }
        
        public List<CardPrototype> GetAllCards()
        {
            var result = new List<CardPrototype>();
            result.Add(MoveSlot ?? CommonCards.Move1);
            result.Add(TurnSlot ?? CommonCards.TurnBack);
            result.AddRange(_cardList);
            return result;
        }

        public static CardOperationsInBattle FromFile()
        {
            // TODO: load from json
            return new CardOperationsInBattle();
        }

        public static void ToFile()
        {
            // TODO: save to json
        }
    }
}