using Card;
using Data;
using GameLogic;
using Render;
using UnityEngine;

namespace Fighting
{
    public class FightingControl : MonoBehaviour
    {
        public GameObject render;
        private Map _map;
        private FightingData _fightingData;

        void Start()
        {
            var playerData = PlayerData.Instance;
                        
            // TODO remove hard code of initializing player data
            playerData.PlayerClass = PlayerClass.FIGHTER;
            playerData.MaxHp = playerData.Hp = 10;
            playerData.InitialCost = 1;
            playerData.MaxCost = 5;

            playerData.DefaultDeck = new Deck(playerData.PlayerClass);
            playerData.DefaultDeck.AddPrototype(CardData.Instance.Find("move"));
            playerData.DefaultDeck.AddPrototype(CardData.Instance.Find("punch"));
            playerData.DefaultDeck.AddPrototype(CardData.Instance.Find("turn_back"));
            
            this._fightingData = FightingData.FromPlayerData(playerData);

            // TODO remove hard code of loading stage
            var stage = Resources.Load<TextAsset>("Stages/teststage");
            var config = StageConfig.CreateFromJson(stage.text);
            Debug.Log("Loading stage config:" + stage.text);
            Debug.Log("Mob number:" + config.Mobs.Count);
            this._map = new Map(config, playerData);
            
            var uiRender = render.GetComponent<UIRender>();
            uiRender.RenderTurn(0);
            uiRender.RenderCost(this._fightingData.CurrentCost, this._fightingData.MaxCost);

            var mapRender = render.GetComponent<MapRender>();
            mapRender.RenderMap(this._map);
            mapRender.RenderEntities(this._map);

            var playerCardsRender = render.GetComponent<PlayerCardsRender>();
            playerCardsRender.RenderCards(this._fightingData.CurrentDeck);
        }

        void Update()
        {
        
        }

        public void NextTurn()
        {
            this._fightingData.CurrentTurn += 1;
            this._map.SpawnMobsAtTurn(this._fightingData.CurrentTurn);
            
            this._fightingData.AddCost(1);
            
            var uiRender = render.GetComponent<UIRender>();
            uiRender.RenderTurn(this._fightingData.CurrentTurn);
            uiRender.RenderCost(this._fightingData.CurrentCost, this._fightingData.MaxCost);
            
            var mapRender = render.GetComponent<MapRender>();
            mapRender.RenderEntities(this._map);
        }

        public void PlayerUseCard(CardInstance card)
        {
            card.Effects.ForEach(effect=>effect(this._map));
            this._fightingData.CurrentCost -= card.CurrentCost;
            NextTurn();
        }
        
        public void UpdatePlayerData()
        {
            var playerData = PlayerData.Instance;
            var player = this._map.GetPlayerFromMap();
            playerData.Hp = player.HP;
        }
    }
}
