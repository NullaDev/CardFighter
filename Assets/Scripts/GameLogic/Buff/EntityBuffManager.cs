using System;
using System.Collections.Generic;
using System.Linq;

namespace GameLogic.Buff
{
    public static class EntityBuffManager
    {
        public enum BuffType
        {
            Positive,
            Neutral,
            Negative
        }

        public static BuffType FromString(string typeName)
        {
            if (Enum.TryParse<BuffType>(typeName, true, out var buffType))
            {
                return buffType;
            }
            else
            {
                throw new ArgumentException($"Invalid BuffType: {typeName}");
            }
        }
        
        public static void Merge(EntityBuff existing, EntityBuff newBuff)
        {
            if (existing == null || newBuff == null)
                throw new ArgumentNullException();

            var stackableParams = newBuff.StackableParams;

            foreach (var newRule in newBuff.EffectRules)
            {
                switch (newRule)
                {
                    case IOperatorEffect newOp and BuffEffectRule:
                    {
                        var oldRule = existing.EffectRules.FirstOrDefault(r => r.GetType() == newRule.GetType());
                        if (oldRule is IOperatorEffect oldOp)
                        {
                            if (IsAdditiveOperator(newOp.Operator) && IsAdditiveOperator(oldOp.Operator))
                            {
                                var oldSigned = oldOp.Operator == ArithmeticOperator.Minus ? -oldOp.Value : oldOp.Value;
                                var newSigned = newOp.Operator == ArithmeticOperator.Minus ? -newOp.Value : newOp.Value;
                                var merged = oldSigned + newSigned;
                                if (merged >= 0)
                                {
                                    oldOp.Operator = ArithmeticOperator.Add;
                                    oldOp.Value = merged;
                                }
                                else
                                {
                                    oldOp.Operator = ArithmeticOperator.Minus;
                                    oldOp.Value = Math.Abs(merged);
                                }
                            }
                            else if (IsMultiplicativeOperator(newOp.Operator) && IsMultiplicativeOperator(oldOp.Operator))
                            {
                                var oldFactor = oldOp.Operator == ArithmeticOperator.Divide ? 1 / oldOp.Value : oldOp.Value;
                                var newFactor = newOp.Operator == ArithmeticOperator.Divide ? 1 / newOp.Value : newOp.Value;
                                var merged = oldFactor * newFactor;
                                oldOp.Operator = ArithmeticOperator.Multiply;
                                oldOp.Value = merged;
                            }
                        }
                        else
                        {
                            existing.EffectRules.Add(newRule.Clone());
                        }

                        break;
                    }

                    case BlockEffectRule:
                        break;

                    case MiscEffectRule newMisc:
                    {
                        var oldMisc = existing.EffectRules.OfType<MiscEffectRule>().FirstOrDefault();
                        if (oldMisc == null)
                        {
                            existing.EffectRules.Add(newMisc.Clone());
                            break;
                        }

                        foreach (var (key, newValObj) in newMisc.Parameters)
                        {
                            if (!stackableParams.Contains(key))
                                continue;

                            var oldVal = oldMisc.Parameters.TryGetValue(key, out var oldValObj)
                                ? Convert.ToSingle(oldValObj)
                                : 0f;

                            var newVal = Convert.ToSingle(newValObj);
                            oldMisc.Parameters[key] = oldVal + newVal;
                        }

                        break;
                    }

                    default:
                        existing.EffectRules.Add(newRule.Clone());
                        break;
                }
            }

            existing.Duration = newBuff.Duration;
        }

        private static bool IsAdditiveOperator(ArithmeticOperator op)
            => op is ArithmeticOperator.Add or ArithmeticOperator.Minus;

        private static bool IsMultiplicativeOperator(ArithmeticOperator op)
            => op is ArithmeticOperator.Multiply or ArithmeticOperator.Divide;
        
        public const string Insight = "insight";
        public const string Stunned = "stunned";
        public const string CounterAttack = "counter_attack";
            public const string CounterAttackValue1 = "counter_attack_value_constant";
            public const string CounterAttackValue2 = "counter_attack_value_ratio";
        public const string Initiative = "initiative";
        public const string SuperArmor = "super_armor";
        public const string Rooted = "rooted";
        public const string LockedFacing = "locked_facing";
        
        public const string Rites = "rites";
        public const string Music = "music";
            public const string MusicPositiveValue = "music_positive_value";
            public const string MusicNegativeValue = "music_negative_value";
            public const string Harmony = "harmony";
            public const string Chaos = "chaos";
        public const string Archery = "archery";
        public const string Charioteering = "charioteering";
        public const string Calligraphy = "calligraphy";
            public const string CalligraphyPositiveValue = "calligraphy_positive_value";
            public const string CalligraphyNegativeValue = "calligraphy_negative_value";
        public const string Mathematics = "mathematics";
        
        public const string FollowHeart = "follow_heart";
        public const string Practice = "practice";
            public const string PracticeValue = "practice_value";
    }
}