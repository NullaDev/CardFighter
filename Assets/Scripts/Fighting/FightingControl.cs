using Data;
using GameLogic;
using Render;
using UnityEngine;

namespace Fighting
{
    public class FightingControl : MonoBehaviour
    {
        public GameObject Render;
        private Map _map;
        private FightingData _fightingData;

        void Start()
        {
            var playerData = PlayerData.Instance;
            this._fightingData = FightingData.FromPlayerData(playerData);

            // TODO remove hard code of loading stage
            var stage = Resources.Load<TextAsset>("Stages/teststage");
            var config = StageConfig.CreateFromJson(stage.text);
            Debug.Log("Loading stage config:" + stage.text);
            Debug.Log("Mob number:" + config.Mobs.Count);
            this._map = new Map(config, playerData);
            
            var uiRender = Render.GetComponent<UIRender>();
            uiRender.RenderTurn(0);

            var mapRender = Render.GetComponent<MapRender>();
            mapRender.RenderMap(this._map);
            mapRender.RenderEntities(this._map);
        }

        void Update()
        {
        
        }

        public void NextTurn()
        {
            this._fightingData.CurrentTurn += 1;
            this._map.SpawnMobsAtTurn(this._fightingData.CurrentTurn);
            
            var uiRender = Render.GetComponent<UIRender>();
            uiRender.RenderTurn(this._fightingData.CurrentTurn);
            
            var mapRender = Render.GetComponent<MapRender>();
            mapRender.RenderEntities(this._map);
        }

        public void PlayerTurnBack()
        {
            var player = this._map.GetPlayerFromMap();
            player.Facing = player.Facing == EntityFacing.LEFT ? EntityFacing.RIGHT : EntityFacing.LEFT;
            NextTurn();
        }
        
        public void UpdatePlayerData()
        {
            var playerData = PlayerData.Instance;
            var player = this._map.GetPlayerFromMap();
            playerData.HP = player.HP;
        }
    }
}
