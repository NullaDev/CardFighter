using System;
using System.Linq;
using GameLogic;
using GameLogic.Buff;
using GameLogic.Card;
using GameLogic.Entity;
using GameLogic.Map;
using GameLogic.Runtime;
using Registry;
using Render;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneControl
{
    public class FightingControl : MonoBehaviour
    {
        public GameObject render;
        public BattleField BattleField { get; private set; }
        public FightingData FightingData { get; private set; }

        public void Start()
        {
            var mapData = MapData.Instance;
            var playerData = PlayerData.Instance;
            
            if (mapData.CurrentStageConfig == null)
            {
                throw new Exception("Enter Battle but CurrentStage is null");
            }
            
            this.BattleField = new BattleField(mapData.CurrentStageConfig, playerData);
            this.BattleField.SpawnEntitiesAtTurn(0);
            this.FightingData = FightingData.FromPlayerData(playerData);

            BattleField.CachePlayerPos();
            EntitiesThink();
            
            Rerender();
        }

        private void Rerender()
        {
            var uiRender = this.render.GetComponent<FightingUIRender>();
            uiRender.RenderTurn(this.FightingData.CurrentTurn);
            uiRender.RenderCost(this.FightingData.CurrentCost, this.FightingData.MaxCost);
            uiRender.RenderModifier(MapData.Instance.CurrentMapAttackModifier);

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
            BattleField.CachePlayerPos();
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
                if (entity is null or Player)
                    continue;
                
                if (entity is IActionableEntity actionableEntity && !entity.IsDead)
                {
                    if (!entity.HasBuff(EntityBuffManager.Stunned))
                        actionableEntity.NextTurnCard?.Effects.ForEach(effect=>effect(this, entity));
                }
                
                entity.UpdateStatusAndBuffs();
                if (entity.IsDead)
                {
                    this.BattleField.RemoveEntityFromMap(entity);
                } else if (entity is Enemy)
                {
                    enemyRemain = true;
                }
            }
            if (!enemyRemain && !this.BattleField.AnyIncomingEnemyRemain())
            {
                SettleCurrentMap();
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
                if (entity is IActionableEntity actionableEntity)
                {
                    actionableEntity.NextTurnCard = actionableEntity.ThinkNextTurnCard(this);
                }
            }
        }

        public void SettleCurrentMap()
        {
            var mapData = MapData.Instance;
            if (!mapData.Initialized)  // Enter from debug mode
            {
                SceneManager.LoadScene("MainMenu");
                return;
            }
            if (!mapData.HasNextLayer())
            {
                if (mapData.HasNextMap())
                {
                    mapData.MoveToNextMap();
                }
                else
                {
                    // TODO winning scene
                    MapData.Instance.Reset(); // this line shouldn't be there
                    SceneManager.LoadScene("MainMenu");
                    return;
                }
            }
            UpdatePlayerData();
            MoveToOptionScene();
        }
        
        public void UpdatePlayerData()
        {
            var playerData = PlayerData.Instance;
            var player = this.BattleField.GetPlayerFromMap();
            playerData.Hp = player.HP;
        }
        
        public static int GetBonusLevel()
        {
            var layer = MapData.Instance.CurrentLayer;
            var config = MapData.Instance.CurrentMap.Config;
            return MapData.Instance.CurrentNodeType switch
            {
                NodeType.FIGHT => (int)(config.BonusLevel.NormalStart + config.BonusLevel.NormalRamp * layer),
                NodeType.ELITE_FIGHT => (int)(config.BonusLevel.EliteStart + config.BonusLevel.EliteRamp * layer),
                _ => 0
            };
        }

        public void MoveToOptionScene()
        {
            var mapData = MapData.Instance;
            var optionName = mapData.CurrentNodeType switch
            {
                NodeType.FIGHT => $"normal_fight_bonus_{GetBonusLevel()}",
                NodeType.ELITE_FIGHT => $"elite_fight_bonus_{GetBonusLevel()}",
                NodeType.BOSS => "boss_fight_bonus",
                _ => ""
            };
            MiscData.Instance.OptionBundle = StaticDataManager.OptionDataManager.GetBundle(optionName);
            SceneManager.LoadScene("OptionChoose");
        }
    }
}
