using System.Collections.Generic;
using System.Linq;
using Entity;
using Fighting;

namespace Card.Engine
{
    public class EntityAction
    {
        public EntitySelector Selector { get; set; }
        public List<EntityFilter> Filters { get; set; } = new();
        public List<EntityProcessor> Processors { get; set; } = new();

        public void Execute(FightingControl fc, EntityBase user)
        {
            var targets = Selector.Select(fc, user);
            targets = Filters.Aggregate(targets, (current, filter) => filter.Apply(current, user));

            foreach (var target in targets)
            {
                foreach (var processor in Processors)
                {
                    processor.Process(fc, user, target);
                }
            }
        }
    }
}