using Entity;

namespace GameLogic
{
    public class EnemyConfig
    {
        public int AppearTurn { get; set; }
        public int AppearPos { get; set; }
        public string Name { get; set; }
        public string TextureName { get; set; }
        public int HP { get; set; }

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