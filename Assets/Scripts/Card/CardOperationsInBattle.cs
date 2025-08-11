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
        
        public bool HasCard(CardPrototype card)
        {
            if (card == null)
                return false;
            return MoveSlot == card || TurnSlot == card || _cardList.Contains(card);
        }

        public bool AddCard(CardPrototype prototype)
        {
            if (_cardList.Contains(prototype))
                return false;
            if (prototype != null && _cardList.Count < MaxCardCount)
            {
                _cardList.Add(prototype);
                return true;
            }
            return false;
        }
        
        public bool RemoveCard(CardPrototype card)
        {
            if (card == MoveSlot || card == TurnSlot)
            {
                return false;
            }

            return _cardList.Remove(card);
        }
        
        public bool ReplaceCard(CardPrototype card, CardPrototype newCard)
        {
            if (card == MoveSlot || card == TurnSlot)
            {
                // TODO
                return false;
            }

            if (_cardList.Contains(newCard))
            {
                return false;
            }

            var index = _cardList.IndexOf(card);
            if (index >= 0)
            {
                _cardList[index] = newCard;
                return true;
            }
            return false;
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