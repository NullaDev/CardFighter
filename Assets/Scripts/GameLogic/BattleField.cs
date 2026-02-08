using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic.Buff;
using GameLogic.Entity;
using GameLogic.Item;
using GameLogic.Runtime;
using JetBrains.Annotations;
using Registry;
using Registry.Data;
using Random = UnityEngine.Random;

namespace GameLogic
{
    public class BattleField
    {
        private readonly StageConfig _config;
        public int Size;
        [ItemCanBeNull] public EntityBase[] ListEntities;
        private readonly HashSet<EntityConfig> _spawnedEntities = new();
        public int PlayerPosCache { get; private set; } = -1;

        public BattleField(StageConfig config, PlayerData pData)
        {
            this._config = config;
            this.SetSize(config.Size);
            this.InitializePlayer(pData, config.PlayerSpawnPos, config.PlayerSpawnFacing);
        }

        private void SetSize(int size)
        {
            this.Size = size;
            this.ListEntities = new EntityBase[size];
        }

        private void InitializePlayer(PlayerData data, int pos, string direc)
        {
            var maxHpBonus = 0;
            var healCurrent = false;
            foreach (var effect in data.HeldItems.SelectMany(heldItem => heldItem.Effects))
            {
                if (effect is MaxHpBonusEffect maxHpEffect)
                {
                    maxHpBonus += maxHpEffect.Value;
                    if (maxHpEffect.HealCurrent) healCurrent = true;
                }
            }
            var effectiveMaxHp = data.MaxHp + maxHpBonus;
            var effectiveHp = data.Hp + (healCurrent ? maxHpBonus : 0);
            effectiveHp = Math.Min(effectiveHp, effectiveMaxHp);

            var player = new Player(effectiveHp, effectiveMaxHp);
            player.Facing = direc switch
            {
                "right" => EntityFacing.Right,
                "left" => EntityFacing.Left,
                "random" => Random.Range(0, 2) == 0 ? EntityFacing.Right : EntityFacing.Left,
                _ => throw new Exception("Unknown direction")
            };
            foreach (var effect in data.HeldItems.SelectMany(heldItem => heldItem.Effects))
            {
                if (effect is StartingBuffEffect buffEffect)
                {
                    foreach (var buffData in buffEffect.Buffs)
                    {
                        player.AddOrUpdateBuff(new EntityBuff(buffData));
                    }
                }
                else if (effect is StartingArmorEffect armorEffect)
                {
                    player.Armor += armorEffect.Value;
                }
            }
            
            this.ListEntities[pos] = player;
        }

        public Player GetPlayerFromMap()
        {
            return Array.Find(this.ListEntities, obj => obj is Player) as Player;
        }
        
        public int GetPlayerIndex()
        {
            return Array.FindIndex(this.ListEntities, e => e is Player);
        }
        
        public int GetEntityIndex(EntityBase entity)
        {
            return Array.FindIndex(this.ListEntities, e => e == entity);
        }

        public bool AddEntityToMap(EntityBase entity, int pos)
        {
            if (this.ListEntities[pos] == null)
            {
                this.ListEntities[pos] = entity;
                if (entity.HasValidFacing && entity.Facing == EntityFacing.Default)
                {
                    entity.Facing = FacingHelper.GetFacing(GetPlayerIndex() - pos);
                }
                if (entity is IActionableEntity actionableEntity)
                {
                    actionableEntity.InitializeBehaviors();
                }
                return true;
            }
            return false;
        }

        public bool RemoveEntityFromMap(EntityBase entity)
        {
            var index = Array.IndexOf(this.ListEntities, entity);
            if (index >= 0)
            {
                this.ListEntities[index] = null;
                return true;
            }
            return false;
        }

        public void SpawnEntitiesAtTurn(int turn)
        {
            var mobsToSpawn = this._config.Entities
                .Where(mob => mob.AppearTurn <= turn && !_spawnedEntities.Contains(mob))
                .GroupBy(mob => mob.AppearPos)
                .Select(group => group.First())
                .ToList();

            foreach (var mob in mobsToSpawn)
            {
                if (this.AddEntityToMap(mob.GenEntityBasedOnHpModifier(), mob.AppearPos))
                {
                    _spawnedEntities.Add(mob);
                }
            }
        }

        public EntityConfig[] GetIncomingEntities(int turn)
        {
            var ie = new EntityConfig[this.Size];
            this._config.Entities
                .Where(m => m.AppearTurn <= turn + 1 && !_spawnedEntities.Contains(m))
                .GroupBy(mob => mob.AppearPos)
                .Select(group => group.First())
                .ToList()
                .ForEach(m => ie[m.AppearPos] = m);
            return ie;
        }

        public bool TryMoveEntityStepByStep(int index, int moveStep)
        {
            if (index < 0 || index >= this.Size || ListEntities[index] == null)
                return false;

            var curPos = index;
            var direction = Math.Sign(moveStep);
            var steps = Math.Abs(moveStep);
            var moved = false;

            for (var i = 0; i < steps; i++)
            {
                var nextPos = curPos + direction;
                if (nextPos < 0 || nextPos >= this.Size || ListEntities[nextPos] != null)
                {
                    break;
                }

                ListEntities[nextPos] = ListEntities[curPos];
                ListEntities[curPos] = null;
                curPos = nextPos;
                moved = true;
            }

            return moved;
        }

        public bool AnyIncomingEnemyRemain()
        {
            return this._config.Entities
                .Any(e => !_spawnedEntities.Contains(e) &&
                          e.Type is "simple_enemy" or "elite_enemy" or "stationary_enemy");
        }
        
        public void CachePlayerPos()
        {
            var idx = GetPlayerIndex();
            PlayerPosCache = idx;
        }
    }
}