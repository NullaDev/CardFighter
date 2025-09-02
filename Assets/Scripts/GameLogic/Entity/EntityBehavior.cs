using System;
using System.Collections.Generic;
using System.Linq;
using Card;
using Card.Engine;
using GameLogic.SceneControl;
using JetBrains.Annotations;
using Registry;
using Registry.Data;

namespace GameLogic.Entity
{
    public abstract class EntityBehavior
    {
        protected CardPrototype Card { get; }
        protected EntityBehavior(CardPrototype card)
        {
            Card = card;
        }
        
        [CanBeNull] public abstract CardInstance TryExecute(EntityBase entity, FightingControl fc);
    }
    
    public class SimpleAttackPlayerBehavior : EntityBehavior
    {
        public SimpleAttackPlayerBehavior(CardPrototype card) : base(card) { }

        public override CardInstance TryExecute(EntityBase entity, FightingControl fc)
        {
            var action = Card.Actions.FirstOrDefault(a => a.Processors.Any(p => p is DamageProcessor));
            if (action == null) return null;

            var targets = action.Selector.Select(fc, entity);
            targets = action.Filters.Aggregate(targets, (cur, f) => f.Apply(cur, entity));

            if (targets.Any(t => t is Player))
                return new CardInstance(Card);

            return null;
        }
    }

    public class SimpleAttackPassiveBehavior : EntityBehavior
    {
        public SimpleAttackPassiveBehavior(CardPrototype card) : base(card) { }

        public override CardInstance TryExecute(EntityBase entity, FightingControl fc)
        {
            var action = Card.Actions.FirstOrDefault(a => a.Processors.Any(p => p is DamageProcessor));
            if (action == null) return null;

            var targets = action.Selector.Select(fc, entity);
            targets = action.Filters.Aggregate(targets, (cur, f) => f.Apply(cur, entity));

            if (targets.Count > 0 && targets.All(t => t is PassiveEntity))
                return new CardInstance(Card);

            return null;
        }
    }

    public class BoundaryTurnBackBehavior : EntityBehavior
    {
        public BoundaryTurnBackBehavior() : base(CommonCards.TurnBack) { }

        public override CardInstance TryExecute(EntityBase entity, FightingControl fc)
        {
            var bf = fc.BattleField;
            var pos = bf.GetEntityIndex(entity);

            if (entity.Facing == EntityFacing.Left && pos == 0)
                return new CardInstance(Card);
            if (entity.Facing == EntityFacing.Right && pos == bf.Size - 1)
                return new CardInstance(Card);

            return null;
        }
    }

    public class FacePlayerTurnBehavior : EntityBehavior
    {
        public FacePlayerTurnBehavior() : base(CommonCards.TurnBack) { }

        public override CardInstance TryExecute(EntityBase entity, FightingControl fc)
        {
            var bf = fc.BattleField;
            var pos = bf.GetEntityIndex(entity);
            var playerPos = bf.GetPlayerIndex();

            if ((playerPos < pos && entity.Facing != EntityFacing.Left) ||
                (playerPos > pos && entity.Facing != EntityFacing.Right))
            {
                return new CardInstance(Card);
            }
            return null;
        }
    }
    
    public class ApproachPlayerBehavior : EntityBehavior
    {
        public ApproachPlayerBehavior() : base(CommonCards.Move1) { }

        public override CardInstance TryExecute(EntityBase entity, FightingControl fc)
        {
            var bf = fc.BattleField;
            var pos = bf.GetEntityIndex(entity);
            var playerPos = bf.GetPlayerIndex();
            var dir = entity.Facing == EntityFacing.Right ? 1 : -1;

            var nextPos = pos + dir;
            if (nextPos >= 0 && nextPos < bf.Size && bf.ListEntities[nextPos] == null)
            {
                return new CardInstance(Card);
            }
            return null;
        }
    }
    
    public class ApproachEffectiveRangeBehavior : EntityBehavior
    {
        private readonly CardPrototype _attackCard;
        public ApproachEffectiveRangeBehavior(CardPrototype attackCard) : base(CommonCards.Move1)
        {
            _attackCard = attackCard;
        }

        public override CardInstance TryExecute(EntityBase entity, FightingControl fc)
        {
            var action = _attackCard.Actions?.FirstOrDefault(a => a.Processors.Any(p => p is DamageProcessor));
            if (action?.Selector is not RangeSelector rs) return null;

            var bf = fc.BattleField;
            var selfPos= bf.GetEntityIndex(entity);
            var playerPos= bf.GetPlayerIndex();
            var dir= entity.Facing == EntityFacing.Right ? 1 : -1;

            var minPos = Math.Clamp(selfPos + rs.RangeMin * dir, 0, bf.Size - 1);
            var maxPos = Math.Clamp(selfPos + rs.RangeMax * dir, 0, bf.Size - 1);
            var minAttackPos = Math.Min(minPos, maxPos);
            var maxAttackPos = Math.Max(minPos, maxPos);

            var curDist = DistanceToInterval(playerPos, minAttackPos, maxAttackPos);

            var nextPos = selfPos + dir;
            if (nextPos < 0 || nextPos >= bf.Size) return null;
            if (bf.ListEntities[nextPos] != null) return null;

            var newMin = Math.Clamp(nextPos + rs.RangeMin * dir, 0, bf.Size - 1);
            var newMax = Math.Clamp(nextPos + rs.RangeMax * dir, 0, bf.Size - 1);
            var newAttackMin = Math.Min(newMin, newMax);
            var newAttackMax = Math.Max(newMin, newMax);
            var newDist = DistanceToInterval(playerPos, newAttackMin, newAttackMax);

            return newDist < curDist ? new CardInstance(Card) : null;

            int DistanceToInterval(int point, int lo, int hi)
            {
                if (point < lo) return lo - point;
                if (point > hi) return point - hi;
                return 0;
            }
        }
    }
    
    public class BlindAttackBehavior : EntityBehavior
    {
        public BlindAttackBehavior(CardPrototype card) : base(card) { }

        public override CardInstance TryExecute(EntityBase entity, FightingControl fc)
        {
            var action = Card.Actions.FirstOrDefault(a => a.Processors.Any(p => p is DamageProcessor));
            if (action == null) return null;

            var targets = action.Selector.Select(fc, entity);
            targets = action.Filters.Aggregate(targets, (cur, f) => f.Apply(cur, entity));

            if (targets.All(t => t is not Enemy) && targets.Count > 0)
                return new CardInstance(Card);

            return null;
        }
    }

    public class IdleBehavior : EntityBehavior
    {
        public IdleBehavior() : base(CommonCards.DoNothing) { }

        public override CardInstance TryExecute(EntityBase entity, FightingControl fc)
        {
            return new CardInstance(Card);
        }
    }
    
    public class ComplexAttackPlayerBehavior : EntityBehavior
    {
        private readonly List<CardPrototype> _cards;

        public ComplexAttackPlayerBehavior(List<CardPrototype> cards) : base(CommonCards.DoNothing)
        {
            _cards = cards ?? new List<CardPrototype>();
        }

        public override CardInstance TryExecute(EntityBase entity, FightingControl fc)
        {
            CardPrototype bestCard = null;
            var bestDamage = 0;

            foreach (var card in _cards)
            {
                var totalDamage = 0;
                foreach (var action in card.Actions.Where(a => a.Processors.Any(p => p is DamageProcessor)))
                {
                    var targets = action.Selector.Select(fc, entity);
                    targets = action.Filters.Aggregate(targets, (cur, f) => f.Apply(cur, entity));

                    if (targets.Any(t => t is Player))
                    {
                        totalDamage += action.Processors.OfType<DamageProcessor>().Sum(p => p.Value);
                    }
                }

                if (totalDamage > bestDamage)
                {
                    bestDamage = totalDamage;
                    bestCard = card;
                }
            }

            if (bestCard != null && bestDamage > 0)
                return new CardInstance(bestCard);

            return null;
        }
    }
    
    public class ComplexAttackPassiveBehavior : EntityBehavior
    {
        private readonly List<CardPrototype> _cards;

        public ComplexAttackPassiveBehavior(List<CardPrototype> cards) : base(null)
        {
            _cards = cards;
        }

        public override CardInstance TryExecute(EntityBase entity, FightingControl fc)
        {
            CardPrototype bestCard = null;
            var bestDamage = 0;

            foreach (var card in _cards)
            {
                var totalDamage = 0;
                foreach (var action in card.Actions.Where(a => a.Processors.Any(p => p is DamageProcessor)))
                {
                    var targets = action.Selector.Select(fc, entity);
                    targets = action.Filters.Aggregate(targets, (cur, f) => f.Apply(cur, entity));

                    if (targets.Any(t => t is Enemy))
                    {
                        totalDamage = 0;
                        break;
                    }

                    if (targets.Count > 0)
                    {
                        totalDamage += action.Processors.OfType<DamageProcessor>().Sum(p => p.Value);
                    }
                }

                if (totalDamage > bestDamage)
                {
                    bestDamage = totalDamage;
                    bestCard = card;
                }
            }

            if (bestCard != null && bestDamage > 0)
                return new CardInstance(bestCard);

            return null;
        }
    }
    
    public class BuffOnEnemyTargetBehavior : EntityBehavior
    {
        public BuffOnEnemyTargetBehavior(CardPrototype card) : base(card) { }

        public override CardInstance TryExecute(EntityBase entity, FightingControl fc)
        {
            if (Card?.Actions == null || Card.Actions.Count == 0) return null;

            foreach (var action in Card.Actions)
            {
                var targets = action.Selector.Select(fc, entity);
                targets = action.Filters.Aggregate(targets, (cur, f) => f.Apply(cur, entity));

                if (targets.Any(t => t is Enemy))
                {
                    return new CardInstance(Card);
                }
            }

            return null;
        }
    }
    
    public class AlwaysPlayBehavior : EntityBehavior
    {
        public AlwaysPlayBehavior(CardPrototype card) : base(card) { }

        public override CardInstance TryExecute(EntityBase entity, FightingControl fc)
        {
            return Card == null ? null : new CardInstance(Card);
        }
    }

}