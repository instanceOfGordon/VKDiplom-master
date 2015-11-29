namespace HermiteInterpolation.Numerics
{
    internal static class LinearSystems
    {
        /// <summary>
        ///     Solve tridiagonal system of equations.
        ///     All values in input attributes will be changed!
        /// </summary>
        /// <param name="lowerDiagonal">Lower diagonal.</param>
        /// <param name="mainDiagonal">Main diagonal.</param>
        /// <param name="upperDiagonal">Upper diagonal. Content of array will be changed.</param>
        /// <param name="rightSide">Right side. Will contains result when completed.</param>
        /// <returns>Result (== rightSide when completed)</returns>
        internal static double[] SolveTridiagonalSystem(double[] lowerDiagonal, double[] mainDiagonal, double[] upperDiagonal,
            double[] rightSide)
        {
            upperDiagonal[0] /= mainDiagonal[0];
            rightSide[0] /= mainDiagonal[0];
            for (var i = 1; i < lowerDiagonal.Length; i++)
            {
                var m = 1/(mainDiagonal[i] - lowerDiagonal[i]*upperDiagonal[i - 1]);
                upperDiagonal[i] *= m;
                rightSide[i] = (rightSide[i] - lowerDiagonal[i]*rightSide[i - 1])*m;
            }

            for (var i = rightSide.Length - 1; i-- > 0;)
            {
                rightSide[i] = rightSide[i] - upperDiagonal[i]*rightSide[i + 1];
            }
            return rightSide;
        }
    }
}