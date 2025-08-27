using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic.Buff;
using Registry.Data;

namespace GameLogic.Entity
{
    public abstract class EntityBase
    {
        public int HP;
        public int MaxHP;
        public string Name = "";
        public string TextureName = "";
        public int Armor = 0;
        public bool IsDead = false;
        public bool DealtDamageToPlayer = false;
        public bool DealtDamageThisTurn = false;
        public List<EntityBuff> Buffs = new();

        public EntityFacing Facing = EntityFacing.Default;
        public virtual bool HasValidFacing => false;

        public EntityBase(int hp)
        {
            this.HP = this.MaxHP = hp;
        }

        public void DoDamageTo(EntityBase target, int value, BattleField battleField, List<string> damageTags)
        {
            var additiveModifier = 0.0;
            var multipleModifier = 1.0;
            foreach (var buff in this.Buffs.ToList())
            {
                foreach (var rule in buff.EffectRules.ToList())
                {
                    if (rule is CausedDamageEffectRule causedRule)
                    {
                        if (causedRule is IBuffFilterEffect buffFilter && !buffFilter.BuffSatisfied(this))
                            continue;

                        if (causedRule is IConditionFilterEffect condFilter && !condFilter.ConditionSatisfied(this, target, battleField))
                            continue;
                        
                        var tagMatch = causedRule.TargetTags.Count == 0 || (
                            causedRule.TagsLogicOr
                                ? causedRule.TargetTags.Any(damageTags.Contains)
                                : causedRule.TargetTags.All(damageTags.Contains)
                        );
                        if (!tagMatch) continue;

                        IOperatorEffect.ApplyBuffEffect(ref value, ref additiveModifier, ref multipleModifier, causedRule);

                        if (causedRule.RemainingTimes > 0)
                        {
                            causedRule.RemainingTimes--;
                            if (causedRule.RemainingTimes == 0)
                            {
                                buff.EffectRules.Remove(causedRule);
                            }
                        }
                    }
                }
            }

            value = (int)(multipleModifier * (value + additiveModifier));
            if (value > 0)
            {
                target.TryHurtFrom(this, value, battleField, damageTags);
            }
            if (!damageTags.Contains(DamageTypeNames.CounterAttack))
                this.DealtDamageThisTurn = true;
        }

        public void TryHurtFrom(EntityBase source, int value, BattleField battleField, List<string> damageTags)
        {
            var additiveModifier = 0.0;
            var multipleModifier = 1.0;
            var doCauseDamage = true;

            foreach (var buff in this.Buffs.ToList())
            {
                foreach (var rule in buff.EffectRules.ToList())
                {
                    if (rule is ReceivedDamageEffectRule receivedRule)
                    {
                        if (receivedRule is IBuffFilterEffect buffFilter && !buffFilter.BuffSatisfied(this))
                            continue;

                        if (receivedRule is IConditionFilterEffect condFilter && !condFilter.ConditionSatisfied(source, this, battleField))
                            continue;
                        
                        var tagMatch = receivedRule.TargetTags.Count == 0 || (
                            receivedRule.TagsLogicOr
                                ? receivedRule.TargetTags.Any(damageTags.Contains)
                                : receivedRule.TargetTags.All(damageTags.Contains)
                        );
                        if (!tagMatch) continue;

                        IOperatorEffect.ApplyBuffEffect(ref value, ref additiveModifier, ref multipleModifier, receivedRule);

                        if (receivedRule.RemainingTimes > 0)
                        {
                            receivedRule.RemainingTimes--;
                            if (receivedRule.RemainingTimes == 0)
                            {
                                buff.EffectRules.Remove(receivedRule);
                            }
                        }
                    }
                    else if (rule is BlockEffectRule blockRule)
                    {
                        if (damageTags.Contains(DamageTypeNames.BreakGuard))
                        {
                            buff.EffectRules.Remove(blockRule);
                            continue;
                        }
                        
                        var attackerIndex = battleField.GetEntityIndex(source);
                        var targetIndex = battleField.GetEntityIndex(this);
                        var isFromFront =
                            (attackerIndex < targetIndex && this.Facing == EntityFacing.Left) ||
                            (attackerIndex > targetIndex && this.Facing == EntityFacing.Right);
                        if (blockRule.FrontOnly && !isFromFront) continue;
                        
                        if (blockRule.RemainingTimes > 0)
                        {
                            doCauseDamage = false;
                            blockRule.RemainingTimes--;
                            if (blockRule.RemainingTimes == 0)
                            {
                                this.Buffs.Remove(buff);
                            }
                        }
                    }
                }
            }
            
            value = (int)(multipleModifier * (value + additiveModifier));
            if (value > 0 && doCauseDamage)
            {
                if (damageTags.Contains(DamageTypeNames.BreakArmor))
                {
                    this.Armor = 0;
                }

                if (this.Armor > 0 && !damageTags.Contains(DamageTypeNames.IgnoreArmor))
                {
                    var absorbed = Math.Min(this.Armor, value);
                    this.Armor -= absorbed;
                    value -= absorbed;
                }

                this.Hurt(source, value, battleField);

                if (this is Player)
                {
                    source.DealtDamageToPlayer = true;
                }

                foreach (var rule in this.Buffs.ToList().SelectMany(buff => buff.EffectRules))
                {
                    if (rule is MiscEffectRule miscRule && !damageTags.Contains(DamageTypeNames.CounterAttack) &&
                        miscRule.Parameters.TryGetValue(EntityBuffManager.CounterAttack, out var counterValueObj))
                    {
                        var counterValue = Convert.ToInt32(counterValueObj);
                        this.DoDamageTo(source, counterValue, battleField, new List<string> { DamageTypeNames.CounterAttack });
                    }
                }
            }
        }

        public abstract void Hurt(EntityBase source, int value, BattleField battleField);

        public void Heal(EntityBase source, int value)
        {
            this.HP = Math.Min(this.HP + value, this.MaxHP);
        }

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
            if (EntityBuffManager.BuffImmunityGroups.Any(kvp => Buffs.Any(b => b.Name == kvp.Key) && kvp.Value.Contains(newBuff.Name)))
            {
                return false;
            }
            
            if (this.HasBuff(EntityBuffManager.Calligraphy) && EntityBuffManager.GetBuffType(newBuff.Name) == EntityBuffManager.BuffType.Positive)
            {
                var calligraphyBuff = this.GetBuff(EntityBuffManager.Calligraphy);
                var currentPositiveBuffs = Buffs.Count(b => EntityBuffManager.GetBuffType(b.Name) == EntityBuffManager.BuffType.Positive);
                var maxPositive = calligraphyBuff.EffectRules
                    .OfType<MiscEffectRule>()
                    .Select(r => r.Parameters.TryGetValue(EntityBuffManager.CalligraphyNegativeValue, out var maxPosObj) ? Convert.ToInt32(maxPosObj) : int.MaxValue)
                    .FirstOrDefault();

                if (currentPositiveBuffs >= maxPositive)
                    return false;

                if (newBuff.Duration > 1)
                {
                    var extraTurns = calligraphyBuff.EffectRules
                        .OfType<MiscEffectRule>()
                        .Select(r => r.Parameters.TryGetValue(EntityBuffManager.CalligraphyPositiveValue, out var extraObj) ? Convert.ToInt32(extraObj) : 0)
                        .FirstOrDefault();
                    if (extraTurns > 0)
                        newBuff.Duration += extraTurns;
                }
            }
            
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
                        foreach (var rule in existing.EffectRules)
                        {
                            if (rule is CausedDamageEffectRule causedRule && param == EntityBuffManager.GenericValueKey)
                            {
                                var newVal = newBuff.EffectRules
                                    .OfType<CausedDamageEffectRule>()
                                    .Select(r => r.Value)
                                    .FirstOrDefault();

                                causedRule.Value += newVal;
                            }
                            else if (rule is ReceivedDamageEffectRule receivedRule && param == EntityBuffManager.GenericValueKey)
                            {
                                var newVal = newBuff.EffectRules
                                    .OfType<ReceivedDamageEffectRule>()
                                    .Select(r => r.Value)
                                    .FirstOrDefault();

                                receivedRule.Value += newVal;
                            }
                            else if (rule is MiscEffectRule miscRule)
                            {
                                if (miscRule.Parameters.TryGetValue(param, out var existingValObj))
                                {
                                    var existingVal = Convert.ToInt32(existingValObj);
                                    var newVal = newBuff.EffectRules
                                        .OfType<MiscEffectRule>()
                                        .Select(r => r.Parameters.TryGetValue(param, out var vObj) ? Convert.ToInt32(vObj) : 0)
                                        .FirstOrDefault();

                                    miscRule.Parameters[param] = existingVal + newVal;
                                }
                            }
                        }
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
        
        public void UpdateStatusAndBuffs()
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

                var miscRule = musicBuff.EffectRules
                    .OfType<MiscEffectRule>()
                    .FirstOrDefault();

                if (miscRule == null) return;

                miscRule.Parameters.TryGetValue(EntityBuffManager.MusicPositiveValue, out var posObj);
                miscRule.Parameters.TryGetValue(EntityBuffManager.MusicNegativeValue, out var negObj);

                var positiveValue = Convert.ToInt32(posObj ?? 0);
                var negativeValue = Convert.ToInt32(negObj ?? 0);

                if (this.DealtDamageThisTurn)
                {
                    if (this.HasBuff(EntityBuffManager.FollowHeart)) return;

                    var buff = new EntityBuff(new BuffData
                    {
                        BuffName = EntityBuffManager.Chaos,
                        Turn = 1,
                        Rules = new List<BuffEffectRule>
                        {
                            new CausedDamageEffectRule
                            {
                                Operator = ArithmeticOperator.Minus,
                                Value = negativeValue
                            }
                        }
                    });
                    this.AddOrUpdateBuff(buff);
                }
                else
                {
                    var buff = new EntityBuff(new BuffData
                    {
                        BuffName = EntityBuffManager.Harmony,
                        Turn = 1,
                        Rules = new List<BuffEffectRule>
                        {
                            new CausedDamageEffectRule
                            {
                                Operator = ArithmeticOperator.Add,
                                Value = negativeValue
                            }
                        }
                    });
                    this.AddOrUpdateBuff(buff);
                }
            }
            this.DealtDamageThisTurn = false;

            this.Armor = 0;
        }
        
    }
}