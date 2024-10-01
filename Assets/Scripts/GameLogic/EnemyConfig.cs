using System;
using Entity;
using Random = UnityEngine.Random;

namespace GameLogic
{
    public class EnemyConfig
    {
        public int AppearTurn { get; set; }
        public int AppearPos { get; set; }
        public string AppearFacing { get; set; }
        public string Name { get; set; }
        public string TextureName { get; set; }
        public int HP { get; set; }

        public Enemy ToEnemyEntity()
        {
            return new Enemy(HP)
            {
                Name = Name,
                TextureName = "Arts/Entities/" + TextureName,
                Facing = AppearFacing switch
                {
                    "right" => EntityFacing.RIGHT,
                    "left" => EntityFacing.LEFT,
                    "random" => Random.Range(0, 2) == 0 ? EntityFacing.RIGHT : EntityFacing.LEFT,
                    _ => throw new Exception("Unknown direction")
                }
            };
        }
    }
}