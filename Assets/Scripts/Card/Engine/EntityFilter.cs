using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic;
using GameLogic.Entity;
using GameLogic.SceneControl;

namespace Card.Engine
{
    public abstract class EntityFilter
    {
        public abstract List<EntityBase> Apply(List<EntityBase> targets, EntityBase self, FightingControl fc);
    }
    
    public class FirstNFilter : EntityFilter
    {
        public int Value { get; set; } = 1;

        public override List<EntityBase> Apply(List<EntityBase> targets, EntityBase self, FightingControl fc)
        {
            return targets.Take(Value).ToList();
        }
    }
    
    public class LastNFilter : EntityFilter
    {
        public int Value { get; set; } = 1;

        public override List<EntityBase> Apply(List<EntityBase> targets, EntityBase self, FightingControl fc)
        {
            return targets.Skip(Math.Max(0, targets.Count - Value)).ToList();
        }
    }

    public class ExcludeSelfFilter : EntityFilter
    {
        public override List<EntityBase> Apply(List<EntityBase> targets, EntityBase self, FightingControl fc)
        {
            return targets.Where(e => e != self).ToList();
        }
    }

    public class IsAliveFilter : EntityFilter
    {
        public override List<EntityBase> Apply(List<EntityBase> targets, EntityBase self, FightingControl fc)
        {
            return targets.Where(e => !e.IsDead).ToList();
        }
    }
    
    public class HealthFilter : EntityFilter
    {
        public RelationalOperator Operator { get; set; }
        public int Value { get; set; }

        public override List<EntityBase> Apply(List<EntityBase> targets, EntityBase self, FightingControl fc)
        {
            return targets.Where(e => OperatorUtils.Compare(e.HP, Operator, Value)).ToList();
        }
    }
    
    public class TypeFilter : EntityFilter
    {
        public List<string> MatchTypes { get; set; } = new();
        public bool Not { get; set; } = false;

        public override List<EntityBase> Apply(List<EntityBase> targets, EntityBase self, FightingControl fc)
        {
            return targets.Where(e =>
            {
                var match = MatchTypes.Any(type =>
                    type switch
                    {
                        "enemy" => e is Enemy,
                        "elite_enemy" => e is EliteEnemy,
                        "player" => e is Player,
                        "passive" => e is PassiveEntity,
                        _ => false
                    });
                return Not ? !match : match;
            }).ToList();
        }
    }
    
    public class ConditionFilter : EntityFilter
    {
        public string ConditionName { get; set; }
        public bool Not { get; set; } = false;

        public override List<EntityBase> Apply(List<EntityBase> targets, EntityBase self, FightingControl fc)
        {
            return targets
                .Where(t =>
                {
                    var ok = Condition.CheckCondition(ConditionName, self, t, fc.BattleField);
                    return Not ? !ok : ok;
                }).ToList();
        }
    }
    
    public class HasBuffFilter : EntityFilter
    {
        public string BuffName { get; set; }
        public bool HasBuff { get; set; } = true;

        public override List<EntityBase> Apply(List<EntityBase> targets, EntityBase self, FightingControl fc)
        {
            return targets.Where(e =>
            {
                var has = e.Buffs.Any(b => b.Name == BuffName);
                return HasBuff ? has : !has;
            }).ToList();
        }
    }
    
    public class NameFilter : EntityFilter
    {
        public List<string> MatchNames { get; set; } = new();
        public bool Not { get; set; } = false;

        public override List<EntityBase> Apply(List<EntityBase> targets, EntityBase self, FightingControl fc)
        {
            return targets.Where(e =>
            {
                var match = MatchNames.Contains(e.TextureName);
                return Not ? !match : match;
            }).ToList();
        }
    }

}