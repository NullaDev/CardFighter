using System;
using Data;
using Entity;

namespace Fighting
{
    public class Map
    {
        public int Size;
        public EntityBase[] ListEntities;

        public Map(int size)
        {
            SetSize(size);
        }

        public void SetSize(int size)
        {
            this.Size = size;
            this.ListEntities = new EntityBase[size];
        }

        public void InitializePlayer(PlayerData data, int pos)
        {
            Player player = new Player(data.MaxHP, data.MaxCost);
            this.ListEntities[pos] = player;
        }

        public Player GetPlayerFromList()
        {
            return Array.Find(this.ListEntities, obj => obj is Player) as Player;
        }

        public Boolean AddEntityToList(EntityBase entity, int pos)
        {
            if (this.ListEntities[pos] == null)
            {
                this.ListEntities[pos] = entity;
                return true;
            }
            else
            {
                // TODO
                return false;
            }
        }

        public Boolean RemoveEntityFromList(EntityBase entity)
        {
            int index = Array.IndexOf(this.ListEntities, entity);
            if (index >= 0)
            {
                this.ListEntities[index] = null;
                return true;
            }
            return false;
        }
    }
}