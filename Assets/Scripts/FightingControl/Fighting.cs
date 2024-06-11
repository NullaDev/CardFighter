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
        
        public int CurrentCost;
        public int MaxCost;
        public List<CardInstance> CurrentDeck;

        void Start()
        {
            var playerData = PlayerData.Instance;
            // this.CurrentDeck = playerData.DefaultDeck.CardList;
            this.MaxCost = playerData.MaxCost;
            this.CurrentCost = 1;

            // TODO remove hard code
            var stage = Resources.Load<TextAsset>("Stages/teststage");
            var config = StageConfig.CreateFromJson(stage.text);
            Debug.Log(config);
            this.Map = new Map(config, playerData);

            var mapRender = GetComponent<MapRender>();
            mapRender.Render(this.Map.Size);
            
            var entityRender = GetComponent<EntityRender>();
            entityRender.Render(this.Map.ListEntities);
        }

        void Update()
        {
        
        }
    }
}
