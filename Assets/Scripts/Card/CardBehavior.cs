using System;
using System.Collections.Generic;
using Entity;
using Fighting;
using GameLogic;
using UnityEngine;

namespace Card
{
    public abstract class CardBehavior
    {
        public string Type { get; set; }
        public abstract Action<FightingControl, EntityBase> Execute();
    }
    
    public class MoveForwardBehavior : CardBehavior
    {
        public int Value { get; set; } = 1;
        public override Action<FightingControl, EntityBase> Execute()
        {
            return (fc, user) =>
            {
                var map = fc.BattleField;
                var pos = map.GetEntityIndex(user);

                var newPos = pos;
                var moveStep = user.Facing == EntityFacing.LEFT ? -1 : 1;
                var stepsTaken = 0;

                while (stepsTaken < Value)
                {
                    var tempPos = newPos + moveStep;
                    if (tempPos < 0 || tempPos > map.Size-1 || map.ListEntities[tempPos] != null)
                    {
                        break;
                    }
                    newPos = tempPos;
                    stepsTaken++;
                }

                if (pos != newPos)
                {
                    map.ListEntities[newPos] = user;
                    map.ListEntities[pos] = null;
                }
            };
        }
    }
    
    public class TurnBackBehavior : CardBehavior
    {
        public override Action<FightingControl, EntityBase> Execute()
        {
            return (fc, user) =>
            {
                user.Facing = user.Facing == EntityFacing.LEFT ? EntityFacing.RIGHT : EntityFacing.LEFT;
            };
        }
    }

    public class DamageBehavior : CardBehavior
    {
        public int Value { get; set; }
        public int RangeMin { get; set; }
        public int RangeMax { get; set; }
        public bool Aoe { get; set; } = false;
        public int KnockBack { get; set; } = 0;
        public List<string> Tags { get; set; } = new();
        public override Action<FightingControl, EntityBase> Execute()
        {
            return (fc, user) =>
            {
                var map = fc.BattleField;
                var pos = map.GetEntityIndex(user);
                
                var minPos = user.Facing == EntityFacing.RIGHT ? pos + RangeMin : pos - RangeMin;
                var maxPos = user.Facing == EntityFacing.RIGHT ? pos + RangeMax : pos - RangeMax;
                
                minPos = Math.Clamp(minPos, 0, map.Size - 1);
                maxPos = Math.Clamp(maxPos, 0, map.Size - 1);
                
                var localTags = new List<string>(this.Tags);
                localTags.Add(this.RangeMax<=2? DamageTypeNames.Melee:DamageTypeNames.Ranged);

                var curPos = minPos;
                var direc = minPos >= maxPos ? -1 : 1;
                var entitySnapshot = (EntityBase[])map.ListEntities.Clone();
                while (true)
                {
                    if (curPos != pos && entitySnapshot[curPos] != null)
                    {
                        var target = entitySnapshot[curPos];
                        if (target != null)
                        {
                            user.DoDamageTo(target, Value, map, localTags);
                            if (!target.IsDead && KnockBack > 0)
                            {
                                var moveStep = Math.Sign(curPos - pos) * KnockBack;
                                map.TryMoveEntity(curPos, moveStep);
                            }
                            if (!Aoe) break;
                        }
                    }
                    if (curPos == maxPos) break;
                    curPos += direc;
                }
            };
        }
    }
    
    public class AddCostBehavior : CardBehavior
    {
        public int Value { get; set; }
        public override Action<FightingControl, EntityBase> Execute()
        {
            return (fc, user) =>
            {
                fc.FightingData.TryAddCost(Value);
            };
        }
    }
    
    public class AddBuffBehavior : CardBehavior
    {
        public string BuffName { get; set; }
        public int Turn { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();
        public override Action<FightingControl, EntityBase> Execute()
        {
            return (fc, user) =>
            {
                var buff = new EntityBuff(BuffName, Turn);
                buff.Parameters = new Dictionary<string, object>(Parameters);
                user.AddOrUpdateBuff(buff);
            };
        }
    }
    
    public class ForceTurnBehavior : CardBehavior
    {
        public int RangeMin { get; set; }
        public int RangeMax { get; set; }
        public bool Aoe { get; set; } = false;

        public override Action<FightingControl, EntityBase> Execute()
        {
            return (fc, user) =>
            {
                var map = fc.BattleField;
                var pos = map.GetEntityIndex(user);

                var minPos = user.Facing == EntityFacing.RIGHT ? pos + RangeMin : pos - RangeMin;
                var maxPos = user.Facing == EntityFacing.RIGHT ? pos + RangeMax : pos - RangeMax;

                minPos = Math.Clamp(minPos, 0, map.Size - 1);
                maxPos = Math.Clamp(maxPos, 0, map.Size - 1);

                var curPos = minPos;
                var direc = minPos >= maxPos ? -1 : 1;
                var entitySnapshot = (EntityBase[])map.ListEntities.Clone();

                while (true)
                {
                    if (curPos != pos && entitySnapshot[curPos] != null)
                    {
                        var target = entitySnapshot[curPos];
                        if (target != null)
                        {
                            target.Facing = target.Facing == EntityFacing.LEFT ? EntityFacing.RIGHT : EntityFacing.LEFT;
                            if (!Aoe) break;
                        }
                    }
                    if (curPos == maxPos) break;
                    curPos += direc;
                }
            };
        }
    }
    
    public class ForceMoveBehavior : CardBehavior
    {
        public int Value { get; set; } = 1; // Positive -> push, negative -> pull
        public int RangeMin { get; set; }
        public int RangeMax { get; set; }
        public bool Aoe { get; set; } = false;

        public override Action<FightingControl, EntityBase> Execute()
        {
            return (fc, user) =>
            {
                var map = fc.BattleField;
                var pos = map.GetEntityIndex(user);

                var minPos = user.Facing == EntityFacing.RIGHT ? pos + RangeMin : pos - RangeMin;
                var maxPos = user.Facing == EntityFacing.RIGHT ? pos + RangeMax : pos - RangeMax;

                minPos = Math.Clamp(minPos, 0, map.Size - 1);
                maxPos = Math.Clamp(maxPos, 0, map.Size - 1);

                var curPos = minPos;
                var direc = minPos >= maxPos ? -1 : 1;
                var entitySnapshot = (EntityBase[])map.ListEntities.Clone();

                while (true)
                {
                    if (curPos != pos && entitySnapshot[curPos] != null)
                    {
                        var target = entitySnapshot[curPos];
                        if (target != null)
                        {
                            var direction = Math.Sign(curPos - pos) * (Value > 0 ? 1 : -1);
                            map.TryMoveEntity(curPos, direction * Math.Abs(Value));
                            if (!Aoe) break;
                        }
                    }
                    if (curPos == maxPos) break;
                    curPos += direc;
                }
            };
        }
    }
    
}