using System.Collections.Generic;
using Entity;
using Fighting;
using UnityEngine;

namespace Render
{
    public class MapRender : MonoBehaviour
    {
        public GameObject FloorPrefab;
        public GameObject FloorGrid;
        public GameObject EntityPrefab;
        public GameObject EntityGrid;
        public GameObject IncomingEntityPrefab;
        public GameObject IncomingEntityGrid;

        private GameObject[] _listFloors = {};
        private GameObject[] _listEntities = {};
        private GameObject[] _listIncomingEntities = {};

        public void RenderMap(Map map)
        {
            if (_listFloors.Length != map.Size)
            {
                foreach (var floor in _listFloors)
                {
                    GameObject.Destroy(floor);
                }

                _listFloors = new GameObject[map.Size];
                for (var i = 0; i < map.Size; i++)
                {
                    _listFloors[i] = GameObject.Instantiate(FloorPrefab, FloorGrid.transform);
                }
            }
        }

        public void RenderEntities(Map map)
        {
            if (_listEntities.Length != map.Size)
            {
                foreach (var entity in _listEntities)
                {
                    GameObject.Destroy(entity);
                }
                
                _listEntities = new GameObject[map.Size];
                for (var i = 0; i < map.Size; i++)
                {
                    var entity = GameObject.Instantiate(EntityPrefab, EntityGrid.transform);
                    _listEntities[i] = entity;
                }
            }
            for (var i = 0; i < map.Size; i++)
            {
                if (map.ListEntities[i] != null)
                {
                    _listEntities[i].GetComponent<EntityRender>().RenderEntity(map.ListEntities[i]);
                }
                else
                {
                    _listEntities[i].GetComponent<EntityRender>().RenderEmpty();
                }
            }
        }

        public void RenderIncomingEntities(Map map, int turn)
        {
            var entityList = map.GetIncomingEntities(turn);
            if (_listIncomingEntities.Length != map.Size)
            {
                foreach (var entity in _listIncomingEntities)
                {
                    GameObject.Destroy(entity);
                }
                
                _listIncomingEntities = new GameObject[map.Size];
                for (var i = 0; i < map.Size; i++)
                {
                    var entity = GameObject.Instantiate(IncomingEntityPrefab, IncomingEntityGrid.transform);
                    _listIncomingEntities[i] = entity;
                }
            }
            for (var i = 0; i < map.Size; i++)
            {
                if (entityList[i] != null)
                {
                    _listIncomingEntities[i].GetComponent<IncomingEntityRender>().RenderEntity(entityList[i]);
                }
                else
                {
                    _listIncomingEntities[i].GetComponent<IncomingEntityRender>().RenderEmpty();
                }
            }
        }
    }
}
