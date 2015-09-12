using System;
using System.Collections.Generic;
using System.Linq;
using CalculationEngine;

namespace HermiteInterpolation.MathFunctions
{

    public delegate double MathFunction(params double[] variables);
    /// <summary>
    /// Factory for creating anonymous delegates represents RxR->R math functions.
    /// </summary>
    internal static class MathFunctions
    {

      

        //Za toto sa hanbim. 

        public static double SafeCall(MathFunction mathFunction, double x, double y)
        {
            //float offset = _meshDensity/10;
            //var value = mathFunction(x, y);
            //if (!double.IsNaN(value) && !double.IsInfinity(value)) return value;
            //value = mathFunction(x, y - 0.05);
            //if (!double.IsNaN(value) && !double.IsInfinity(value)) return value;
            //value = mathFunction(x - 0.05, y - 0.05);

            var value = mathFunction(x, y);
            if (!double.IsNaN(value) && !double.IsInfinity(value)) return value;
            value = mathFunction(x, y +  double.Epsilon);
            if (!double.IsNaN(value) && !double.IsInfinity(value)) return value;
            value = mathFunction(x +  double.Epsilon, y + double.Epsilon);
            return value;
        }

        public static MathFunction CompileFromString(string mathExpression,params string[] variables)
        {
            var variablesValues = variables.ToDictionary<string, string, object>(t => t, t => 0.0d);

            var engine = new CalcEngine { Variables = variablesValues };
            try
            {
                engine.Evaluate(mathExpression);
            }
            catch (Exception)
            {
                return null;
            }
            MathFunction mathFunction = variableValues =>
            {
                for (int i = 0; i < variableValues.Length; i++)
                    engine.Variables[variables[i]] = variableValues[i];


                //engine.Variables[Variables[0]] = variableValues[0];
                //engine.Variables[Variables[1]] = variableValues[1];
                var result = (double)engine.Evaluate(mathExpression);
                return result;
            };
            return mathFunction;
        }
    }
}