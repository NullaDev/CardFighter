using Entity;

namespace GameLogic
{
    public class EnemyConfig
    {
        public int AppearTurn;
        public int AppearPos;
        public string Name;
        public string Img;
        public int HP;

        public Enemy ToEnemyEntity()
        {
            return new Enemy(HP);
        }
    }
}