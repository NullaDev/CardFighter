using Card;
using Fighting;

namespace Entity
{
    public class Enemy: EntityBase
    {
        public Enemy(int hp) : base(hp)
        {
        }

        public override void Hurt(EntityBase source, int value, Map map)
        {
            this.HP -= value;
            if (this.HP <= 0)
            {
                map.RemoveEntityFromMap(this);
                // TODO
            }
        }

        public override void UseCard(CardInstance card, Map map)
        {
            // TODO
        }
    }
}