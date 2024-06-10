using System.Collections.Generic;
using Card;
using Data;
using GameLogic;
using Render;
using Unity.VisualScripting;
using UnityEngine;

namespace FightingControl
{
    public class Fighting : MonoBehaviour
    {
        public Map Map;
        public MapRender MapRender;
        
        public int CurrentCost;
        public int MaxCost;
        public List<CardInstance> CurrentDeck;

        void Start()
        {
            var playerData = GetComponent<PlayerData>();
            this.CurrentDeck = playerData.DefaultDeck.CardList;
            this.MaxCost = playerData.MaxCost;
            this.CurrentCost = 1;

            // TODO remove hard code
            var stage = Resources.Load<TextAsset>("Stages/teststage");
            var config = StageConfig.CreateFromJson(stage.text);
            this.Map = new Map(config, playerData);
        }

        void Update()
        {
        
        }
    }
}
