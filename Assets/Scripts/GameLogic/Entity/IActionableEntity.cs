using System.Collections.Generic;
using Card;
using GameLogic.SceneControl;

namespace GameLogic.Entity
{
    public interface IActionableEntity
    {
        CardInstance NextTurnCard { get; set; } 
        SortedDictionary<int, EntityBehavior> Behaviors { get; set; } 
        CardInstance ThinkNextTurnCard(FightingControl fc);

        public abstract EntityBase InitializeBehaviors();
    }
}