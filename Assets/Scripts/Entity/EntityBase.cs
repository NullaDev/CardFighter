using System.Collections.Generic;
using System.Linq;
using Fighting;
using GameLogic;
using Newtonsoft.Json;

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
            
            if (this.HasBuff(EntityBuffManager.HonestWord))
            {
                var buff = this.GetBuff(EntityBuffManager.HonestWord);
                multipleModifier *= buff.GetParam<float>(EntityBuffManager.HonestWordValue);
                this.Buffs.Remove(buff);
            }
            
            if (this.HasBuff(EntityBuffManager.Harmony))
            {
                var buff = this.GetBuff(EntityBuffManager.Harmony);
                additiveModifier += buff.GetParam<int>(EntityBuffManager.HarmonyValue);
                this.Buffs.Remove(buff);
            }
            
            if (this.HasBuff(EntityBuffManager.Chaos))
            {
                var buff = this.GetBuff(EntityBuffManager.Chaos);
                additiveModifier -= buff.GetParam<int>(EntityBuffManager.ChaosValue);
                this.Buffs.Remove(buff);
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
                        else if (isBackAttacked && !this.HasBuff(EntityBuffManager.FollowHeart))
                        {
                            multipleModifier *= this.GetBuff(EntityBuffManager.Rites).GetParam<float>(EntityBuffManager.RitesNegativeValue);
                        }
                    }
                }
                else if (this.HasBuff(EntityBuffManager.Archery) && !this.HasBuff(EntityBuffManager.FollowHeart))
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
            if (!damageTags.Contains(DamageTypeNames.CounterAttack))
                this.DamageDealtThisTurn = true;
        }

        public void TryHurtFrom(EntityBase source, int value, BattleField battleField, List<string> damageTags)
        {
            var additiveModifier = 0;
            var multipleModifier = 1.0;
            var doCauseDamage = true;
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
                            doCauseDamage = false;
                            if (blockTimes <= 1)
                            {
                                Buffs.Remove(blockBuff);
                            }
                            else
                            {
                                blockBuff.SetParam(EntityBuffManager.BlockTimes, blockTimes - 1);
                            }
                        }
                    }
                }

                if (doCauseDamage)
                {
                    this.Hurt(source, value, battleField);
                    if (this is Player && source is Enemy enemy)
                    {
                        enemy.DealtDamageToPlayer = true;
                    }
                    if (this.HasBuff(EntityBuffManager.CounterAttack) && !damageTags.Contains(DamageTypeNames.CounterAttack))
                    {
                        var counterValue = this.GetBuff(EntityBuffManager.CounterAttack).GetParam<int>(EntityBuffManager.CounterAttackValue);
                        this.DoDamageTo(source, counterValue, battleField, new List<string>{DamageTypeNames.CounterAttack});
                    }
                }
            }
        }

        public abstract void Hurt(EntityBase source, int value, BattleField battleField);

        public void SetDeadAndRemove(BattleField battleField)
        {
            this.IsDead = true;
            battleField.RemoveEntityFromMap(this);
        }
        
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
                    if (this.HasBuff(EntityBuffManager.FollowHeart)) return;
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