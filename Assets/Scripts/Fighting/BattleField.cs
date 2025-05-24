using System;
using System.Collections.Generic;
using System.Linq;
using Entity;
using GameLogic;
using JetBrains.Annotations;
using Registry;
using Registry.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Fighting
{
    public class BattleField
    {
        private StageConfig _config;
        public int Size;
        [ItemCanBeNull] public EntityBase[] ListEntities;
        private HashSet<EntityConfig> _spawnedEntities = new();

        public BattleField(StageConfig config, PlayerData pData)
        {
            this._config = config;
            this.SetSize(config.Size);
            this.InitializePlayer(pData, config.PlayerSpawnPos, config.PlayerSpawnFacing);
        }

        public void SetSize(int size)
        {
            this.Size = size;
            this.ListEntities = new EntityBase[size];
        }

        public void InitializePlayer(PlayerData data, int pos, string direc)
        {
            var player = new Player(data.Hp, data.MaxHp);
            player.Facing = direc switch
            {
                "right" => EntityFacing.RIGHT,
                "left" => EntityFacing.LEFT,
                "random" => Random.Range(0, 2) == 0 ? EntityFacing.RIGHT : EntityFacing.LEFT,
                _ => throw new Exception("Unknown direction")
            };
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
                if (entity is Enemy { Facing: EntityFacing.DEFAULT } enemy)
                {
                    enemy.Facing = FacingHelper.GetFacing(GetPlayerIndex() - pos);
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
                if (this.AddEntityToMap(mob.GenEntity(), mob.AppearPos))
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
    }
}