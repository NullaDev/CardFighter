using System;
using System.Collections.Generic;
using System.Linq;
using Entity;

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

}