using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Entity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameLogic
{
    public abstract class EntityConfig
    {
        public string Type { get; set; }
        public int AppearTurn { get; set; }
        public int AppearPos { get; set; }
        public string Name { get; set; }
        public string TextureName { get; set; }
        public int Hp { get; set; }

        public abstract EntityBase GenEntity();
    }

    public class PassiveEntityConfig : EntityConfig
    {
        public override EntityBase GenEntity()
        {
            return new PassiveEntity(Hp)
            {
                Name = Name,
                TextureName = "Arts/Entities/" + TextureName,
                Facing = EntityFacing.DEFAULT
            };
        }
    }
    
    public class SimpleEnemyConfig : EntityConfig
    {
        public string AppearFacing { get; set; } = "auto";
        public string Card { get; set; }

        public override EntityBase GenEntity()
        {
            return new SimpleEnemy(Hp)
            {
                Name = Name,
                TextureName = "Arts/Entities/" + TextureName,
                Facing = AppearFacing switch
                {
                    "right" => EntityFacing.RIGHT,
                    "left" => EntityFacing.LEFT,
                    "random" => Random.Range(0, 2) == 0 ? EntityFacing.RIGHT : EntityFacing.LEFT,
                    "auto" => EntityFacing.DEFAULT,
                    _ => throw new Exception("Unknown direction")
                },
                HeldCard = CardData.Instance.Find(Card)
            };
        }
    }

    public class EliteEntityConfig : EntityConfig
    {
        public string AppearFacing { get; set; } = "auto";
        public List<string> Cards { get; set; }

        public override EntityBase GenEntity()
        {
            return new EliteEnemy(Hp)
            {
                Name = Name,
                TextureName = "Arts/Entities/" + TextureName,
                Facing = AppearFacing switch
                {
                    "right" => EntityFacing.RIGHT,
                    "left" => EntityFacing.LEFT,
                    "random" => Random.Range(0, 2) == 0 ? EntityFacing.RIGHT : EntityFacing.LEFT,
                    "auto" => EntityFacing.DEFAULT,
                    _ => throw new Exception("Unknown direction")
                },
                HeldCards = Cards.Select(card => CardData.Instance.Find(card)).ToList()
            };
        }
    }
}