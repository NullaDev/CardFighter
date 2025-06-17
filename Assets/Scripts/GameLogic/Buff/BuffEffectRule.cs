using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameLogic.Buff
{
    
    public enum BuffEffectOperator
    {
        Add,
        Minus,
        Multiply,
        Divide,
        Set
    }

    public interface IOperatorEffect
    {
        public BuffEffectOperator Operator { get; set; }
        public float Value { get; set; }

        public static void ApplyBuffEffect(ref int value, ref double additiveModifier, ref double multipleModifier, IOperatorEffect effect)
        {
            switch (effect.Operator)
            {
                case BuffEffectOperator.Add:
                    additiveModifier += effect.Value;
                    break;
                case BuffEffectOperator.Minus:
                    additiveModifier -= effect.Value;
                    break;
                case BuffEffectOperator.Multiply:
                    multipleModifier *= effect.Value;
                    break;
                case BuffEffectOperator.Divide:
                    if (effect.Value == 0)
                        throw new DivideByZeroException();
                    multipleModifier /= effect.Value;
                    break;
                case BuffEffectOperator.Set:
                    value = (int)effect.Value;
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }

    public abstract class BuffEffectRule
    {
        public abstract BuffEffectRule Clone();
    }

    public class CausedDamageEffectRule : BuffEffectRule, IOperatorEffect
    {
        public BuffEffectOperator Operator { get; set; }
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

    public class ReceivedDamageEffectRule : BuffEffectRule, IOperatorEffect
    {
        public BuffEffectOperator Operator { get; set; }
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