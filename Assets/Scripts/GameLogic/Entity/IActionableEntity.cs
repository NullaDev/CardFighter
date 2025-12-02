using System.Collections.Generic;
using Card;
using SceneControl;

namespace GameLogic.Entity
{
    public interface IActionableEntity
    {
        CardInstance NextTurnCard { get; set; } 
        SortedDictionary<int, EntityBehavior> Behaviors { get; set; } 
        CardInstance ThinkNextTurnCard(FightingControl fc);
        int TurnsPerAction { get; set; }
        int ActionTick { get; set; }

        public abstract void InitializeBehaviors();
    }
}