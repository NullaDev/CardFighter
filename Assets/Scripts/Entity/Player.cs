using Card;
using Fighting;

namespace Entity
{
    public class Player: EntityBase
    {
        public Player(int hp) : base(hp)
        {
            this.TextureName = "Arts/Entities/player";
        }

        public override void Hurt(EntityBase source, int value, Map map)
        {
            this.HP -= value;
            if (this.HP <= 0)
            {
                // TODO
            }
        }
        
    }
}