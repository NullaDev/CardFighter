using Card;
using FightingControl;

namespace Entity
{
    public class Player: EntityBase
    {
        public Player(int hp) : base(hp)
        {
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