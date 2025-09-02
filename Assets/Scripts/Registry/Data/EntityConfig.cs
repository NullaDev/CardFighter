using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic;
using GameLogic.Entity;

namespace Registry.Data
{
    public interface IFacingConfig
    {
        string AppearFacing { get; set; }

        static EntityFacing ParseFacing(string facing)
        {
            return facing switch
            {
                "right" => EntityFacing.Right,
                "left" => EntityFacing.Left,
                "auto" => EntityFacing.Default,
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
        public int TurnsPerAction { get; set; } = -1;
        public int InitialActionTick { get; set; } = 0;

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
                Facing = EntityFacing.Default
            };
        }
    }
    
    public class SimpleEnemyConfig : EntityConfig, IFacingConfig
    {
        public string AppearFacing { get; set; } = "auto";
        public string Card { get; set; }

        public override EntityBase GenEntity()
        {
            return new SimpleEnemy(Hp)
            {
                Name = Name,
                TextureName = "Arts/Entities/" + TextureName,
                Facing = IFacingConfig.ParseFacing(this.AppearFacing),
                HeldCard = StaticDataManager.CardDataManager.Find(Card),
                TurnsPerAction = this.TurnsPerAction,
                ActionTick = this.InitialActionTick
            };
        }
    }
    
    public class StationaryEnemyConfig : EntityConfig, IFacingConfig
    {
        public string AppearFacing { get; set; } = "auto";
        public string Card { get; set; }

        public override EntityBase GenEntity()
        {
            return new StationaryEnemy(Hp)
            {
                Name = Name,
                TextureName = "Arts/Entities/" + TextureName,
                Facing = IFacingConfig.ParseFacing(this.AppearFacing),
                HeldCard = StaticDataManager.CardDataManager.Find(Card),
                TurnsPerAction = this.TurnsPerAction,
                ActionTick = this.InitialActionTick
            };
        }
    }
    
    public class StationaryBuffEnemyConfig : EntityConfig, IFacingConfig
    {
        public string AppearFacing { get; set; } = "auto";
        public string Card { get; set; }

        public override EntityBase GenEntity()
        {
            return new StationaryBuffEnemy(Hp)
            {
                Name = Name,
                TextureName = "Arts/Entities/" + TextureName,
                Facing = IFacingConfig.ParseFacing(this.AppearFacing),
                HeldCard = StaticDataManager.CardDataManager.Find(Card),
                TurnsPerAction = this.TurnsPerAction,
                ActionTick = this.InitialActionTick
            };
        }
    }
    
    public class FixedCardEnemyConfig : EntityConfig, IFacingConfig
    {
        public string AppearFacing { get; set; } = "auto";
        public string Card { get; set; }

        public override EntityBase GenEntity()
        {
            return new FixedCardEnemy(Hp)
            {
                Name = Name,
                TextureName = "Arts/Entities/" + TextureName,
                Facing = IFacingConfig.ParseFacing(this.AppearFacing),
                HeldCard = StaticDataManager.CardDataManager.Find(Card),
                TurnsPerAction = this.TurnsPerAction,
                ActionTick = this.InitialActionTick
            };
        }
    }

    public class EliteEntityConfig : EntityConfig, IFacingConfig
    {
        public string AppearFacing { get; set; } = "auto";
        public List<string> Cards { get; set; } = new();

        public override EntityBase GenEntity()
        {
            return new EliteEnemy(Hp)
            {
                Name = Name,
                TextureName = "Arts/Entities/" + TextureName,
                Facing = IFacingConfig.ParseFacing(this.AppearFacing),
                HeldCards = Cards.Select(card => StaticDataManager.CardDataManager.Find(card)).ToList(),
                TurnsPerAction = this.TurnsPerAction,
                ActionTick = this.InitialActionTick
            };
        }
    }
}