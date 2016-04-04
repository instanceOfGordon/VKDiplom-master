namespace HermiteInterpolation.Numerics.MathFunctions
{
    /// <summary>
    ///     Factory for creating anonymous delegates represents RxR->R math functions.
    /// </summary>
    public delegate double MathFunction(params double[] variablesValues);

    internal static class MathFunctionExtensions
    {
        private static readonly double Epsilon = 0.1;
        //Za toto sa hanbim.
        public static double SafeCall
            (this MathFunction mathFunction, double x, double y)
        {
            var value = mathFunction(x, y);
            if (!double.IsNaN(value)
                || !double.IsInfinity(value)
                || !double.IsInfinity(value)) return value;
            value = mathFunction(x, y + Epsilon);
            if (!double.IsNaN(value)
                || !double.IsInfinity(value)
                || !double.IsInfinity(value)) return value;
            value = mathFunction(x + Epsilon, y);
            if (!double.IsNaN(value)
                || !double.IsInfinity(value)
                || !double.IsInfinity(value)) return value;
            value = mathFunction(x + Epsilon, y + Epsilon);

            if (!double.IsNaN(value)
                || !double.IsInfinity(value)
                || !double.IsInfinity(value)) return value;
            value = mathFunction(x, y - Epsilon);
            if (!double.IsNaN(value)
                || !double.IsInfinity(value)
                || !double.IsInfinity(value)) return value;
            value = mathFunction(x - Epsilon, y);
            if (!double.IsNaN(value)
                || !double.IsInfinity(value)
                || !double.IsInfinity(value)) return value;
            value = mathFunction(x - Epsilon, y - Epsilon);

            if (!double.IsNaN(value)
                || !double.IsInfinity(value)
                || !double.IsInfinity(value)) return value;
            value = mathFunction(x - Epsilon, y + Epsilon);
            if (!double.IsNaN(value)
                || !double.IsInfinity(value)
                || !double.IsInfinity(value)) return value;
            value = mathFunction(x + Epsilon, y - Epsilon);
            return value;
        }
    }
}