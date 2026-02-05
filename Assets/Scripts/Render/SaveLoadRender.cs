using System;
using System.Collections.Generic;
using GameLogic.SaveLoad;
using Render.Interact;
using UnityEngine;
using UnityEngine.UI;

namespace Render
{
    public class SaveLoadRender : MonoBehaviour
    {
        public GameObject SaveDataPrefab;
        public GameObject SaveGrid;
        
        private readonly List<GameObject> _listSaveData = new();
        
        public void RenderSaveData(int pageIndex, int pageSize)
        {
            foreach (var card in _listSaveData)
            {
                GameObject.Destroy(card);
            }
            _listSaveData.Clear();

            var startIndex = pageIndex * pageSize;
            var endIndex = Math.Min(startIndex + pageSize, SaveLoadFileControl.MaxSaveCount);

            for (var i = startIndex; i < endIndex; i++)
            {
                var saveData = GameObject.Instantiate(SaveDataPrefab, SaveGrid.transform);
                var saveDataInteract = saveData.GetComponent<SaveDataInteract>();
                saveDataInteract.saveIndex = i;
                
                var saveDataText = saveData.transform.Find("Text").GetComponent<Text>();
                if (SaveLoadFileControl.HasSaveAt(i))
                {
                    saveDataText.text = SaveLoadFileControl.GetSaveSummary(i).ToDisplayText();
                }
                else
                {
                    saveDataText.text = $"存档编号：{i}\n无存档";
                }
                
                _listSaveData.Add(saveData);
            }
        }
    }
}