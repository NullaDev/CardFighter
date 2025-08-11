using System;
using System.Collections.Generic;
using System.Linq;
using Card;
using Card.Engine;
using GameLogic.SceneControl;
using Newtonsoft.Json;
using Registry;
using Registry.Data;

namespace GameLogic.Entity
{
    public abstract class Enemy: EntityBase
    {
        [JsonIgnore] public CardInstance NextTurnCard = null;
        [JsonIgnore] public bool DealtDamageToPlayer = false;
        public Enemy(int hp) : base(hp)
        {
        }

        public override void Hurt(EntityBase source, int value, BattleField battleField)
        {
            this.HP -= value;
            if (this.HP <= 0)
            {
                this.SetDeadAndRemove(battleField);
            }
        }

        public abstract CardInstance ThinkNextTurnCard(FightingControl fc);
    }

    public class SimpleEnemy : Enemy
    {
        [JsonIgnore] public CardPrototype HeldCard;
        public SimpleEnemy(int hp) : base(hp)
        {
        }

        public override CardInstance ThinkNextTurnCard(FightingControl fc)
        {
            var battleField = fc.BattleField;
            var attackAction = this.HeldCard.Actions.FirstOrDefault(action =>
                action.Selector is RangeSelector &&
                action.Processors.Any(p => p is DamageProcessor)
            );
            if (attackAction == null)
                return new CardInstance(CommonCards.DoNothing);
            var rangeSelector = attackAction.Selector as RangeSelector;
            
            var selfPos = battleField.GetEntityIndex(this);
            var playerPos = battleField.GetPlayerIndex();
            var rangeMin = rangeSelector.RangeMin;
            var rangeMax = rangeSelector.RangeMax;
            var direction = this.Facing == EntityFacing.Right ? 1 : -1;
                
            var minPos = selfPos + rangeMin * direction;
            var maxPos = selfPos + rangeMax * direction;
            minPos = Math.Clamp(minPos, 0, battleField.Size - 1);
            maxPos = Math.Clamp(maxPos, 0, battleField.Size - 1);

            var minAttackPos = Math.Min(minPos, maxPos);
            var maxAttackPos = Math.Max(minPos, maxPos);
            if (playerPos >= minAttackPos && playerPos <= maxAttackPos)
            {
                var targets = rangeSelector.Select(fc, this);
                targets = attackAction.Filters.Aggregate(targets, (current, filter) => filter.Apply(current, this));

                var onlyPassive = targets.All(t => t is PassiveEntity);
                var containsPlayer = targets.Any(t => t is Player);

                if (containsPlayer || (targets.Count > 0 && onlyPassive))
                {
                    return new CardInstance(this.HeldCard);
                }
                else
                {
                    return new CardInstance(CommonCards.DoNothing);
                }
            }

            if (this.Facing == EntityFacing.Left && selfPos == 0)
            {
                return new CardInstance(CommonCards.TurnBack);
            }
            if (this.Facing == EntityFacing.Right && selfPos == battleField.Size-1)
            {
                return new CardInstance(CommonCards.TurnBack);
            }
            
            var distance = playerPos <= minAttackPos? minAttackPos - playerPos : playerPos - maxAttackPos;
            var newMin = Math.Clamp(minAttackPos + direction, 0, battleField.Size - 1);
            var newMax = Math.Clamp(maxAttackPos + direction, 0, battleField.Size - 1);
            var newDistance = playerPos <= newMin? newMin - playerPos : playerPos - newMax;

            if (newDistance < distance)
            {
                var nextPos = selfPos + direction;
                if (nextPos >= 0 && nextPos < battleField.Size && battleField.ListEntities[nextPos] == null)
                {
                    return new CardInstance(CommonCards.Move1);
                }
                else
                {
                    var willHurtAllies = false;
                    for (var i = minAttackPos; i <= maxAttackPos; i++)
                    {
                        var entity = battleField.ListEntities[i];
                        if (entity is Enemy && entity != this)
                        {
                            willHurtAllies = true;
                            break;
                        }
                    }
                    return willHurtAllies ? new CardInstance(CommonCards.DoNothing) : new CardInstance(this.HeldCard);
                }
            }
            else
            {
                return new CardInstance(CommonCards.TurnBack);
            }
        }
    }
    
    public class StationaryEnemy : Enemy
    {
        [JsonIgnore] public CardPrototype HeldCard;

        public StationaryEnemy(int hp) : base(hp)
        {
        }

        public override CardInstance ThinkNextTurnCard(FightingControl fc)
        {
            var battleField = fc.BattleField;
            var attackAction = this.HeldCard.Actions.FirstOrDefault(action =>
                action.Selector is RangeSelector &&
                action.Processors.Any(p => p is DamageProcessor)
            );
            if (attackAction == null)
                return new CardInstance(CommonCards.DoNothing);
            var rangeSelector = attackAction.Selector as RangeSelector;

            var selfPos = battleField.GetEntityIndex(this);
            var playerPos = battleField.GetPlayerIndex();

            if ((playerPos < selfPos && this.Facing != EntityFacing.Left) ||
                (playerPos > selfPos && this.Facing != EntityFacing.Right))
            {
                return new CardInstance(CommonCards.TurnBack);
            }

            var rangeMin = rangeSelector.RangeMin;
            var rangeMax = rangeSelector.RangeMax;
            var direction = this.Facing == EntityFacing.Right ? 1 : -1;

            var minPos = selfPos + rangeMin * direction;
            var maxPos = selfPos + rangeMax * direction;

            minPos = Math.Clamp(minPos, 0, battleField.Size - 1);
            maxPos = Math.Clamp(maxPos, 0, battleField.Size - 1);

            var minAttackPos = Math.Min(minPos, maxPos);
            var maxAttackPos = Math.Max(minPos, maxPos);

            if (playerPos >= minAttackPos && playerPos <= maxAttackPos)
            {
                var targets = rangeSelector.Select(fc, this);
                targets = attackAction.Filters.Aggregate(targets, (current, filter) => filter.Apply(current, this));

                var onlyPassive = targets.All(t => t is PassiveEntity);
                var containsPlayer = targets.Any(t => t is Player);

                if (containsPlayer || (targets.Count > 0 && onlyPassive))
                {
                    return new CardInstance(this.HeldCard);
                }
                else
                {
                    return new CardInstance(CommonCards.DoNothing);
                }
            }

            for (var i = minAttackPos; i <= maxAttackPos; i++)
            {
                var entity = battleField.ListEntities[i];
                if (entity is Enemy && entity != this)
                {
                    return new CardInstance(CommonCards.DoNothing);
                }
            }

            return new CardInstance(CommonCards.DoNothing);
        }
    }

    
    public class EliteEnemy : Enemy
    {
        [JsonIgnore] public List<CardPrototype> HeldCards;
        public EliteEnemy(int hp) : base(hp)
        {
        }

        // Unfinished, maybe buggy
        public override CardInstance ThinkNextTurnCard(FightingControl fc)
        {
            var battleField = fc.BattleField;

            var attackInfos = new List<(CardPrototype Card, EntityAction Action, RangeSelector Range, int Damage)>();
            foreach (var card in this.HeldCards ?? Enumerable.Empty<CardPrototype>())
            {
                foreach (var action in card.Actions ?? Enumerable.Empty<EntityAction>())
                {
                    if (action.Selector is RangeSelector rs && action.Processors != null && action.Processors.Any(p => p is DamageProcessor))
                    {
                        var totalDamage = action.Processors.OfType<DamageProcessor>().Sum(p => p.Value);
                        attackInfos.Add((card, action, rs, totalDamage));
                    }
                }
            }

            if (attackInfos.Count == 0)
                return new CardInstance(CommonCards.DoNothing);

            var selfPos   = battleField.GetEntityIndex(this);
            var playerPos = battleField.GetPlayerIndex();
            var direction = this.Facing == EntityFacing.Right ? 1 : -1;

            var canHitPlayer = attackInfos.Where(info =>
                {
                    var (card, action, rangeSel, dmg) = info;
                    var minPos = Math.Clamp(selfPos + rangeSel.RangeMin * direction, 0, battleField.Size - 1);
                    var maxPos = Math.Clamp(selfPos + rangeSel.RangeMax * direction, 0, battleField.Size - 1);
                    if (playerPos < Math.Min(minPos, maxPos) || playerPos > Math.Max(minPos, maxPos))
                        return false;
                    var targets = rangeSel.Select(fc, this);
                    targets = action.Filters.Aggregate(targets, (cur, f) => f.Apply(cur, this));
                    return targets.Any(t => t is Player);
                })
                .OrderByDescending(info => info.Damage)
                .FirstOrDefault();
            if (canHitPlayer.Card != null)
                return new CardInstance(canHitPlayer.Card);

            var canHitPassive = attackInfos.Where(info =>
                {
                    var (card, action, rangeSel, dmg) = info;
                    var minPos = Math.Clamp(selfPos + rangeSel.RangeMin * direction, 0, battleField.Size - 1);
                    var maxPos = Math.Clamp(selfPos + rangeSel.RangeMax * direction, 0, battleField.Size - 1);
                    if (playerPos < Math.Min(minPos, maxPos) || playerPos > Math.Max(minPos, maxPos))
                        return false;
                    var targets = rangeSel.Select(fc, this);
                    targets = action.Filters.Aggregate(targets, (cur, f) => f.Apply(cur, this));
                    return targets.Count > 0 && targets.All(t => t is PassiveEntity);
                })
                .OrderByDescending(info => info.Damage)
                .FirstOrDefault();
            if (canHitPassive.Card != null)
                return new CardInstance(canHitPassive.Card);

            var best = attackInfos.OrderByDescending(x => x.Damage).First();
            var minAttackPos = Math.Min(
                Math.Clamp(selfPos + best.Range.RangeMin * direction, 0, battleField.Size - 1),
                Math.Clamp(selfPos + best.Range.RangeMax * direction, 0, battleField.Size - 1)
            );
            var maxAttackPos = Math.Max(
                Math.Clamp(selfPos + best.Range.RangeMin * direction, 0, battleField.Size - 1),
                Math.Clamp(selfPos + best.Range.RangeMax * direction, 0, battleField.Size - 1)
            );

            if (this.Facing == EntityFacing.Left && selfPos == 0)
                return new CardInstance(CommonCards.TurnBack);
            if (this.Facing == EntityFacing.Right && selfPos == battleField.Size - 1)
                return new CardInstance(CommonCards.TurnBack);

            var distance    = playerPos <= minAttackPos ? (minAttackPos - playerPos) : (playerPos - maxAttackPos);
            var newMin      = Math.Clamp(minAttackPos + direction, 0, battleField.Size - 1);
            var newMax      = Math.Clamp(maxAttackPos + direction, 0, battleField.Size - 1);
            var newDistance = playerPos <= newMin ? (newMin - playerPos) : (playerPos - newMax);

            if (newDistance < distance)
            {
                var nextPos = selfPos + direction;
                if (nextPos >= 0 && nextPos < battleField.Size && battleField.ListEntities[nextPos] == null)
                    return new CardInstance(CommonCards.Move1);

                var willHurtAllies = false;
                for (var i = minAttackPos; i <= maxAttackPos; i++)
                {
                    var entity = battleField.ListEntities[i];
                    if (entity is Enemy && entity != this)
                    {
                        willHurtAllies = true;
                        break;
                    }
                }
                return willHurtAllies ? new CardInstance(CommonCards.DoNothing) : new CardInstance(best.Card);
            }
            else
            {
                return new CardInstance(CommonCards.TurnBack);
            }
        }

    }
}