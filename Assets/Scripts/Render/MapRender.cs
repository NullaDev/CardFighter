using System.Collections.Generic;
using FightingControl;
using UnityEngine;

namespace Render
{
    public class MapRender : MonoBehaviour
    {
        public GameObject FloorPrefab;
        public GameObject FloorGrid;

        private List<GameObject> _listFloors = new();

        void Start()
        {
        
        }

        void Update()
        {
        
        }

        public void Render(Map map)
        {
            if (_listFloors.Count != map.Size)
            {
                foreach (var floor in _listFloors)
                {
                    GameObject.Destroy(floor);
                }
                for (var i = 0; i < map.Size; i++)
                {
                    GameObject.Instantiate(FloorPrefab, FloorGrid.transform);
                }
            }
        }
    }
}
