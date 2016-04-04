using HermiteInterpolation.Numerics.MathFunctions;

namespace HermiteInterpolation.Numerics
{
    /// <summary>
    ///     Abstract interpreter for mathematic R->RxR functions.
    /// </summary>
    public abstract class MathExpression
    {
        protected MathExpression(string expression, string[] variables)
        {
            Expression = expression;
            Variables = variables;
        }

        public string Expression { get; }
        public string[] Variables { get; }
        public abstract MathFunction Interpret();

        public abstract MathFunction InterpretMathDifferentiation
            (params string[] respectToVariables);

        public static MathExpression CreateDefault
            (string expression, params string[] variables)
        {
            //return new SymbolicMathExpression(expression, variables);
            return new CalculationMathExpression(expression, variables);
        }
    }
}