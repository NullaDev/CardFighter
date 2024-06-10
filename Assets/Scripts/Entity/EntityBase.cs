using Card;
using FightingControl;

namespace Entity
{
    public abstract class EntityBase
    {
        public int HP;
        public int MaxHP;

        public EntityBase(int hp)
        {
            this.HP = this.MaxHP = hp;
        }

        public abstract void Hurt(EntityBase source, int value, Map map);
        
        public abstract void UseCard(CardInstance card, Map map);
    }
}