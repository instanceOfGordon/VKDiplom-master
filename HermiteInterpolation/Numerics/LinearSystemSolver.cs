

namespace HermiteInterpolation.Numerics
{
    internal static class LinearSystemSolver 
    {
        /// <summary>  
        /// Solve tridiagonal system of equations. 
        /// All values in input attributes will be changed!     
        /// </summary>
        /// <param name="a">Lower diagonal.</param>
        /// <param name="b">Main diagonal.</param>
        /// <param name="c">Upper diagonal.</param>
        /// <param name="d">Right side. Will contains result when completed.</param>
       /// <returns>Result (== d when completed)</returns>
        internal static double[] TridiagonalSystem(double[] a, double[] b, double[] c, double[] d)
        {
            var n=a.Length;
            c[0] /= b[0];
            d[0] /= b[0];
            for (int i = 1; i < a.Length; i++)
            {
                var m = 1/(b[i] - a[i]*c[i - 1]);
                c[i] *=m;
                d[i] = (d[i] - a[i] * d[i - 1]) *m;
            }
            --n;
            d[n] = (d[n] - a[n] * d[n - 1]) / (b[n] - a[n] * c[n - 1]);

            for (int i = n; i-- > 0; )
            {
                d[i] -= c[i] * d[i + 1];
            }
            return d;
        }
    }
}
