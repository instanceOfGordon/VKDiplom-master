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
        /// <param name="expression">String form of aproximated function.</param>
        /// <param name="varX">Substring of expression used as first variable.</param>
        /// <param name="varY">Substring of expression used as second variable.</param>
        /// <returns>
        ///     Anonymous delegate if expression is correct math function, othervise
        ///     returns null;
        /// </returns>
        internal static Func<double, double, double> FromString(string expression, string varX, string varY)
        {
            var engine = new CalcEngine
            {
                Variables =
                {
                    [varX] = 0.0d,
                    [varY] = 0.0d
                }
            };
            try
            {
                engine.Evaluate(expression);
            }
            catch (Exception)
            {
                return null;
            }
            Func<double, double, double> function = (x, y) =>
            {
                engine.Variables[varX] = x;
                engine.Variables[varY] = y;
                var result = (double) engine.Evaluate(expression);
                return result;
            };
            return function;
        }


        //Za toto sa hanbim. 

        internal static double NaNSafeCall(Func<double, double, double> func, double p0, double p1)
        {
            //float offset = _meshDensity/10;
            var value = func(p0, p1);
            if (Double.IsNaN(value) || Double.IsInfinity(value))
            {
                value = func(p0, p1 - 0.5);
                if (Double.IsNaN(value) || Double.IsInfinity(value))
                {
                    value = func(p0 - 0.5, p1 - 0.5);
                }
            }


            return value;
        }
    }
}