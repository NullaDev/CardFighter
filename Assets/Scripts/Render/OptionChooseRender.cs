using System;
using System.Collections.Generic;
using Registry;
using UnityEngine;
using UnityEngine.UI;

namespace Render
{
    public class OptionChooseRender : MonoBehaviour
    {
        public Text OptionDescription;
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

            if (playerData.OptionBundle == null)
            {
                throw new Exception("Enter OptionChoose but OptionBundle is null");
            }
            
            OptionDescription.text = playerData.OptionBundle.Description;
            
            foreach (var option in playerData.OptionBundle.GetOptions(playerData, 3))
            {
                var optionButton = GameObject.Instantiate(OptionButtonPrefab, OptionButtonGrid.transform);
                optionButton.GetComponent<OptionButtonRender>().SetOptionAndRender(option);
            }
        }
    }
}