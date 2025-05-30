using GameLogic;

namespace Entity
{
    public class Player: EntityBase
    {
        public Player(int hp, int maxHp) : base(maxHp)
        {
            this.HP = hp;
            this.TextureName = "Arts/Entities/player";
        }

        public override void Hurt(EntityBase source, int value, BattleField battleField)
        {
            this.HP -= value;
            if (this.HP <= 0)
            {
                // TODO
            }
        }
        
    }
}