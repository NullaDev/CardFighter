using Card;
using Data;
using Entity;
using GameLogic;
using Render;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fighting
{
    public class FightingControl : MonoBehaviour
    {
        public GameObject render;
        public BattleField BattleField { get; private set; }
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
            
            this.BattleField = new BattleField(playerData.CurrentStage, playerData);

            Rerender();
        }

        void Update()
        {
        
        }

        void Rerender()
        {
            var uiRender = this.render.GetComponent<UIRender>();
            uiRender.RenderTurn(this.FightingData.CurrentTurn);
            uiRender.RenderCost(this.FightingData.CurrentCost, this.FightingData.MaxCost);

            var mapRender = this.render.GetComponent<BattleFieldRender>();
            mapRender.RenderBattleField(this.BattleField);
            mapRender.RenderEntities(this.BattleField);
            mapRender.RenderIncomingEntities(this.BattleField, this.FightingData.CurrentTurn);

            var playerCardsRender = this.render.GetComponent<DeckRender>();
            playerCardsRender.RenderCards(this.FightingData.CurrentDeck);
        }

        public void PlayerUseCard(CardInstance card)
        {
            if (this.FightingData.CurrentCost < card.CurrentCost) return;
            
            this.FightingData.CurrentCost -= card.CurrentCost;
            card.Effects.ForEach(effect=>effect(this, this.BattleField.GetPlayerFromMap()));
            EndTurn();
        }

        public void EndTurn()
        {
            NextTurn();

            var enemyRemain = false;
            for (var i = 0; i < this.BattleField.Size; i++)
            {
                var entity = this.BattleField.ListEntities[i];
                if (entity is Enemy enemy)
                {
                    enemy.NextTurnCard?.Effects.ForEach(effect=>effect(this, enemy));
                    enemyRemain = true;
                }
            }

            if (!enemyRemain)
            {
                if (!this.BattleField.AnyIncomingEnemyRemain())
                {
                    Debug.Log("win");
                    SceneManager.LoadScene("RogueMap");
                }
            }
            
            this.BattleField.EntitiesThink();
            Rerender();
        }
        
        public void NextTurn()
        {
            this.FightingData.CurrentTurn += 1;
            this.BattleField.SpawnEntitiesAtTurn(this.FightingData.CurrentTurn);
            
            this.FightingData.TryAddCost(1);
        }
        
        public void UpdatePlayerData()
        {
            var playerData = PlayerData.Instance;
            var player = this.BattleField.GetPlayerFromMap();
            playerData.Hp = player.HP;
        }
    }
}
