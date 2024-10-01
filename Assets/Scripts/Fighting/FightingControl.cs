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
        public Map Map { get; private set; }
        public FightingData FightingData { get; private set; }

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
            playerData.DefaultDeck.AddPrototype(CardData.Instance.Find("turn_back"));
            playerData.DefaultDeck.AddPrototype(CardData.Instance.Find("punch"));
            playerData.DefaultDeck.AddPrototype(CardData.Instance.Find("focus_energy"));
            
            playerData.DefaultDeck.AddPrototype(CardData.Instance.Find("kick"));
            playerData.DefaultDeck.AddPrototype(CardData.Instance.Find("spear"));
            
            this.FightingData = FightingData.FromPlayerData(playerData);

            // TODO remove hard code of loading stage
            var stage = Resources.Load<TextAsset>("Stages/teststage");
            var config = StageConfig.CreateFromJson(stage.text);
            Debug.Log("Loading stage config:" + stage.text);
            Debug.Log("Mob number:" + config.Mobs.Count);
            this.Map = new Map(config, playerData);
            
            var uiRender = render.GetComponent<UIRender>();
            uiRender.RenderTurn(0);
            uiRender.RenderCost(this.FightingData.CurrentCost, this.FightingData.MaxCost);

            var mapRender = render.GetComponent<MapRender>();
            mapRender.RenderMap(this.Map);
            mapRender.RenderEntities(this.Map);
            mapRender.RenderIncomingEntities(this.Map, this.FightingData.CurrentTurn);

            var playerCardsRender = render.GetComponent<PlayerCardsRender>();
            playerCardsRender.RenderCards(this.FightingData.CurrentDeck);
        }

        void Update()
        {
        
        }

        public void NextTurn()
        {
            this.FightingData.CurrentTurn += 1;
            this.Map.SpawnMobsAtTurn(this.FightingData.CurrentTurn);
            
            this.FightingData.TryAddCost(1);
            
            var uiRender = render.GetComponent<UIRender>();
            uiRender.RenderTurn(this.FightingData.CurrentTurn);
            uiRender.RenderCost(this.FightingData.CurrentCost, this.FightingData.MaxCost);
            
            var mapRender = render.GetComponent<MapRender>();
            mapRender.RenderEntities(this.Map);
            mapRender.RenderIncomingEntities(this.Map, this.FightingData.CurrentTurn);
        }

        public void PlayerUseCard(CardInstance card)
        {
            if (this.FightingData.CurrentCost < card.CurrentCost) return;
            
            this.FightingData.CurrentCost -= card.CurrentCost;
            card.Effects.ForEach(effect=>effect(this));
            NextTurn();
        }
        
        public void UpdatePlayerData()
        {
            var playerData = PlayerData.Instance;
            var player = this.Map.GetPlayerFromMap();
            playerData.Hp = player.HP;
        }
    }
}
