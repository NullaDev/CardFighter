using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GameLogic.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Registry.Data;

namespace GameLogic.Buff
{

    public interface IOperatorEffect
    {
        public ArithmeticOperator Operator { get; set; }
        public float Value { get; set; }

        public static void ApplyBuffEffect(ref int value, ref double additiveModifier, ref double multipleModifier, IOperatorEffect effect)
        {
            switch (effect.Operator)
            {
                case ArithmeticOperator.Add:
                case ArithmeticOperator.Minus:
                    additiveModifier = OperatorUtils.ApplyOperator(additiveModifier, effect.Operator, effect.Value);
                    break;
                case ArithmeticOperator.Multiply:
                case ArithmeticOperator.Divide:
                    multipleModifier = OperatorUtils.ApplyOperator(multipleModifier, effect.Operator, effect.Value);
                    break;
                case ArithmeticOperator.Set:
                    value = (int)effect.Value;
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
    
    public interface IBuffFilterEffect
    {
        public List<string> WithBuff { get; set; }
        public List<string> WithoutBuff { get; set; }
        
        public bool BuffSatisfied(EntityBase self)
        {
            var hasAllWithBuff = WithBuff.All(self.HasBuff);
            var hasNoWithoutBuff = WithoutBuff.All(b => !self.HasBuff(b));
            return hasAllWithBuff && hasNoWithoutBuff;
        }
    }
    
    public interface IConditionFilterEffect
    {
        public List<string> WithCondition { get; set; }
        public List<string> WithoutCondition { get; set; }
        
        public bool ConditionSatisfied(EntityBase self, EntityBase target, BattleField battleField)
        {
            var hasAllWithCondition = WithCondition.All(c => Condition.CheckCondition(c, self, target, battleField));
            var hasNoWithoutCondition = WithoutCondition.All(c => !Condition.CheckCondition(c, self, target, battleField));
            return hasAllWithCondition && hasNoWithoutCondition;
        }
    }

    public abstract class BuffEffectRule
    {
        public abstract BuffEffectRule Clone();
        
        public static List<BuffData> ParseBuffs(JToken buffsToken, JsonSerializer serializer)
        {
            var result = new List<BuffData>();
            if (buffsToken?.Type != JTokenType.Array)
                return result;

            foreach (var buffToken in buffsToken)
            {
                var buffData = new BuffData
                {
                    BuffName = buffToken["BuffName"]?.ToString(),
                    BuffType = buffToken["BuffType"]?.ToString() ?? "Positive",
                    Turn = buffToken["Turn"]?.ToObject<int>() ?? 1,
                    Rules = new List<BuffEffectRule>()
                };

                var rulesToken = buffToken["Rules"];
                if (rulesToken != null)
                {
                    foreach (var r in rulesToken)
                    {
                        var ruleType = r["RuleType"]?.ToString();
                        BuffEffectRule rule = ruleType switch
                        {
                            "damage_caused" => r.ToObject<CausedDamageEffectRule>(serializer),
                            "damage_received" => r.ToObject<ReceivedDamageEffectRule>(serializer),
                            "card_cost" => r.ToObject<CardCostEffectRule>(serializer),
                            "block" => r.ToObject<BlockEffectRule>(serializer),
                            "misc" => r.ToObject<MiscEffectRule>(serializer),
                            _ => throw new Exception("Unknown RuleType: " + ruleType)
                        };
                        buffData.Rules.Add(rule);
                    }
                }

                result.Add(buffData);
            }
            return result;
        }
    }

    public class CausedDamageEffectRule : BuffEffectRule, IOperatorEffect, IBuffFilterEffect, IConditionFilterEffect
    {
        public ArithmeticOperator Operator { get; set; }
        public float Value { get; set; }
        public int RemainingTimes { get; set; } = -1;

        public List<string> TargetTags { get; set; } = new();
        public bool TagsLogicOr { get; set; } = true;

        public List<string> WithBuff { get; set; } = new();
        public List<string> WithoutBuff { get; set; } = new();
        public List<string> WithCondition { get; set; } = new();
        public List<string> WithoutCondition { get; set; } = new();
        
        public override BuffEffectRule Clone()
        {
            return new CausedDamageEffectRule
            {
                Operator = this.Operator,
                Value = this.Value,
                RemainingTimes = this.RemainingTimes,
                TargetTags = new List<string>(this.TargetTags),
                TagsLogicOr = this.TagsLogicOr,
                WithBuff = new List<string>(this.WithBuff),
                WithoutBuff = new List<string>(this.WithoutBuff),
                WithCondition = new List<string>(this.WithCondition),
                WithoutCondition = new List<string>(this.WithoutCondition)
            };
        }
    }

    public class ReceivedDamageEffectRule : BuffEffectRule, IOperatorEffect, IBuffFilterEffect, IConditionFilterEffect
    {
        public ArithmeticOperator Operator { get; set; }
        public float Value { get; set; }
        public int RemainingTimes { get; set; } = -1;

        public List<string> TargetTags { get; set; } = new();
        public bool TagsLogicOr { get; set; } = true;

        public List<string> WithBuff { get; set; } = new();
        public List<string> WithoutBuff { get; set; } = new();
        public List<string> WithCondition { get; set; } = new();
        public List<string> WithoutCondition { get; set; } = new();

        public override BuffEffectRule Clone()
        {
            return new ReceivedDamageEffectRule
            {
                Operator = this.Operator,
                Value = this.Value,
                RemainingTimes = this.RemainingTimes,
                TargetTags = new List<string>(this.TargetTags),
                TagsLogicOr = this.TagsLogicOr,
                WithBuff = new List<string>(this.WithBuff),
                WithoutBuff = new List<string>(this.WithoutBuff),
                WithCondition = new List<string>(this.WithCondition),
                WithoutCondition = new List<string>(this.WithoutCondition)
            };
        }
    }
    
    public class CardCostEffectRule : BuffEffectRule, IOperatorEffect, IBuffFilterEffect
    {
        public ArithmeticOperator Operator { get; set; }
        public float Value { get; set; }

        public bool AffectAllCards { get; set; } = false;
        public List<string> AffectedCardIds { get; set; } = new();

        public List<string> WithBuff { get; set; } = new();
        public List<string> WithoutBuff { get; set; } = new();

        public override BuffEffectRule Clone()
        {
            return new CardCostEffectRule
            {
                Operator = this.Operator,
                Value = this.Value,
                AffectAllCards = this.AffectAllCards,
                AffectedCardIds = new List<string>(this.AffectedCardIds),
                WithBuff = new List<string>(this.WithBuff),
                WithoutBuff = new List<string>(this.WithoutBuff)
            };
        }
    }

    public class BlockEffectRule : BuffEffectRule
    {
        public int RemainingTimes { get; set; }
        public bool FrontOnly { get; set; } = true;

        public override BuffEffectRule Clone()
        {
            return new BlockEffectRule
            {
                RemainingTimes = this.RemainingTimes,
                FrontOnly = this.FrontOnly
            };
        }
    }

    public class MiscEffectRule : BuffEffectRule
    {
        public Dictionary<string, object> Parameters { get; set; } = new();

        public override BuffEffectRule Clone()
        {
            return new MiscEffectRule
            {
                Parameters = new Dictionary<string, object>(this.Parameters)
            };
        }
    }
}