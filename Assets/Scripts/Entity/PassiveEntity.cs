using Fighting;

namespace Entity
{
    public class PassiveEntity: EntityBase
    {
        public PassiveEntity(int hp) : base(hp)
        {
        }

        public override void Hurt(EntityBase source, int value, BattleField battleField)
        {
            this.HP -= value;
            if (this.HP <= 0)
            {
                this.IsDead = true;
                battleField.RemoveEntityFromMap(this);
            }
        }
        
    }
}