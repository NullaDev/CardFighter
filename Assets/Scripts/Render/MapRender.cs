using UnityEngine;

namespace Render
{
    public class MapRender : MonoBehaviour
    {
        public GameObject FloorPrefab;
        public GameObject FloorGrid;

        void Start()
        {
        
        }

        void Update()
        {
        
        }

        public void Render(int size)
        {
            for (int i = 0; i < size; i++)
            {
                GameObject.Instantiate(FloorPrefab, FloorGrid.transform);
            }
        }
    }
}
