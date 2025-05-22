using System.Linq;
using Card;
using Entity;
using GameLogic;
using Registry;
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
            this.FightingData = FightingData.FromPlayerData(playerData);
            
            this.BattleField = new BattleField(playerData.CurrentStage, playerData);
            this.BattleField.SpawnEntitiesAtTurn(0);
            EntitiesThink();
            
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
            mapRender.RenderBuff(this.BattleField);
            mapRender.RenderBattleField(this.BattleField);
            mapRender.RenderEntities(this.BattleField);
            mapRender.RenderIncomingEntities(this.BattleField, this.FightingData.CurrentTurn);

            var playerCardsRender = this.render.GetComponent<DeckRender>();
            playerCardsRender.RenderCards(this.FightingData.AvailableCards, this.BattleField.GetPlayerFromMap());
        }

        public void PlayerUseCard(CardInstance card)
        {
            var player = this.BattleField.GetPlayerFromMap();
            if (player.HasBuff(EntityBuffManager.Stunned))
            {
                EndTurn();
                return;
            }
            if (this.FightingData.CurrentCost < card.GetCurrentCost(player)) return;
            
            this.FightingData.CurrentCost -= card.GetCurrentCost(player);
            if (player.HasBuff(EntityBuffManager.Practice))
            {
                var times = player.GetBuff(EntityBuffManager.Practice).GetParam<int>(EntityBuffManager.PracticeValue);
                foreach (var _ in Enumerable.Range(0, times))
                {
                    card.Effects.ForEach(effect=>effect(this, player));
                }
            }
            else
            {
                card.Effects.ForEach(effect=>effect(this, player));
            }
            EndTurn();
        }

        public void EndTurn()
        {
            NextTurn();
            ResolveEntityActions();
            UpdatePlayerStatus();
            EntitiesThink();
            Rerender();
        }
        
        public void NextTurn()
        {
            this.FightingData.CurrentTurn += 1;
            this.BattleField.SpawnEntitiesAtTurn(this.FightingData.CurrentTurn);
        }

        public void ResolveEntityActions()
        {
            var enemyRemain = false;
            var listEntitiesSnapshot = (EntityBase[])this.BattleField.ListEntities.Clone();
            for (var i = 0; i < this.BattleField.Size; i++)
            {
                var entity = listEntitiesSnapshot[i];
                if (entity == null || entity.IsDead || entity is Player)
                    continue;
                
                if (entity is Enemy enemy)
                {
                    if (!enemy.HasBuff(EntityBuffManager.Stunned))
                        enemy.NextTurnCard?.Effects.ForEach(effect=>effect(this, enemy));
                    if (!enemy.IsDead)
                        enemyRemain = true;
                }
                
                entity.UpdateBuffs();
            }
            if (!enemyRemain && !this.BattleField.AnyIncomingEnemyRemain())
            {
                // TODO win
                Debug.Log("win");
                SceneManager.LoadScene("RogueMap");
            }
        }

        public void UpdatePlayerStatus()
        {
            var player = this.BattleField.GetPlayerFromMap();
            player.UpdateBuffs();
            this.FightingData.UpdatePlayerDeck(player);
            this.FightingData.TryAddCost(1);
        }
        
        public void EntitiesThink()
        {
            for (var i = 0; i < this.BattleField.Size; i++)
            {
                var entity = this.BattleField.ListEntities[i];
                if (entity is Enemy enemy)
                {
                    enemy.NextTurnCard = enemy.ThinkNextTurnCard(this.BattleField);
                }
            }
        }
        
        public void UpdatePlayerData()
        {
            var playerData = PlayerData.Instance;
            var player = this.BattleField.GetPlayerFromMap();
            playerData.Hp = player.HP;
        }
    }
}
