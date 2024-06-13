using System;
using Data;
using Entity;
using GameLogic;

namespace FightingControl
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
        }

        public void SetSize(int size)
        {
            this.Size = size;
            this.ListEntities = new EntityBase[size];
        }

        public void InitializePlayer(PlayerData data, int pos)
        {
            Player player = new Player(data.MaxHP);
            this.ListEntities[pos] = player;
        }

        public Player GetPlayerFromList()
        {
            return Array.Find(this.ListEntities, obj => obj is Player) as Player;
        }

        public bool AddEntityToMap(EntityBase entity, int pos)
        {
            if (this.ListEntities[pos] == null)
            {
                this.ListEntities[pos] = entity;
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
                    mob.AppearTurn >= turn && 
                    this.AddEntityToMap(mob.ToEnemyEntity(), mob.AppearPos)
                );
        }
    }
}