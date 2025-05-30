using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic.Entity;

namespace Card.Engine
{
    public abstract class EntityFilter
    {
        public abstract List<EntityBase> Apply(List<EntityBase> targets, EntityBase self);
    }
    
    public class FirstNFilter : EntityFilter
    {
        public int Value { get; set; } = 1;

        public override List<EntityBase> Apply(List<EntityBase> targets, EntityBase self)
        {
            return targets.Take(Value).ToList();
        }
    }
    
    public class LastNFilter : EntityFilter
    {
        public int Value { get; set; } = 1;

        public override List<EntityBase> Apply(List<EntityBase> targets, EntityBase self)
        {
            return targets.Skip(Math.Max(0, targets.Count - Value)).ToList();
        }
    }

    public class ExcludeSelfFilter : EntityFilter
    {
        public override List<EntityBase> Apply(List<EntityBase> targets, EntityBase self)
        {
            return targets.Where(e => e != self).ToList();
        }
    }

    public class IsAliveFilter : EntityFilter
    {
        public override List<EntityBase> Apply(List<EntityBase> targets, EntityBase self)
        {
            return targets.Where(e => !e.IsDead).ToList();
        }
    }
    
    public class HealthFilter : EntityFilter
    {
        public string Operator { get; set; }
        public int Value { get; set; }

        public override List<EntityBase> Apply(List<EntityBase> targets, EntityBase self)
        {
            return targets.Where(e => Compare(e.HP, Operator, Value)).ToList();
        }

        private static bool Compare(int health, string op, int value)
        {
            return op switch
            {
                ">"  => health > value,
                "<"  => health < value,
                ">=" => health >= value,
                "<=" => health <= value,
                "==" => health == value,
                "!=" => health != value,
                _    => throw new ArgumentException($"Unsupported operator: {op}")
            };
        }
    }
    
    public class TypeFilter : EntityFilter
    {
        public List<string> MatchTypes { get; set; } = new();
        public bool Not { get; set; } = false;

        public override List<EntityBase> Apply(List<EntityBase> targets, EntityBase self)
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
    
    public class DealtDamageToPlayerFilter : EntityFilter
    {
        public override List<EntityBase> Apply(List<EntityBase> targets, EntityBase self)
        {
            return targets
                .OfType<Enemy>()
                .Where(e => e.DealtDamageToPlayer)
                .Cast<EntityBase>()
                .ToList();
        }
    }

}