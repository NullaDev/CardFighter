using System;
using System.Collections.Generic;
using Entity;
using Fighting;
using GameLogic;

namespace Card.Engine
{
    public class BuffData
    {
        public string BuffName { get; set; }
        public int Turn { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();
    }
    
    public abstract class EntityProcessor
    {
        public abstract void Process(FightingControl fc, EntityBase user, EntityBase target);
    }
    
    public class MoveForwardProcessor : EntityProcessor
    {
        public int Value { get; set; } = 1;

        public override void Process(FightingControl fc, EntityBase user, EntityBase target)
        {
            var map = fc.BattleField;
            var pos = map.GetEntityIndex(user);
            if (pos == -1 || Value == 0) return;

            var direction = user.Facing == EntityFacing.LEFT ? -1 : 1;
            map.TryMoveEntityStepByStep(pos, direction * Value);
        }
    }
    
    public class TurnProcessor : EntityProcessor
    {
        public string DirectionMode { get; set; } = "auto";

        public override void Process(FightingControl fc, EntityBase user, EntityBase target)
        {
            var userPos = fc.BattleField.GetEntityIndex(user);
            var targetPos = fc.BattleField.GetEntityIndex(target);
            if (userPos == -1 || targetPos == -1)
            {
                throw new Exception("Can't find one or more entity when execute Turn.");
            }
            target.Facing = DirectionMode switch
            {
                "towards" => targetPos < userPos ? EntityFacing.RIGHT : EntityFacing.LEFT,
                "away" => targetPos < userPos ? EntityFacing.LEFT : EntityFacing.RIGHT,
                _ => target.Facing == EntityFacing.LEFT ? EntityFacing.RIGHT : EntityFacing.LEFT
            };
        }
    }

    public class DamageProcessor : EntityProcessor
    {
        public int Value { get; set; }
        public List<string> Tags { get; set; } = new();

        public override void Process(FightingControl fc, EntityBase user, EntityBase target)
        {
            var map = fc.BattleField;
            user.DoDamageTo(target, Value, map, Tags);
        }
    }
    
    public class ForceMoveProcessor : EntityProcessor
    {
        public int Value { get; set; }  // Positive for push, negative for pull
        public override void Process(FightingControl fc, EntityBase user, EntityBase target)
        {
            var map = fc.BattleField;
            var userPos = map.GetEntityIndex(user);
            var targetPos = map.GetEntityIndex(target);

            if (userPos == -1 || targetPos == -1)
            {
                throw new Exception("Can't find one or more entity when execute KnockBack.");
            }

            var direction = Math.Sign(targetPos - userPos);
            map.TryMoveEntityStepByStep(targetPos, direction * Value);
        }
    }
    
    public class AddBuffProcessor : EntityProcessor
    {
        public List<BuffData> Buffs { get; set; } = new();

        public override void Process(FightingControl fc, EntityBase user, EntityBase target)
        {
            foreach (var buffData in Buffs)
            {
                var buff = new EntityBuff(buffData.BuffName, buffData.Turn);
                buff.Parameters = new Dictionary<string, object>(buffData.Parameters);
                target.AddOrUpdateBuff(buff);
            }
        }
    }
    
    public class AddCostProcessor : EntityProcessor
    {
        public int Value { get; set; }

        public override void Process(FightingControl fc, EntityBase user, EntityBase target)
        {
            fc.FightingData.TryAddCost(Value);
        }
    }
    
    public class AddArmorProcessor : EntityProcessor
    {
        public int Value { get; set; }

        public override void Process(FightingControl fc, EntityBase user, EntityBase target)
        {
            target.Armor += Value;
        }
    }
    
    public class MoveAttackProcessor : EntityProcessor
    {
        public int Value { get; set; }
        public int Damage { get; set; }
        public bool CanCrossEnemies { get; set; } = false;
        public List<string> Tags { get; set; } = new();

        public override void Process(FightingControl fc, EntityBase user, EntityBase _)
        {
            var map = fc.BattleField;
            var startPos = map.GetEntityIndex(user);
            var direction = user.Facing == EntityFacing.LEFT ? -1 : 1;

            var currentPos = startPos;
            var stepsTaken = 0;
            int? finalPos = null;

            var localTags = new List<string>(Tags);
            if (!localTags.Contains(DamageTypeNames.Melee))
                localTags.Add(DamageTypeNames.Melee);
            if (!localTags.Contains(DamageTypeNames.Charge))
                localTags.Add(DamageTypeNames.Charge);

            var entitySnapshot = (EntityBase[])map.ListEntities.Clone();

            while (stepsTaken < Value)
            {
                var nextPos = currentPos + direction;
                if (nextPos < 0 || nextPos >= map.Size) break;

                var target = entitySnapshot[nextPos];
                var blocked = false;

                if (target != null && target != user)
                {
                    user.DoDamageTo(target, Damage, map, localTags);

                    if (!CanCrossEnemies && !target.IsDead)
                        break;

                    if (CanCrossEnemies && !target.IsDead)
                        blocked = true;
                }

                if (!blocked && map.ListEntities[nextPos] == null)
                {
                    finalPos = nextPos;
                }

                currentPos = nextPos;
                stepsTaken++;
            }

            if (finalPos.HasValue && finalPos.Value != startPos)
            {
                map.ListEntities[startPos] = null;
                map.ListEntities[finalPos.Value] = user;
            }
        }
    }

    public class KillProcessor : EntityProcessor
    {
        public override void Process(FightingControl fc, EntityBase user, EntityBase target)
        {
            if (target.IsDead || target is EliteEnemy || target is Player)
                return;
            target.SetDeadAndRemove(fc.BattleField);
        }
    }
    
}