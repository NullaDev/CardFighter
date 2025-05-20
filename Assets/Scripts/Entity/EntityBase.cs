using System.Collections.Generic;
using System.Linq;
using Fighting;
using GameLogic;
using Newtonsoft.Json;
using UnityEngine;

namespace Entity
{
    public abstract class EntityBase
    {
        public int HP;
        public int MaxHP;
        public string Name = "";
        public string TextureName = "";
        [JsonIgnore] public bool IsDead = false;
        [JsonIgnore] public bool DamageDealtThisTurn = false;
        [JsonIgnore] public List<EntityBuff> Buffs = new();

        [JsonIgnore] public EntityFacing Facing = EntityFacing.DEFAULT;

        public EntityBase(int hp)
        {
            this.HP = this.MaxHP = hp;
        }

        public void DoDamageTo(EntityBase target, int value, BattleField battleField, List<string> damageTags)
        {
            var additiveModifier = 0;
            var multipleModifier = 1.0;
            if (this.HasBuff(EntityBuffManager.Noble))
            {
                additiveModifier += this.GetBuff(EntityBuffManager.Noble).GetParam<int>(EntityBuffManager.NobleValue);
            }
            
            if (this.HasBuff(EntityBuffManager.Harmony))
            {
                var harmony = this.GetBuff(EntityBuffManager.Harmony);
                Debug.Log(harmony.GetParam<int>(EntityBuffManager.HarmonyValue));
                additiveModifier += harmony.GetParam<int>(EntityBuffManager.HarmonyValue);
                this.Buffs.Remove(harmony);
            }
            
            if (this.HasBuff(EntityBuffManager.Chaos))
            {
                var chaos = this.GetBuff(EntityBuffManager.Chaos);
                additiveModifier -= chaos.GetParam<int>(EntityBuffManager.ChaosValue);
                this.Buffs.Remove(chaos);
            }
            
            if (damageTags.Contains(DamageTypeNames.Melee))
            {
                if (this.HasBuff(EntityBuffManager.Rites))
                {
                    var attackerIndex = battleField.GetEntityIndex(this);
                    var targetIndex = battleField.GetEntityIndex(target);

                    if (this.Facing != EntityFacing.DEFAULT && target.Facing != EntityFacing.DEFAULT)
                    {
                        var isFacingEachOther =
                            (attackerIndex < targetIndex && this.Facing == EntityFacing.RIGHT && target.Facing == EntityFacing.LEFT) ||
                            (attackerIndex > targetIndex && this.Facing == EntityFacing.LEFT && target.Facing == EntityFacing.RIGHT);

                        var isBackAttacked =
                            (attackerIndex < targetIndex && this.Facing == EntityFacing.RIGHT && target.Facing == EntityFacing.RIGHT) ||
                            (attackerIndex > targetIndex && this.Facing == EntityFacing.LEFT && target.Facing == EntityFacing.LEFT);

                        if (isFacingEachOther)
                        {
                            additiveModifier += this.GetBuff(EntityBuffManager.Rites).GetParam<int>(EntityBuffManager.RitesPositiveValue);
                        }
                        else if (isBackAttacked)
                        {
                            multipleModifier *= this.GetBuff(EntityBuffManager.Rites).GetParam<float>(EntityBuffManager.RitesNegativeValue);
                        }
                    }
                }
                else if (this.HasBuff(EntityBuffManager.Archery))
                {
                    multipleModifier *= this.GetBuff(EntityBuffManager.Archery).GetParam<float>(EntityBuffManager.ArcheryNegativeValue);
                }
            }

            if (damageTags.Contains(DamageTypeNames.Ranged))
            {
                if (this.HasBuff(EntityBuffManager.Archery))
                {
                    additiveModifier += this.GetBuff(EntityBuffManager.Archery).GetParam<int>(EntityBuffManager.ArcheryPositiveValue);
                }
            }

            value = (int)(multipleModifier * (value + additiveModifier));
            if (value > 0)
            {
                target.TryHurtFrom(this, value, battleField, damageTags);
            }
            this.DamageDealtThisTurn = true;
        }

        public void TryHurtFrom(EntityBase source, int value, BattleField battleField, List<string> damageTags)
        {
            var additiveModifier = 0;
            var multipleModifier = 1.0;
            if (this.HasBuff(EntityBuffManager.Mathematics))
            {
                if (this.HasBuff(EntityBuffManager.Insight))
                    additiveModifier -= this.GetBuff(EntityBuffManager.Mathematics).GetParam<int>(EntityBuffManager.MathematicsPositiveValue);
                else
                    additiveModifier += this.GetBuff(EntityBuffManager.Mathematics).GetParam<int>(EntityBuffManager.MathematicsNegativeValue);
            }
            
            value = (int)(multipleModifier * (value + additiveModifier));
            if (value > 0)
            {
                if (this.HasBuff(EntityBuffManager.Block))
                {
                    var blockBuff = this.GetBuff(EntityBuffManager.Block);
                    var blockTimes = blockBuff.GetParam<int>(EntityBuffManager.BlockTimes);

                    var attackerIndex = battleField.GetEntityIndex(source);
                    var targetIndex = battleField.GetEntityIndex(this);
                    var isFromFront =
                        (attackerIndex < targetIndex && this.Facing == EntityFacing.LEFT) ||
                        (attackerIndex > targetIndex && this.Facing == EntityFacing.RIGHT);
                    
                    if (isFromFront)
                    {
                        if (damageTags.Contains(DamageTypeNames.BreakGuard))
                        {
                            Buffs.Remove(blockBuff);
                        }
                        else if (blockTimes > 0)
                        {
                            if (blockTimes <= 1)
                            {
                                Buffs.Remove(blockBuff);
                            }
                            else
                            {
                                blockBuff.SetParam(EntityBuffManager.BlockTimes, blockTimes - 1);
                            }
                            return;
                        }
                    }
                }
                this.Hurt(source, value, battleField);
            }
        }

        public abstract void Hurt(EntityBase source, int value, BattleField battleField);
        
        public bool HasBuff(string buffName)
        {
            return Buffs.Any(b => b.Name == buffName);
        }
        
        public EntityBuff GetBuff(string buffName)
        {
            return Buffs.FirstOrDefault(b => b.Name == buffName);
        }
        
        public bool AddOrUpdateBuff(EntityBuff newBuff)
        {
            foreach (var conflictingBuff in from @group in EntityBuffManager.BuffConflictGroups where @group.Contains(newBuff.Name) select Buffs.FirstOrDefault(b => @group.Contains(b.Name)) into conflictingBuff where conflictingBuff != null select conflictingBuff)
            {
                if (conflictingBuff.Name != newBuff.Name) Buffs.Remove(conflictingBuff);
            }
            
            var existing = Buffs.FirstOrDefault(b => b.Name == newBuff.Name);
            if (existing != null)
            {
                if (EntityBuffManager.ToggleBuffs.Contains(newBuff.Name))
                {
                    Buffs.Remove(existing);
                    return true;
                }
                if (EntityBuffManager.StackableBuffs.TryGetValue(newBuff.Name, out var stackParams))
                {
                    foreach (var param in stackParams)
                    {
                        var newVal = newBuff.GetParam<int>(param);
                        var existingVal = existing.GetParam<int>(param);
                        existing.SetParam(param, existingVal + newVal);
                    }
                    existing.Duration = newBuff.Duration;
                    return true;
                }
                if (existing.Duration == -1 || newBuff.Duration <= existing.Duration && newBuff.Duration != -1)
                {
                    return false;
                }
                Buffs.Remove(existing);
            }
            Buffs.Add(newBuff);
            return true;
        }
        
        public void UpdateBuffs()
        {
            this.Buffs = this.Buffs
                .Select(buff =>
                {
                    if (buff.Duration < 0) return buff;
                    buff.Duration--;
                    return buff;
                })
                .Where(buff => buff.Duration != 0)
                .ToList();

            if (this.HasBuff(EntityBuffManager.Music))
            {
                var musicBuff = this.GetBuff(EntityBuffManager.Music);
                if (this.DamageDealtThisTurn)
                {
                    var buff = new EntityBuff(EntityBuffManager.Chaos, 1);
                    buff.SetParam(EntityBuffManager.ChaosValue, musicBuff.GetParam<int>(EntityBuffManager.MusicNegativeValue));
                    this.AddOrUpdateBuff(buff);
                }
                else
                {
                    var buff = new EntityBuff(EntityBuffManager.Harmony, 1);
                    buff.SetParam(EntityBuffManager.HarmonyValue, musicBuff.GetParam<int>(EntityBuffManager.MusicPositiveValue));
                    this.AddOrUpdateBuff(buff);
                }
            }
            this.DamageDealtThisTurn = false;
        }
        
    }
}