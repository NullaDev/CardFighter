using System;
using Fighting;
using GameLogic;

namespace Card
{
    public abstract class CardBehavior
    {
        public string Type { get; set; }
        public abstract Action<Map> Execute();
    }
    
    public class MoveForwardBehavior : CardBehavior
    {
        public int Value { get; set; }
        public override Action<Map> Execute()
        {
            return map =>
            {
                var player = map.GetPlayerFromMap();
                var playerPos = map.GetPlayerIndex();

                var playerNewPos = playerPos;
                var moveStep = player.Facing == EntityFacing.LEFT ? -1 : 1;
                var stepsTaken = 0;

                while (stepsTaken < Value)
                {
                    if (playerNewPos <= 0 || playerNewPos >= map.Size-1 || map.ListEntities[playerNewPos+moveStep] != null)
                    {
                        break;
                    }
                    playerNewPos += moveStep;
                    stepsTaken++;
                }

                if (playerPos != playerNewPos)
                {
                    map.ListEntities[playerNewPos] = player;
                    map.ListEntities[playerPos] = null;
                }
            };
        }
    }
    
    public class TurnBackBehavior : CardBehavior
    {
        public override Action<Map> Execute()
        {
            return map =>
            {
                var player = map.GetPlayerFromMap();
                player.Facing = player.Facing == EntityFacing.LEFT ? EntityFacing.RIGHT : EntityFacing.LEFT;
            };
        }
    }

    public class DamageBehavior : CardBehavior
    {
        public int Value { get; set; }
        public int RangeMin { get; set; }
        public int RangeMax { get; set; }
        public bool Aoe { get; set; }
        public int KnockBack { get; set; }
        public override Action<Map> Execute()
        {
            return map =>
            {
                var player = map.GetPlayerFromMap();
                var playerPos = map.GetPlayerIndex();
                
                var minPos = player.Facing == EntityFacing.LEFT ? playerPos + RangeMin : playerPos - RangeMin;
                var maxPos = player.Facing == EntityFacing.LEFT ? playerPos + RangeMax : playerPos - RangeMax;
                
                minPos = Math.Clamp(minPos, 0, map.Size - 1);
                maxPos = Math.Clamp(maxPos, 0, map.Size - 1);

                var curPos = minPos;
                var direc = minPos >= maxPos ? -1 : 1;
                while (true)
                {
                    if (curPos != playerPos && map.ListEntities[curPos] != null)
                    {
                        map.ListEntities[curPos].Hurt(player, Value, map);

                        if (map.ListEntities[curPos] != null && KnockBack>0)
                        {
                            var enemyNewPos = curPos;
                            var stepsTaken = 0;
                            var moveStep = Math.Sign(curPos-playerPos);
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
    
}