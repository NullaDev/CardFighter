using System;
using Card;
using FightingControl;

namespace Entity
{
    public class Player: EntityBase
    {
        private int _coin;  //持有金币数
        public event Action<int> OnCoinCountChanged;
        public Player(int hp, int initCoins = 0) : base(hp)
        {
            _coin = initCoins;
        }

        public int GetCurrentCoins()
        {
            return _coin;
        }

        public void AddCoins(int coinNumber)
        {
            _coin += coinNumber;
            OnCoinCountChanged?.Invoke(_coin);
        }

        public bool LoseCoins(int coinNumber)
        {
            if (_coin >= coinNumber)
            {
                _coin -= coinNumber;
                OnCoinCountChanged?.Invoke(_coin);
            }
            else  //TODO 等待应用场景
            {
                return false;
            }

            return true;
        }

        public override void Hurt(EntityBase source, int value, Map map)
        {
            this.HP -= value;
            if (this.HP <= 0)
            {
                // TODO
            }
        }

        public override void UseCard(CardInstance card, Map map)
        {
            // TODO
        }
    }
}