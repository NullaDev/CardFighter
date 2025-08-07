using System;
using System.Linq;
using Card;
using GameLogic.Buff;
using GameLogic.Entity;
using GameLogic.RogueMap;
using Registry;
using Render;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic.SceneControl
{
    public class FightingControl : MonoBehaviour
    {
        public GameObject render;
        public BattleField BattleField { get; private set; }
        public FightingData FightingData { get; private set; }

        public void Start()
        {
            var playerData = PlayerData.Instance;
            
            if (playerData.CurrentStage == null)
            {
                throw new Exception("Enter Battle but CurrentStage is null");
            }
            
            this.BattleField = new BattleField(playerData.CurrentStage, playerData);
            this.BattleField.SpawnEntitiesAtTurn(0);
            this.FightingData = FightingData.FromPlayerData(playerData);
            
            EntitiesThink();
            
            Rerender();
        }

        private void Rerender()
        {
            var uiRender = this.render.GetComponent<FightingUIRender>();
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
                var times = player.GetBuff(EntityBuffManager.Practice).GetMiscParam<int>(EntityBuffManager.PracticeValue);
                foreach (var _ in Enumerable.Range(0, times))
                {
                    card.Effects.ForEach(effect=>effect(this, player));
                }
            }
            else
            {
                card.Effects.ForEach(effect=>effect(this, player));
            }
            
            if (player.HasBuff(EntityBuffManager.Initiative))
            {
                var buff = player.GetBuff(EntityBuffManager.Initiative);
                player.Buffs.Remove(buff);
                UpdatePlayerStatus(false);
                Rerender();
                return;
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
                
                entity.UpdateStatusAndBuffs();
            }
            if (!enemyRemain && !this.BattleField.AnyIncomingEnemyRemain())
            {
                UpdatePlayerData();
                SceneManager.LoadScene("OptionChoose");
            }
        }

        public void UpdatePlayerStatus(bool shouldAddCost=true)
        {
            var player = this.BattleField.GetPlayerFromMap();
            player.UpdateStatusAndBuffs();
            this.FightingData.UpdatePlayerDeck(player);
            if (shouldAddCost)
                this.FightingData.TryAddCost(1);
        }
        
        public void EntitiesThink()
        {
            for (var i = 0; i < this.BattleField.Size; i++)
            {
                var entity = this.BattleField.ListEntities[i];
                if (entity is Enemy enemy)
                {
                    enemy.NextTurnCard = enemy.ThinkNextTurnCard(this);
                }
            }
        }
        
        public void UpdatePlayerData()
        {
            var playerData = PlayerData.Instance;
            var player = this.BattleField.GetPlayerFromMap();
            playerData.Hp = player.HP;

            var optionName = playerData.CurrentNodeType switch
            {
                NodeType.FIGHT => $"normal_fight_bonus_{playerData.CurrentLayerDifficulty/2}",
                NodeType.ELITE_FIGHT => $"elite_fight_bonus_{playerData.CurrentLayerDifficulty/4}",
                NodeType.BOSS => "boss_fight_bonus",
                _ => ""
            };
            playerData.OptionBundle = StaticDataManager.OptionDataManager.GetBundle(optionName);
        }
    }
}
