using HermiteInterpolation.Numerics.MathFunctions;
using SymbolicDifferentiation;

namespace HermiteInterpolation.Numerics
{
    /// <summary>
    /// Default interpreter of mathematic R->RxR functions.
    /// </summary>
    public class SymbolicMathExpression : MathExpression
    {
        public SymbolicMathExpression(string expression, string[] variables) : base(expression, variables)
        {
        }

        protected MathFunction Interpret(string expression) => vals =>
        {
            double result = 0;
            (new Calculator()).Calculate(expression, vals[0], vals[1], ref result);
            return result;
        };

        public string Differentiate(params string[] respectToVariables)
        {
            var diff = Expression;
            foreach (var respectTo in respectToVariables)
            {
                diff = (new Differentiator()).Differentiate(diff, respectTo, true);
            }
            return diff;
        }

        public override MathFunction Interpret()
        {
            return Interpret(Expression);
        }

        public override MathFunction InterpretMathDifferentiation(params string[] respectToVariables)
        {
            var diff = Differentiate(respectToVariables);

            return Interpret(diff);
        }
    }
}