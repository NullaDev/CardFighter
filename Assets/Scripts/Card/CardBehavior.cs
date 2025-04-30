using System;
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
                while (true)
                {
                    if (curPos != pos && map.ListEntities[curPos] != null)
                    {
                        map.ListEntities[curPos].Hurt(user, Value, map);

                        if (map.ListEntities[curPos] != null && KnockBack>0)
                        {
                            var enemyNewPos = curPos;
                            var stepsTaken = 0;
                            var moveStep = Math.Sign(curPos-pos);
                            while (stepsTaken < KnockBack)
                            {
                                if (enemyNewPos <= 0 || enemyNewPos >= map.Size-1 || map.ListEntities[enemyNewPos+moveStep] != null)
                                {
                                    break;
                                }
                                enemyNewPos += moveStep;
                                stepsTaken++;
                            }
                            if (curPos != enemyNewPos)
                            {
                                map.ListEntities[enemyNewPos] = map.ListEntities[curPos];
                                map.ListEntities[curPos] = null;
                            }
                        }
                        
                        if (!Aoe) break;
                    }
                    if (curPos==maxPos) break;
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
    
}