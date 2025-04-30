using Card;
using Fighting;
using GameLogic;

namespace Entity
{
    public abstract class EntityBase
    {
        public int HP;
        public int MaxHP;
        public string Name = "";
        public string TextureName = "";

        public EntityFacing Facing = EntityFacing.DEFAULT;

        public EntityBase(int hp)
        {
            this.HP = this.MaxHP = hp;
        }

        public abstract void Hurt(EntityBase source, int value, BattleField battleField);
    }
}