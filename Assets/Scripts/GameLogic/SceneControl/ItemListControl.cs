using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic.Runtime;
using Item;
using Render;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic.SceneControl
{
    public class ItemListControl: MonoBehaviour
    {
        public GameObject render;
        private const int ItemPerPage = 12;
        private int _currentPageIndex = 0;
        private List<HeldItem> _heldItems;

        private void Awake()
        {
            var playerData = PlayerData.Instance;
            _heldItems = new List<HeldItem>(playerData.HeldItems);
            Rerender();
        }
        
        private void Rerender()
        {
            var itemRender = render.GetComponent<ItemListRender>();
            itemRender.RenderItems(GetCurrentPageItems());
        }
        
        private List<HeldItem> GetCurrentPageItems()
        {
            var startIndex = _currentPageIndex * ItemPerPage;
            var pageItems = _heldItems.Skip(startIndex).Take(ItemPerPage).ToList();
            return pageItems;
        }
        
        private int GetTotalPage()
        {
            return (int)Math.Ceiling((double)this._heldItems.Count / ItemPerPage);
        }
        
        public void NextPage()
        {
            this._currentPageIndex = Math.Min(this._currentPageIndex + 1, GetTotalPage() - 1);
            Rerender();
        }
        
        public void PreviousPage()
        {
            this._currentPageIndex = Math.Max(this._currentPageIndex - 1, 0);
            Rerender();
        }
        
        public void Return()
        {
            SceneManager.LoadScene("RogueMap");
        }

    }
}