using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Entity;
using UnityEngine;

namespace GameLogic
{
    public interface IHasFacing
    {
        string AppearFacing { get; set; }

        static EntityFacing ParseFacing(string facing)
        {
            return facing switch
            {
                "right" => EntityFacing.RIGHT,
                "left" => EntityFacing.LEFT,
                "auto" => EntityFacing.DEFAULT,
                _ => throw new Exception("Unknown direction")
            };
        }
    }
    
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
    
    public class SimpleEnemyConfig : EntityConfig, IHasFacing
    {
        public string AppearFacing { get; set; } = "auto";
        public string Card { get; set; }

        public override EntityBase GenEntity()
        {
            return new SimpleEnemy(Hp)
            {
                Name = Name,
                TextureName = "Arts/Entities/" + TextureName,
                Facing = IHasFacing.ParseFacing(this.AppearFacing),
                HeldCard = CardData.Instance.Find(Card)
            };
        }
    }

    public class EliteEntityConfig : EntityConfig, IHasFacing
    {
        public string AppearFacing { get; set; } = "auto";
        public List<string> Cards { get; set; }

        public override EntityBase GenEntity()
        {
            return new EliteEnemy(Hp)
            {
                Name = Name,
                TextureName = "Arts/Entities/" + TextureName,
                Facing = IHasFacing.ParseFacing(this.AppearFacing),
                HeldCards = Cards.Select(card => CardData.Instance.Find(card)).ToList()
            };
        }
    }
}