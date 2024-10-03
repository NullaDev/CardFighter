using System;
using System.Collections.Generic;
using System.Linq;
using Card;
using Data;
using Fighting;
using GameLogic;
using UnityEngine;

namespace Entity
{
    public abstract class Enemy: EntityBase
    {
        public CardInstance NextTurnCard;
        public Enemy(int hp) : base(hp)
        {
        }

        public override void Hurt(EntityBase source, int value, Map map)
        {
            this.HP -= value;
            if (this.HP <= 0)
            {
                map.RemoveEntityFromMap(this);
            }
        }

        public abstract CardInstance ThinkNextTurnCard(Map map);
    }

    public class SimpleEnemy : Enemy
    {
        public CardPrototype HeldCard;
        public SimpleEnemy(int hp) : base(hp)
        {
        }

        public override CardInstance ThinkNextTurnCard(Map map)
        {
            var dmg = this.HeldCard.Behaviors.OfType<DamageBehavior>().FirstOrDefault();
            if (dmg == null)
                return new CardInstance(CardData.Instance.Find("do_nothing"));
            
            var selfPos = map.GetEntityIndex(this);
            var playerPos = map.GetPlayerIndex();
            var rangeMin = dmg.RangeMin;
            var rangeMax = dmg.RangeMax;
            var direction = this.Facing == EntityFacing.RIGHT ? 1 : -1;
                
            var minPos = selfPos + rangeMin * direction;
            var maxPos = selfPos + rangeMax * direction;
            minPos = Math.Clamp(minPos, 0, map.Size - 1);
            maxPos = Math.Clamp(maxPos, 0, map.Size - 1);

            var minAttackPos = Math.Min(minPos, maxPos);
            var maxAttackPos = Math.Max(minPos, maxPos);
            if (playerPos >= minAttackPos && playerPos <= maxAttackPos)
            {
                return new CardInstance(this.HeldCard);
            }

            if (this.Facing == EntityFacing.LEFT && selfPos == 0)
            {
                return new CardInstance(CardData.Instance.Find("turn_back"));
            }
            if (this.Facing == EntityFacing.RIGHT && selfPos == map.Size-1)
            {
                return new CardInstance(CardData.Instance.Find("turn_back"));
            }
                
            var distance = playerPos <= minAttackPos? minAttackPos - playerPos : playerPos - maxAttackPos;
            minAttackPos = Math.Clamp(minAttackPos + direction, 0, map.Size - 1);
            maxAttackPos = Math.Clamp(maxAttackPos + direction, 0, map.Size - 1);
            var newDistance = playerPos <= minAttackPos? minAttackPos - playerPos : playerPos - maxAttackPos;

            if (newDistance < distance)
            {
                return new CardInstance(CardData.Instance.Find("move"));
            }
            else
            {
                return new CardInstance(CardData.Instance.Find("turn_back"));
            }

        }
    }
    
    public class EliteEnemy : Enemy
    {
        public List<CardPrototype> HeldCards;
        public EliteEnemy(int hp) : base(hp)
        {
        }

        public override CardInstance ThinkNextTurnCard(Map map)
        {
            throw new System.NotImplementedException();
        }
    }
}