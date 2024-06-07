using Card;
using Fighting;

namespace Entity
{
    public class Player: EntityBase
    {
        public int Cost;
        public int MaxCost;
        
        public Player(int hp, int maxCost) : base(hp)
        {
            this.MaxCost = maxCost;
            this.Cost = 1;
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