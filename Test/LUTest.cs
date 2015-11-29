using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LUTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestLU()
        {
            var numCount = 7;
            var L = new double[numCount];
            var M = new double[numCount];
            var U = new double[numCount];
            var R = new double[numCount];
            for (var i = 0; i < numCount; i++)
            {
                L[i] = U[i] = 1;
                M[i] = 4;
                R[i] = Math.Sin(i);
            }
            SolveTridiagonalSystem(L, M, U, R);
        }

        private static double[] SolveTridiagonalSystem(double[] lowerDiagonal, double[] mainDiagonal,
            double[] upperDiagonal,
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