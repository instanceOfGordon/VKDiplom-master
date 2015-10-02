using HermiteInterpolation.MathFunctions;
using HermiteInterpolation.Numerics.MathFunctions;
using SymbolicDifferentiation;

namespace HermiteInterpolation.Numerics
{
    public class SymbolicMathExpression : MathExpression
    {
        public SymbolicMathExpression(string expression, string[] variables) : base(expression, variables)
        {
        }

        protected MathFunction Compile(string expression) => vals =>
        {
            double result = 0;
            Calculator.Calculate(expression, vals[0], vals[1], ref result);
            return result;
        };

        public string Differentiate(params string[] respectToVariables)
        {
            var diff = Expression;
            foreach (var respectTo in respectToVariables)
            {
                diff = Differentiator.Differentiate(diff, respectTo, true);
            }
            return diff;
        }

        //protected MathFunction CompileDerivative(string deriveFunction) =>
        //    vals =>
        //    {
        //        double result = 0;
        //        Calculator.Calculate(deriveFunction, vals[0], vals[1], ref result);
        //        return result;
        //    };

        public override MathFunction Compile()
        {
            return Compile(Expression);
        }

        public override MathFunction CompileDerivative(params string[] respectToVariables)
        {
            var diff = Differentiate(respectToVariables);

            return Compile(diff);
        }
    }
}