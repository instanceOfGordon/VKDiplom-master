using System;
using CalculationEngine;

namespace HermiteInterpolation.Functions
{
    /// <summary>
    /// Factory for creating anonymous delegates represents RxR->R math functions.
    /// </summary>
    internal static class Functions
    {
        /// <summary>
        ///     Parse function from string.        /// 
        /// </summary>
        /// <param name="mathExpression">String form of aproximated function.</param>
        /// <param name="variableX">Substring of mathExpression used as first variable.</param>
        /// <param name="variableY">Substring of mathExpression used as second variable.</param>
        /// <returns>
        ///     Function delegate if mathExpression is correct math function, othervise
        ///     returns null;
        /// </returns>
        internal static Function FromString(string mathExpression, string variableX, string variableY)
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
            Function function = (x, y) =>
            {
                engine.Variables[variableX] = x;
                engine.Variables[variableY] = y;
                var result = (double) engine.Evaluate(mathExpression);
                return result;
            };
            return function;
        }


        //Za toto sa hanbim. 

        internal static double NaNSafeCall(Function function, double x, double y)
        {
            //float offset = _meshDensity/10;
            var value = function(x, y);
            if (!double.IsNaN(value) && !double.IsInfinity(value)) return value;
            value = function(x, y - 0.05);
            if (!double.IsNaN(value) && !double.IsInfinity(value)) return value;
            value = function(x - 0.05, y - 0.05);


            return value;
        }
    }
}