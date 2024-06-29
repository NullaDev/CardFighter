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

        public int CurrentTurn;
        
        public int CurrentCost;
        public int MaxCost;
        public List<CardInstance> CurrentDeck;

        void Start()
        {
            this.CurrentCost = 0;
            
            var playerData = PlayerData.Instance;
            // this.CurrentDeck = playerData.DefaultDeck.CardList;
            this.MaxCost = playerData.MaxCost;
            this.CurrentCost = 1;

            // TODO remove hard code of loading stage
            var stage = Resources.Load<TextAsset>("Stages/teststage");
            var config = StageConfig.CreateFromJson(stage.text);
            Debug.Log("Loading stage config:" + stage.text);
            Debug.Log("Mob number:" + config.Mobs.Count);
            this.Map = new Map(config, playerData);

            MapRender = GetComponent<MapRender>();
            MapRender.RenderMap(this.Map);
            MapRender.RenderEntities(this.Map);
        }

        void Update()
        {
        
        }

        public void SettleTurn()
        {
            this.CurrentTurn += 1;
            this.Map.SpawnMobsAtTurn(CurrentTurn);
        }
    }
}
