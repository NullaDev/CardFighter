using System.Collections.Generic;
using GameLogic.Item;
using Render.Component;
using UnityEngine;

namespace Render
{
    public class ItemListRender: MonoBehaviour
    {
        public GameObject ItemPrefab;
        public GameObject ItemGrid;
        
        private readonly List<GameObject> _listItems = new();
        
        public void RenderItems(List<HeldItem> items)
        {
            foreach (var card in _listItems)
            {
                GameObject.Destroy(card);
            }
            _listItems.Clear();
            
            foreach (var item in items)
            {
                var itemObject = GameObject.Instantiate(ItemPrefab, ItemGrid.transform);
                var itemRender = itemObject.GetComponent<HeldItemRender>();
                itemRender.RenderItem(item);
                _listItems.Add(itemObject);
            }
        }
    }
}