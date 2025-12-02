using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic;
using GameLogic.Buff;
using GameLogic.Entity;
using Registry;
using Registry.Data;
using SceneControl;

namespace Card.Engine
{
    public abstract class EntityProcessor
    {
        public abstract void Process(FightingControl fc, EntityBase user, EntityBase target);
    }
    
    public class MoveProcessor : EntityProcessor
    {
        public string Mode { get; set; } = "forward";
        public int Value { get; set; } = 1;

        public override void Process(FightingControl fc, EntityBase user, EntityBase target)
        {
            var map = fc.BattleField;
            var userPos = map.GetEntityIndex(user);
            var targetPos = map.GetEntityIndex(target);

            if (userPos == -1 || targetPos == -1)
            {
                throw new Exception("Can't find one or more entity when executing Move.");
            }
            
            if (Mode is "forward")
            {
                if (target.HasBuff(EntityBuffManager.Rooted)) return;
            }

            if (Mode is "push" or "pull")
            {
                if (user == target) return;
                if (target.HasBuff(EntityBuffManager.SuperArmor)) return;
            }

            var direction = Mode switch
            {
                "forward" => (int)user.Facing,
                "push"    => Math.Sign(targetPos - userPos),
                "pull"    => Math.Sign(userPos - targetPos),
                _         => throw new ArgumentException($"Unsupported MoveProcessor mode: {Mode}")
            };
            map.TryMoveEntityStepByStep(targetPos, direction * Value);
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

            if (DirectionMode is "auto")
            {
                if (target.HasBuff(EntityBuffManager.LockedFacing)) return;
            }

            if (DirectionMode is "towards" or "away")
            {
                if (user == target) return;
                if (target.HasBuff(EntityBuffManager.SuperArmor)) return;
            }
            target.Facing = DirectionMode switch
            {
                "towards" => FacingHelper.GetFacing(userPos - targetPos),
                "away" => FacingHelper.GetFacing(targetPos - userPos),
                _ => (EntityFacing)(-(int)target.Facing)
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
    
    public class AddBuffProcessor : EntityProcessor
    {
        public List<BuffData> Buffs { get; set; } = new();

        public override void Process(FightingControl fc, EntityBase user, EntityBase target)
        {
            foreach (var buffData in Buffs)
            {
                target.AddOrUpdateBuff(new EntityBuff(buffData));
            }
        }
    }
    
    public class AddCostProcessor : EntityProcessor
    {
        public int Value { get; set; }

        public override void Process(FightingControl fc, EntityBase user, EntityBase target)
        {
            if (target is not Player) return;
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
            var direction = (int)user.Facing;

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
    
    public class ClearBuffProcessor : EntityProcessor
    {
        public string BuffType { get; set; } = "negative";
        public int Count { get; set; } = -1;

        public override void Process(FightingControl fc, EntityBase user, EntityBase target)
        {
            var toRemove = target.Buffs
                .Where(b =>
                {
                    return BuffType switch
                    {
                        "positive" => b.BuffType == EntityBuffManager.BuffType.Positive,
                        "negative" => b.BuffType == EntityBuffManager.BuffType.Negative,
                        "neutral" => b.BuffType == EntityBuffManager.BuffType.Neutral,
                        "all" => true,
                        _ => false
                    };
                })
                .Take(Count < 0 ? int.MaxValue : Count)
                .ToList();

            foreach (var b in toRemove)
            {
                target.Buffs.Remove(b);
            }
        }
    }
    
    public class RemoveBuffProcessor : EntityProcessor
    {
        public List<string> Buffs { get; set; } = new();

        public override void Process(FightingControl fc, EntityBase user, EntityBase target)
        {
            if (target.Buffs == null || target.Buffs.Count == 0) return;

            target.Buffs.RemoveAll(b => Buffs.Contains(b.Name));
        }
    }
    
    public class SummonProcessor : EntityProcessor
    {
        public string Mode { get; set; }
        public int Position { get; set; } = 1;
        public int Hp { get; set; }
        public string Name { get; set; }
        public string TextureName { get; set; }
        public string Card { get; set; }
        public int TurnsPerAction { get; set; } = -1;
        public int InitialActionTick { get; set; } = 0;

        public override void Process(FightingControl fc, EntityBase user, EntityBase target)
        {
            var map = fc.BattleField;
            var targetIndex = map.GetEntityIndex(target);
            if (targetIndex < 0)
                throw new Exception("Can't find target when executing Summon.");

            var dir = (int)target.Facing;
            var spawnIndex = targetIndex + dir * Position;

            if (spawnIndex < 0 || spawnIndex >= map.Size)
                return;
            if (map.ListEntities[spawnIndex] != null)
                return;

            EntityBase entity = Mode switch
            {
                "passive"               => new PassiveEntity(Hp),
                "simple_enemy"          => new SimpleEnemy(Hp),
                "stationary_enemy"      => new StationaryEnemy(Hp),
                "stationary_buff_enemy" => new StationaryBuffEnemy(Hp),
                "fixed_card_enemy"      => new FixedCardEnemy(Hp),
                _ => throw new ArgumentException($"Unsupported SummonProcessor mode: {Mode}")
            };

            entity.Name = this.Name;
            entity.TextureName = "Arts/Entities/" + this.TextureName;
            entity.Facing = entity is PassiveEntity? EntityFacing.Default : user.Facing;

            switch (entity)
            {
                case SimpleEnemy e:
                    e.HeldCard = StaticDataManager.CardDataManager.Find(this.Card);
                    e.TurnsPerAction = this.TurnsPerAction;
                    e.ActionTick = this.InitialActionTick;
                    break;
                case StationaryEnemy e:
                    e.HeldCard = StaticDataManager.CardDataManager.Find(this.Card);
                    e.TurnsPerAction = this.TurnsPerAction;
                    e.ActionTick = this.InitialActionTick;
                    break;
                case StationaryBuffEnemy e:
                    e.HeldCard = StaticDataManager.CardDataManager.Find(this.Card);
                    e.TurnsPerAction = this.TurnsPerAction;
                    e.ActionTick = this.InitialActionTick;
                    break;
                case FixedCardEnemy e:
                    e.HeldCard = StaticDataManager.CardDataManager.Find(this.Card);
                    e.TurnsPerAction = this.TurnsPerAction;
                    e.ActionTick = this.InitialActionTick;
                    break;
            }

            map.AddEntityToMap(entity, spawnIndex);
        }
    }
    
    public class ExecuteActionProcessor : EntityProcessor
    {
        public EntityAction Action { get; set; }

        public override void Process(FightingControl fc, EntityBase user, EntityBase target)
        {
            Action?.Execute(fc, target);
        }
    }
    
}