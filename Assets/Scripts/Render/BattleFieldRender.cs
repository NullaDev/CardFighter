using System.Collections.Generic;
using Entity;
using Fighting;
using UnityEngine;

namespace Render
{
    public class BattleFieldRender : MonoBehaviour
    {
        public GameObject BuffPrefab;
        public GameObject BuffGrid;
        
        public GameObject FloorPrefab;
        public GameObject FloorGrid;
        public GameObject EntityPrefab;
        public GameObject EntityGrid;
        public GameObject IncomingEntityPrefab;
        public GameObject IncomingEntityGrid;

        private List<GameObject> _listBuffs = new();
        private GameObject[] _listFloors = {};
        private GameObject[] _listEntities = {};
        private GameObject[] _listIncomingEntities = {};

        public void RenderBuff(BattleField battleField)
        {
            foreach (var buff in _listBuffs)
            {
                GameObject.Destroy(buff);
            }
            _listBuffs.Clear();
            
            var player = battleField.GetPlayerFromMap();
            foreach (var playerBuff in player.Buffs)
            {
                var buff = GameObject.Instantiate(BuffPrefab, BuffGrid.transform);
                var buffRender = buff.GetComponent<BuffRender>();
                buffRender.RenderBuff(playerBuff);
                _listBuffs.Add(buff);
            }
        }

        public void RenderBattleField(BattleField battleField)
        {
            if (_listFloors.Length != battleField.Size)
            {
                foreach (var floor in _listFloors)
                {
                    GameObject.Destroy(floor);
                }

                _listFloors = new GameObject[battleField.Size];
                for (var i = 0; i < battleField.Size; i++)
                {
                    _listFloors[i] = GameObject.Instantiate(FloorPrefab, FloorGrid.transform);
                }
            }
        }

        public void RenderEntities(BattleField battleField)
        {
            if (_listEntities.Length != battleField.Size)
            {
                foreach (var entity in _listEntities)
                {
                    GameObject.Destroy(entity);
                }
                
                _listEntities = new GameObject[battleField.Size];
                for (var i = 0; i < battleField.Size; i++)
                {
                    var entity = GameObject.Instantiate(EntityPrefab, EntityGrid.transform);
                    _listEntities[i] = entity;
                }
            }
            for (var i = 0; i < battleField.Size; i++)
            {
                if (battleField.ListEntities[i] != null)
                {
                    _listEntities[i].GetComponent<EntityRender>().RenderEntity(battleField.ListEntities[i]);
                }
                else
                {
                    _listEntities[i].GetComponent<EntityRender>().RenderEmpty();
                }
            }
        }

        public void RenderIncomingEntities(BattleField battleField, int turn)
        {
            var entityList = battleField.GetIncomingEntities(turn);
            if (_listIncomingEntities.Length != battleField.Size)
            {
                foreach (var entity in _listIncomingEntities)
                {
                    GameObject.Destroy(entity);
                }
                
                _listIncomingEntities = new GameObject[battleField.Size];
                for (var i = 0; i < battleField.Size; i++)
                {
                    var entity = GameObject.Instantiate(IncomingEntityPrefab, IncomingEntityGrid.transform);
                    _listIncomingEntities[i] = entity;
                }
            }
            for (var i = 0; i < battleField.Size; i++)
            {
                if (entityList[i] != null)
                {
                    _listIncomingEntities[i].GetComponent<IncomingEntityRender>().RenderEntity(entityList[i], battleField);
                }
                else
                {
                    _listIncomingEntities[i].GetComponent<IncomingEntityRender>().RenderEmpty();
                }
            }
        }
    }
}
