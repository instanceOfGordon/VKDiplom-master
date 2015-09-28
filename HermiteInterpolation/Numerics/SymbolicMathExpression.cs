using System;
using HermiteInterpolation.MathFunctions;
using SymbolicDifferentiation;

namespace HermiteInterpolation.Numerics
{
    public class SymbolicMathExpression : MathExpression
    {
        public SymbolicMathExpression(string expression, string[] variables) : base(expression,variables)
        {
           
        }

        public override MathFunction Compile()
        {
            return vals =>
            {
                double result = 0;
                Calculator.Calculate(Expression, vals[0], vals[1], ref result);
                return result;
            };
        }

        private MathFunction CompileDerivative(string deriveFunction)
        {
            return vals =>
            {
                double result = 0;
                Calculator.Calculate(deriveFunction, vals[0], vals[1], ref result);
                return result;
            };
        }

        public override MathFunction Differentiate(params string[] respectTovariables)
        {
            var diff = Expression;

            foreach (var respectTo in respectTovariables)
            {
                diff = Differentiator.Differentiate(diff, respectTo, true);
            }

            return CompileDerivative(diff);
        }
    }
}