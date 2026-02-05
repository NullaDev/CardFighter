using System;
using GameLogic.Runtime;
using GameLogic.SaveLoad;
using Render;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneControl
{
    public class SaveLoadControl : MonoBehaviour
    {
        public GameObject render;
        private const int SavePerPage = 8;
        private int _currentPageIndex = 0;

        private void Awake()
        {
            Rerender();
        }

        public void Rerender()
        {
            var saveLoadRender = render.GetComponent<SaveLoadRender>();
            saveLoadRender.RenderSaveData(_currentPageIndex, SavePerPage);
        }
        
        private int GetTotalPage()
        {
            return SaveLoadFileControl.MaxSaveCount / SavePerPage;
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
            if (MiscData.Instance.InSavingMode)
            {
                SceneManager.LoadScene("RogueMap");
            }
            else
            {
                if (MapData.Instance.Initialized)
                {
                    SceneManager.LoadScene("RogueMap");
                }
                else
                {
                    SceneManager.LoadScene("MainMenu");
                }
            }
        }
    }
}