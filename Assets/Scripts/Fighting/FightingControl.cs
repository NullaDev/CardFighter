using Data;
using GameLogic;
using Render;
using UnityEngine;

namespace Fighting
{
    public class FightingControl : MonoBehaviour
    {
        public Map Map;
        public MapRender MapRender;

        public FightingData FightingData;

        void Start()
        {
            var playerData = PlayerData.Instance;
            this.FightingData = FightingData.FromPlayerData(playerData);

            // TODO remove hard code of loading stage
            var stage = Resources.Load<TextAsset>("Stages/teststage");
            var config = StageConfig.CreateFromJson(stage.text);
            Debug.Log("Loading stage config:" + stage.text);
            Debug.Log("Mob number:" + config.Mobs.Count);
            this.Map = new Map(config, playerData);

            this.MapRender = GetComponent<MapRender>();
            this.MapRender.RenderMap(this.Map);
            this.MapRender.RenderEntities(this.Map);
        }

        void Update()
        {
        
        }

        public void SettleTurn()
        {
            this.FightingData.CurrentTurn += 1;
            this.Map.SpawnMobsAtTurn(this.FightingData.CurrentTurn);
            this.MapRender.RenderEntities(this.Map);
        }
        
        public void UpdatePlayerData()
        {
            var playerData = PlayerData.Instance;
            var player = this.Map.GetPlayerFromMap();
            playerData.HP = player.HP;
        }
    }
}
