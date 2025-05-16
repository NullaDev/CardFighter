using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Entity;
using GameLogic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Fighting
{
    public class BattleField
    {
        private StageConfig _config;
        public int Size;
        [ItemCanBeNull] public EntityBase[] ListEntities;

        public BattleField(StageConfig config, PlayerData pData)
        {
            this._config = config;
            this.SetSize(config.Size);
            this.InitializePlayer(pData, config.PlayerSpawnPos, config.PlayerSpawnFacing);
            this.SpawnEntitiesAtTurn(0);
            this.EntitiesThink();
        }

        public void SetSize(int size)
        {
            this.Size = size;
            this.ListEntities = new EntityBase[size];
        }

        public void InitializePlayer(PlayerData data, int pos, string direc)
        {
            var player = new Player(data.MaxHp);
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
                .Where(mob => mob.AppearTurn <= turn)
                .GroupBy(mob => mob.AppearPos)
                .Select(group => group.First())
                .ToList();
            
            this._config.Entities.RemoveAll(
                entity => 
                    mobsToSpawn.Contains(entity) && 
                    this.AddEntityToMap(entity.GenEntity(), entity.AppearPos)
                );
        }

        public EntityConfig[] GetIncomingEntities(int turn)
        {
            var ie = new EntityConfig[this.Size];
            this._config.Entities
                .Where(m => m.AppearTurn <= turn + 1)
                .GroupBy(mob => mob.AppearPos)
                .Select(group => group.First())
                .ToList()
                .ForEach(
                    m=>ie[m.AppearPos]=m
                );
            return ie;
        }

        public bool TryKnockBackEntity(int index, int knockBack)
        {
            if (index < 0 || index >= this.Size || ListEntities[index] == null)
                return false;

            var curPos = index;
            var direction = Math.Sign(knockBack);
            var steps = Math.Abs(knockBack);
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
        
        public void EntitiesThink()
        {
            for (var i = 0; i < this.Size; i++)
            {
                var entity = this.ListEntities[i];
                if (entity is Enemy enemy)
                {
                    enemy.NextTurnCard = enemy.ThinkNextTurnCard(this);
                }
            }
        }

        public bool AnyIncomingEnemyRemain()
        {
            return this._config.Entities.Any(e=>e.Type is "simple_enemy" or "elite_enemy");
        }
    }
}