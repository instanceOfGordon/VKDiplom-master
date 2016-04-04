using System;
using CalculationEngine;
using HermiteInterpolation.Numerics.MathFunctions;
using HermiteInterpolation.Utils;
using MathNet.Numerics;

namespace HermiteInterpolation.Numerics
{
    public class CalculationMathExpression : MathExpression
    {
        public CalculationMathExpression(string expression, string[] variables)
            : base(expression, variables)
        {
        }

        public override MathFunction Interpret()
        {
            var calcEngine = new CalcEngine();
            foreach (var variable in Variables)
            {
                calcEngine.Variables.Add(variable, 0);
            }
            return vals =>
            {
                for (var i = 0; i < vals.Length; i++)
                {
                    calcEngine.Variables[Variables[i]] = vals[i];
                }
                return (double) calcEngine.Evaluate(Expression);
            };
        }

        public override MathFunction InterpretMathDifferentiation
            (params string[] respectToVariables)
        {
            var diff = Interpret();
            for (var i = 0; i < respectToVariables.Length; i++)
            {
                var respectTo = Variables.IndexOf(respectToVariables[i]);
                diff = new MathFunction
                    (Differentiate.FirstPartialDerivativeFunc
                        (new Func<double[], double>(diff), respectTo));
            }
            return diff;
        }
    }
}