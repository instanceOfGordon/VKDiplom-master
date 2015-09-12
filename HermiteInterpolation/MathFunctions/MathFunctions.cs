using System;
using CalculationEngine;

namespace HermiteInterpolation.MathFunctions
{
    /// <summary>
    /// Factory for creating anonymous delegates represents RxR->R math functions.
    /// </summary>
    internal static class MathFunctions
    {
        /// <summary>
        ///     Parse MathFunction from string.        /// 
        /// </summary>
        /// <param name="mathExpression">String form of aproximated MathFunction.</param>
        /// <param name="variableX">Substring of mathExpression used as first variable.</param>
        /// <param name="variableY">Substring of mathExpression used as second variable.</param>
        /// <returns>
        ///     MathFunction delegate if mathExpression is correct math MathFunction, othervise
        ///     returns null;
        /// </returns>
        internal static MathFunction FromString(string mathExpression, string variableX, string variableY)
        {
            var engine = new CalcEngine
            {
                Variables =
                {
                    [variableX] = 0.0d,
                    [variableY] = 0.0d
                }
            };
            try
            {
                engine.Evaluate(mathExpression);
            }
            catch (Exception)
            {
                return null;
            }
            MathFunction mathFunction = vars =>
            {
                engine.Variables[variableX] = vars[0];
                engine.Variables[variableY] = vars[1];
                var result = (double) engine.Evaluate(mathExpression);
                return result;
            };
            return mathFunction;
        }


        //Za toto sa hanbim. 

        internal static double SafeCall(MathFunction mathFunction, double x, double y)
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
    }
}