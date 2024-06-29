using Entity;

namespace GameLogic
{
    public class EnemyConfig
    {
        public int AppearTurn;
        public int AppearPos;
        public string Name;
        public string TextureName;
        public int HP;

        public Enemy ToEnemyEntity()
        {
            return new Enemy(HP)
            {
                Name = Name,
                TextureName = TextureName
            };
        }
    }
}