using System;

namespace GameLogic
{
    public enum Operator
    {
        Add,
        Minus,
        Multiply,
        Divide,
        Set
    }

    public static class OperatorUtils
    {
        public static T ApplyOperator<T>(T value1, Operator op, T value2)
        {
            dynamic a = value1;
            dynamic b = value2;
            return op switch
            {
                Operator.Add => (T)(a + b),
                Operator.Minus => (T)(a - b),
                Operator.Multiply => (T)(a * b),
                Operator.Divide => (T)(a / b),
                Operator.Set => value2,
                _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
            };
        }
    }
}