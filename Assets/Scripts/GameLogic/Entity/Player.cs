using GameLogic.Runtime;

namespace GameLogic.Entity
{
    public class Player: EntityBase
    {
        public override bool HasValidFacing => true;

        public Player(int hp, int maxHp) : base(maxHp)
        {
            this.HP = hp;
            this.TextureName = "Arts/Entities/player";
            this.Name = "你";
        }

        public override void Hurt(EntityBase source, int value, BattleField battleField)
        {
            var mapData = MapData.Instance;
            this.HP -= (int)(value * mapData.CurrentMapAttackModifier);
            if (this.HP <= 0)
            {
                // TODO Lose
            }
        }
        
    }
}