using System;
using Data;
using Entity;
using GameLogic;
using UnityEngine;

namespace Fighting
{
    public class Map
    {
        private StageConfig _config;
        public int Size;
        public EntityBase[] ListEntities;

        public Map(StageConfig config, PlayerData pData)
        {
            this._config = config;
            this.SetSize(config.Size);
            this.InitializePlayer(pData, config.PlayerSpawnPos);
            this.SpawnMobsAtTurn(0);
        }

        public void SetSize(int size)
        {
            this.Size = size;
            this.ListEntities = new EntityBase[size];
        }

        public void InitializePlayer(PlayerData data, int pos)
        {
            Player player = new Player(data.maxHP);
            this.ListEntities[pos] = player;
        }

        public Player GetPlayerFromMap()
        {
            return Array.Find(this.ListEntities, obj => obj is Player) as Player;
        }
        
        public int GetPlayerIndex()
        {
            return Array.FindIndex(this.ListEntities, entity => entity is Player);
        }

        public bool AddEntityToMap(EntityBase entity, int pos)
        {
            if (this.ListEntities[pos] == null)
            {
                this.ListEntities[pos] = entity;
                if (entity is Enemy)
                {
                    entity.Facing = FacingHelper.GetFacing(GetPlayerIndex() - pos);
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

        public void SpawnMobsAtTurn(int turn)
        {
            this._config.Mobs.RemoveAll(
                mob => 
                    mob.AppearTurn <= turn && 
                    this.AddEntityToMap(mob.ToEnemyEntity(), mob.AppearPos)
                );
        }
    }
}