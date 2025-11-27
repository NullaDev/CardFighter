using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic.Entity;
using GameLogic.Runtime;

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

        public abstract EntityBase GenEntityBasedOnHpModifier();
    }

    public class PassiveEntityConfig : EntityConfig
    {
        public override EntityBase GenEntityBasedOnHpModifier()
        {
            return new PassiveEntity((int)(Hp*MapData.Instance.CurrentMapHpModifier))
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

        public override EntityBase GenEntityBasedOnHpModifier()
        {
            return new SimpleEnemy((int)(Hp*MapData.Instance.CurrentMapHpModifier))
            {
                Name = this.Name,
                TextureName = "Arts/Entities/" + this.TextureName,
                Facing = IFacingConfig.ParseFacing(this.AppearFacing),
                HeldCard = StaticDataManager.CardDataManager.Find(this.Card),
                TurnsPerAction = this.TurnsPerAction,
                ActionTick = this.InitialActionTick
            };
        }
    }
    
    public class StationaryEnemyConfig : EntityConfig, IFacingConfig
    {
        public string AppearFacing { get; set; } = "auto";
        public string Card { get; set; }

        public override EntityBase GenEntityBasedOnHpModifier()
        {
            return new StationaryEnemy((int)(Hp*MapData.Instance.CurrentMapHpModifier))
            {
                Name = this.Name,
                TextureName = "Arts/Entities/" + this.TextureName,
                Facing = IFacingConfig.ParseFacing(this.AppearFacing),
                HeldCard = StaticDataManager.CardDataManager.Find(this.Card),
                TurnsPerAction = this.TurnsPerAction,
                ActionTick = this.InitialActionTick
            };
        }
    }
    
    public class StationaryBuffEnemyConfig : EntityConfig, IFacingConfig
    {
        public string AppearFacing { get; set; } = "auto";
        public string Card { get; set; }

        public override EntityBase GenEntityBasedOnHpModifier()
        {
            return new StationaryBuffEnemy((int)(Hp*MapData.Instance.CurrentMapHpModifier))
            {
                Name = this.Name,
                TextureName = "Arts/Entities/" + this.TextureName,
                Facing = IFacingConfig.ParseFacing(this.AppearFacing),
                HeldCard = StaticDataManager.CardDataManager.Find(this.Card),
                TurnsPerAction = this.TurnsPerAction,
                ActionTick = this.InitialActionTick
            };
        }
    }
    
    public class FixedCardEnemyConfig : EntityConfig, IFacingConfig
    {
        public string AppearFacing { get; set; } = "auto";
        public string Card { get; set; }

        public override EntityBase GenEntityBasedOnHpModifier()
        {
            return new FixedCardEnemy((int)(Hp*MapData.Instance.CurrentMapHpModifier))
            {
                Name = this.Name,
                TextureName = "Arts/Entities/" + this.TextureName,
                Facing = IFacingConfig.ParseFacing(this.AppearFacing),
                HeldCard = StaticDataManager.CardDataManager.Find(this.Card),
                TurnsPerAction = this.TurnsPerAction,
                ActionTick = this.InitialActionTick
            };
        }
    }

    public class EliteEntityConfig : EntityConfig, IFacingConfig
    {
        public string AppearFacing { get; set; } = "auto";
        public List<string> Cards { get; set; } = new();

        public override EntityBase GenEntityBasedOnHpModifier()
        {
            return new EliteEnemy((int)(Hp*MapData.Instance.CurrentMapHpModifier))
            {
                Name = this.Name,
                TextureName = "Arts/Entities/" + this.TextureName,
                Facing = IFacingConfig.ParseFacing(this.AppearFacing),
                HeldCards = this.Cards.Select(card => StaticDataManager.CardDataManager.Find(card)).ToList(),
                TurnsPerAction = this.TurnsPerAction,
                ActionTick = this.InitialActionTick
            };
        }
    }
}