using System.Collections.Generic;
using Entity;
using Fighting;
using GameLogic;

namespace Card.Engine
{
    public abstract class EntitySelector
    {
        public abstract List<EntityBase> Select(FightingControl fc, EntityBase user);
    }
    
    public class EmptySelector : EntitySelector
    {
        public override List<EntityBase> Select(FightingControl fc, EntityBase user)
        {
            return new List<EntityBase>();
        }
    }

    public class SelfSelector : EntitySelector
    {
        public override List<EntityBase> Select(FightingControl fc, EntityBase user)
        {
            return new List<EntityBase> { user };
        }
    }

    public class RangeSelector : EntitySelector
    {
        public int RangeMin { get; set; }
        public int RangeMax { get; set; }

        public override List<EntityBase> Select(FightingControl fc, EntityBase user)
        {
            var list = new List<EntityBase>();
            var pos = fc.BattleField.GetEntityIndex(user);
            var direction = user.Facing == EntityFacing.RIGHT ? 1 : -1;

            for (int i = RangeMin; i <= RangeMax; i++)
            {
                int index = pos + i * direction;
                if (index >= 0 && index < fc.BattleField.Size)
                {
                    var target = fc.BattleField.ListEntities[index];
                    if (target != null) list.Add(target);
                }
            }

            return list;
        }
    }
}