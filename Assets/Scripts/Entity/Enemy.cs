using System.Collections.Generic;
using Card;
using Fighting;

namespace Entity
{
    public abstract class Enemy: EntityBase
    {
        public CardInstance NextTurnCard;
        public Enemy(int hp) : base(hp)
        {
        }

        public override void Hurt(EntityBase source, int value, Map map)
        {
            this.HP -= value;
            if (this.HP <= 0)
            {
                map.RemoveEntityFromMap(this);
            }
        }

        public abstract void ThinkingNextTurnCard(Map map);
    }

    public class SimpleEnemy : Enemy
    {
        public CardInstance HeldCard;
        public SimpleEnemy(int hp) : base(hp)
        {
        }

        public override void ThinkingNextTurnCard(Map map)
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class EliteEnemy : Enemy
    {
        public List<CardInstance> HeldCards;
        public EliteEnemy(int hp) : base(hp)
        {
        }

        public override void ThinkingNextTurnCard(Map map)
        {
            throw new System.NotImplementedException();
        }
    }
}