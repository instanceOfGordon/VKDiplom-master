namespace HermiteInterpolation.Numerics.MathFunctions
{

    public delegate double MathFunction(params double[] variablesValues);
    /// <summary>
    /// Factory for creating anonymous delegates represents RxR->R math functions.
    /// </summary>
    internal static class MathFunctionExtensions
    {

      

        //Za toto sa hanbim. 

        public static double SafeCall(this MathFunction mathFunction, double x, double y)
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