
using System;
using System.Linq;
using HermiteInterpolation.Numerics.MathFunctions;
using CalculationEngine;
using HermiteInterpolation.Utils;
using MathNet.Numerics;

namespace HermiteInterpolation.Numerics
{
    public class CalculationMathExpression : MathExpression
    {

        public CalculationMathExpression(string expression, string[] variables) : base(expression, variables)
        {
        }

        public override MathFunction Interpret()
        {
            var calcEngine = new CalcEngine();
            foreach (var variable in Variables)
            {
                calcEngine.Variables.Add(variable,0);
            }
            return vals =>
            {
                for (int i = 0; i < vals.Length; i++)
                {
                    calcEngine.Variables[Variables[i]] = vals[i];
                }
                return (double)calcEngine.Evaluate(Expression);
                 
            };
        }

        public override MathFunction InterpretMathDifferentiation(params string[] respectToVariables)
        {
            var diff = Interpret();
            //var diff = new MathFunction(Differentiate.FirstPartialDerivativeFunc(new Func<double[], double>(z), 0));
            //for (int i = 0; i < respectToVariables.Length; i++)
            //{
            //    var respectTo = respectToVariables[i] == "x" ? 0 : 1;
            //    diff = new MathFunction(Differentiate.FirstPartialDerivativeFunc(new Func<double[], double>(diff), respectTo));
            //}
            for (int i = 0; i < respectToVariables.Length; i++)
            {
                var respectTo = Variables.IndexOf(respectToVariables[i]);
                diff = new MathFunction(Differentiate.FirstPartialDerivativeFunc(new Func<double[], double>(diff), respectTo));
            }
            return diff;
        }

       
    }
}
