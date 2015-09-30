using System;
using System.Linq;
using CalculationEngine;
using HermiteInterpolation.MathFunctions;

namespace HermiteInterpolation.Numerics
{
    public class NumericMathExpression : MathExpression
    {
        public NumericMathExpression(string expression, params string[] variables) : base(expression, variables)
        {
        }

       


        public override MathFunction Compile()
        {
            var variablesValues = Variables.ToDictionary<string, string, object>(t => t, t => 0.0d);

            var engine = new CalcEngine { Variables = variablesValues };
            try
            {
                engine.Evaluate(Expression);
            }
            catch (Exception)
            {
                return null;
            }
            MathFunction mathFunction = variableValues =>
            {
                for (int i = 0; i < variableValues.Length; i++)
                    engine.Variables[Variables[i]] = variableValues[i];


                //engine.Variables[Variables[0]] = variableValues[0];
                //engine.Variables[Variables[1]] = variableValues[1];
                var result = (double)engine.Evaluate(Expression);
                return result;
            };
            return mathFunction;
        }

        public override MathFunction CompileDerivative(params string[] respectToVariables)
        {
            var diff = Compile();

            foreach (var respectTo in respectToVariables)
            {
                var idx = Array.IndexOf(Variables, respectTo);
                if(idx<0) continue;
                diff = new MathFunction(MathNet.Numerics.Differentiate.FirstPartialDerivativeFunc(new Func<double[], double>(diff), idx));
            }

            return diff;

        }
    }
}