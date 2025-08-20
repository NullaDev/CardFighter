using System.Collections.Generic;
using Card;
using GameLogic.SceneControl;
using Registry;
using Registry.Data;

namespace GameLogic.Entity
{
    public abstract class Enemy: EntityBase, IActionableEntity
    {
        public SortedDictionary<int, EntityBehavior> Behaviors { get; set; } = new();
        public CardInstance NextTurnCard = null;
        public bool DealtDamageToPlayer = false;
        public Enemy(int hp) : base(hp) {}

        public virtual EntityBase InitializeBehaviors()
        {
            return this;
        }

        public CardInstance ThinkNextTurnCard(FightingControl fc)
        {
            foreach (var kv in Behaviors)
            {
                var action = kv.Value.TryExecute(this, fc);
                if (action != null) return action;
            }
            return new CardInstance(CommonCards.DoNothing);
        }
        
        public override void Hurt(EntityBase source, int value, BattleField battleField)
        {
            this.HP -= value;
            if (this.HP <= 0)
            {
                this.SetDeadAndRemove(battleField);
            }
        }
    }

    public class SimpleEnemy : Enemy
    {
        public CardPrototype HeldCard;
        public SimpleEnemy(int hp) : base(hp) {}

        public override EntityBase InitializeBehaviors()
        {
            Behaviors.Add(0, new SimpleAttackPlayerBehavior(HeldCard));
            Behaviors.Add(1, new SimpleAttackPassiveBehavior(HeldCard));
            Behaviors.Add(2, new BoundaryTurnBackBehavior());
            Behaviors.Add(3, new FacePlayerTurnBehavior());
            Behaviors.Add(4, new ApproachPlayerBehavior());
            Behaviors.Add(5, new BlindAttackBehavior(HeldCard));
            Behaviors.Add(6, new IdleBehavior());
            return this;
        }

    }
    
    public class StationaryEnemy : Enemy
    {
        public CardPrototype HeldCard;
        public StationaryEnemy(int hp) : base(hp) {}
        
        public override EntityBase InitializeBehaviors()
        {
            Behaviors.Add(0, new SimpleAttackPlayerBehavior(HeldCard));
            Behaviors.Add(1, new SimpleAttackPassiveBehavior(HeldCard));
            Behaviors.Add(2, new FacePlayerTurnBehavior());
            Behaviors.Add(3, new BlindAttackBehavior(HeldCard));
            Behaviors.Add(4, new IdleBehavior());
            return this;
        }
    }

    
    public class EliteEnemy : Enemy
    {
        public List<CardPrototype> HeldCards;
        public EliteEnemy(int hp) : base(hp) {}

        public override EntityBase InitializeBehaviors()
        {
            Behaviors.Add(0, new ComplexAttackPlayerBehavior(HeldCards));
            Behaviors.Add(1, new ComplexAttackPassiveBehavior(HeldCards));
            Behaviors.Add(2, new BoundaryTurnBackBehavior());
            Behaviors.Add(3, new FacePlayerTurnBehavior());
            Behaviors.Add(4, new ApproachPlayerBehavior());
            Behaviors.Add(5, new IdleBehavior());
            return this;
        }
    }
}