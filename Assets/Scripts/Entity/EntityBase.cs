using System;
using System.Collections.Generic;
using System.Linq;
using Card;
using Fighting;
using GameLogic;
using UnityEngine;

namespace Entity
{
    public abstract class EntityBase
    {
        public int HP;
        public int MaxHP;
        public string Name = "";
        public string TextureName = "";
        public bool IsDead = false;
        public List<EntityBuff> Buffs = new();

        public EntityFacing Facing = EntityFacing.DEFAULT;

        public EntityBase(int hp)
        {
            this.HP = this.MaxHP = hp;
        }

        public void DoDamageTo(EntityBase target, int value, BattleField battleField, List<string> damageTags)
        {
            var additiveModifier = 0;
            var multipleModifier = 1;
            if (this.HasBuff(EntityBuffManager.Noble))
            {
                additiveModifier += this.GetBuff(EntityBuffManager.Noble).GetParam<int>(EntityBuffManager.NobleValue);
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
                            additiveModifier += 2;
                        }
                        else if (isBackAttacked)
                        {
                            multipleModifier *= 0;
                        }
                    }
                }
            }

            value = multipleModifier * (value + additiveModifier);
            target.Hurt(this, value, battleField);
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
                Buffs.Remove(conflictingBuff);
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
        }
        
    }
}