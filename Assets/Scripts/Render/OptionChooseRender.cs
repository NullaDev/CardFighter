using System.Collections.Generic;
using Registry;
using UnityEngine;

namespace Render
{
    public class OptionChooseRender : MonoBehaviour
    {
        public GameObject OptionButtonPrefab;
        public GameObject OptionButtonGrid;
        
        private readonly List<GameObject> _listOptions = new();
        
        public void Render(PlayerData playerData)
        {
            foreach (var option in _listOptions)
            {
                GameObject.Destroy(option);
            }
            _listOptions.Clear();
            
            if (playerData.Options.Count > 3) return;

            foreach (var option in playerData.Options)
            {
                var optionButton = GameObject.Instantiate(OptionButtonPrefab, OptionButtonGrid.transform);
                optionButton.GetComponent<OptionButtonRender>().SetOptionAndRender(option);
            }
        }
    }
}