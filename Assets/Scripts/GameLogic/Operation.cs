using System;

namespace GameLogic
{
    public enum ArithmeticOperator
    {
        Add,
        Minus,
        Multiply,
        Divide,
        Set
    }
    
    public enum RelationalOperator
    {
        GreaterThan,
        LessThan,
        GreaterOrEqual,
        LessOrEqual,
        Equal,
        NotEqual
    }

    public static class OperatorUtils
    {
        public static T ApplyOperator<T>(T value1, ArithmeticOperator op, T value2)
        {
            dynamic a = value1;
            dynamic b = value2;
            return op switch
            {
                ArithmeticOperator.Add => (T)(a + b),
                ArithmeticOperator.Minus => (T)(a - b),
                ArithmeticOperator.Multiply => (T)(a * b),
                ArithmeticOperator.Divide => (T)(a / b),
                ArithmeticOperator.Set => value2,
                _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
            };
        }
        
        public static bool Compare<T>(T value1, RelationalOperator op, T value2)
        {
            dynamic a = value1;
            dynamic b = value2;
            return op switch
            {
                RelationalOperator.GreaterThan    => a >  b,
                RelationalOperator.LessThan       => a <  b,
                RelationalOperator.GreaterOrEqual => a >= b,
                RelationalOperator.LessOrEqual    => a <= b,
                RelationalOperator.Equal          => a == b,
                RelationalOperator.NotEqual       => a != b,
                _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
            };
        }
    }
}