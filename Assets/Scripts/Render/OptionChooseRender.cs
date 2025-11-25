using System;
using System.Collections.Generic;
using GameLogic;
using GameLogic.Runtime;
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
        
        public void Render()
        {
            var mapData = MapData.Instance;
            var playerData = PlayerData.Instance;
            
            foreach (var option in _listOptions)
            {
                GameObject.Destroy(option);
            }
            _listOptions.Clear();

            if (mapData.OptionBundle == null)
            {
                throw new Exception("Enter OptionChoose but OptionBundle is null");
            }
            
            OptionDescription.text = mapData.OptionBundle.Description;
            
            foreach (var option in mapData.OptionBundle.GetValidOptionsAccordingToPlayer(playerData, 3))
            {
                var optionButton = GameObject.Instantiate(OptionButtonPrefab, OptionButtonGrid.transform);
                optionButton.GetComponent<OptionButtonRender>().SetOptionAndRender(option);
            }
        }
    }
}