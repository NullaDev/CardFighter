using GameLogic.Runtime;
using GameLogic.SaveLoad;
using SceneControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Render.Interact
{
    public class SaveDataInteract : MonoBehaviour, IPointerClickHandler
    {
        public int saveIndex = -1;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (saveIndex == -1) return;
            
            if (MiscData.Instance.InSavingMode)
            {
                SaveLoadFileControl.Save(saveIndex);
                
                var slcontrol = GameObject.Find("SaveLoadControl");
                slcontrol?.GetComponent<SaveLoadControl>()?.Rerender();
            }
            else
            {
                if (SaveLoadFileControl.HasSaveAt(saveIndex))
                {
                    if (SaveLoadFileControl.Load(saveIndex))
                    {
                        SceneManager.LoadScene("RogueMap");
                    }
                }
            }
        }
    }
}