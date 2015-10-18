using System;
using HermiteInterpolation.MathFunctions;
using HermiteInterpolation.Numerics;
using HermiteInterpolation.Shapes.SplineInterpolation;
using HermiteInterpolation.Utils;

namespace HermiteInterpolation.SplineKnots
{
    public sealed class ReducedDeBoorKnotsGenerator :KnotsGenerator//: DeBoorKnotsGenerator
    {
        private readonly DeBoorKnotsGenerator _initializator;
        public ReducedDeBoorKnotsGenerator(InterpolativeMathFunction function) : base(function)
        {
            _initializator = new DeBoorKnotsGenerator(function);
        }

        public ReducedDeBoorKnotsGenerator(MathExpression expression) : base(expression)
        {
            _initializator = new DeBoorKnotsGenerator(expression);
        }

        //        private readonly int _uCount;
        //        private readonly int _vCount;
        //
        //        public ReducedDeBoorKnotsGenerator(InterpolativeMathFunction function, int uCount, int vCount) : base(function)
        //        {
        //            _uCount = uCount;
        //            _vCount = vCount;
        //        }

        //private bool _rowsEven;
        //private bool _columnsEven;


        public override KnotMatrix GenerateKnots(SurfaceDimension uDimension, SurfaceDimension vDimension)
        {
            if (uDimension.KnotCount < 4 || vDimension.KnotCount < 4)
            {
                return new DirectKnotsGenerator(Function).GenerateKnots(uDimension, vDimension);
            }

            var values = new KnotMatrix(uDimension.KnotCount, vDimension.KnotCount);
            //_rowsEven = uDimension.KnotCount % 2 == 0;
            //_columnsEven = vDimension.KnotCount % 2 == 0;

            _initializator.InitializeKnots(uDimension, vDimension, values);

            FillXDerivations(values);
            FillYDerivations(values);
            FillXYDerivations(values);
            FillYXDerivations(values);

            return values;
        }

        private double[] MainDiagonal(int unknownsCount,bool even)
        {
            //var even = unknownsCount%2 == 0;
            //var unknownsCount = even ? unknownsCount/2 - 2 : unknownsCount/2 - 3;
            var diag = MyArrays.InitalizedArray<double>(unknownsCount, -14);
            diag[unknownsCount - 1] = even ? -15 : -14;
            return diag;
        }

        private double[] RightSide(Func<int, double> rightSide, double h, double dfirst, double dlast,
            int unknownsCount, bool even)
        {
            //var length = even ? 2 * unknownsCount : 2 * unknownsCount + 1;
            //var equationParams = new EquationParams(length);

            //var rs = new double[unknownsCount];
            //var div3h = 3 / h;
            //var div12h = div3h * 4;
            //rs[0] = div3h * (rightSide(4) - rightSide(0)) - div12h * (rightSide(3) - rightSide(1)) - dfirst;
            //rs[unknownsCount - 1] = div3h *
            //                         (rightSide(equationParams.Upsilon + equationParams.Tau) -
            //                          rightSide(equationParams.Upsilon - 2))
            //                         -
            //                         div12h *
            //                         (rightSide(equationParams.Upsilon + 1) - rightSide(equationParams.Upsilon - 1)) -
            //                         dlast;
            //for (var i = 1; i < unknownsCount; i++)
            //{
            //    var i2 = i * 2;
            //    rs[i] = div3h * (rightSide(i2 + 4) - rightSide(i2)) - div12h * (rightSide(i2 + 3) - rightSide(i2 + 1));
            //}
            //return rs;
            //var length = even ? 2 * unknownsCount : 2 * unknownsCount + 1;
            //var equationParams = new EquationParams(length);
            //var even = unknownsCount%2 == 0;
            //var unknownsCount = even ? unknownsCount/2 - 2 : unknownsCount/2 - 3;
            var tau = even ? 0 : 2;
            var eta = even ? -4 : 1;
            var upsilon = even ? unknownsCount - 0 : unknownsCount - 1;
            //dlast = eta*dlast;
            var rs = new double[unknownsCount];
            var div3h = 3/h;
            var div12h = div3h*4;
            rs[0] = div3h*(rightSide(4) - rightSide(0)) - div12h*(rightSide(3) - rightSide(1)) - dfirst;
            rs[unknownsCount - 1] = div3h*
                                     (rightSide(upsilon + tau) -
                                      rightSide(upsilon - 2))
                                     -
                                     div12h*
                                     (rightSide(upsilon + 1) - rightSide(upsilon - 1)) -
                                     eta*dlast;
            for (var i = 1; i < unknownsCount - 1; i++)
            {
                var i2 = i*2;
                rs[i] = div3h*(rightSide(i2 + 2) - rightSide(i2 - 2)) - div12h*(rightSide(2*i + 1) - rightSide(i2 + 1));
            }
            return rs;
        }

        private double[] RightSideCross(KnotMatrix knots, int i, double dfirst, double dlast,
            int unknownsCount)
        {
            var even = unknownsCount%2 == 0;
            var equationsCount = even ? unknownsCount/2 - 2 : unknownsCount/2 - 3;
            var tau = even ? 0 : 2;
            var eta = even ? -4 : 1;
            var upsilon = even ? unknownsCount - 1 : unknownsCount - 2;
            //dlast = eta*dlast;
            var rs = new double[equationsCount];
            var oneDiv7 = 1/7;
            var hx = knots[1, 0].X - knots[0, 0].X;
            var hy = knots[0, 1].Y - knots[0, 0].Y;
            var oneDivHx = 1d/hx;
            var oneDivHy = 1d/hy;
            var threeDivHx = 3/hx;
            //var twelweDivHx = threeDivHx * 4;
            var threeDivHy = 3/hy;
            //var twelweDivHy = threeDivHy * 4;
            var threeDiv7hx = oneDiv7*threeDivHx;
            var threeDiv7hy = oneDiv7*threeDivHy;
            var threeDiv7hxhy = threeDiv7hy/hy;
            var rows = knots.Rows;
            var columns = knots.Columns;

            rs[0] = oneDiv7*(knots[0, 6].Dxy + knots[0, 2].Dxy) - 2*knots[1, 4].Dxy
                    + threeDiv7hx*(knots[1, 6].Dy + knots[1, 2].Dy) + threeDiv7hy*(-knots[0, 6].X + knots[0, 2].X)
                    + 3*threeDivHx*(knots[i, 6].Dy + knots[i, 2].Dy) + 3*threeDiv7hxhy*(-knots[0, 6].Z + knots[0, 2].Z)
                    + 4*threeDiv7hx*(-knots[1, 6].Dy - knots[1, 2].Dy) + 4*threeDiv7hy*(knots[0, 5].X - knots[0, 3].X)
                    + threeDiv7hy*(knots[i, 6].Dx + knots[i, 2].Dx) + 9*threeDiv7hxhy*(-knots[i, 6].Z + knots[i, 2].Z)
                    - 12*threeDiv7hxhy*(knots[1, 6].Z - knots[1, 2].Z + knots[0, 5].Z - knots[0, 3].Z)
                    - 6*oneDivHx*knots[0, 4].Dy + 12*oneDivHy*(knots[i, 5].Dx + knots[i, 3].Dx) +
                    36*threeDiv7hxhy*(knots[i, 5].Z + knots[i, 3].Z)
                    - 18*oneDivHx*knots[i, 4].Dy + 48*threeDiv7hxhy*(-knots[1, 5].Z + knots[1, 3].Z) +
                    24*oneDivHx*knots[1, 4].Z - dfirst;
            rs[equationsCount - 1] = oneDiv7*(knots[rows - 5, columns - 1].Dxy + knots[rows - 5, columns - 5].Dxy) -
                                     2*knots[rows - 4, columns - 3].Dxy
                                     + threeDiv7hx*(knots[rows - 4, columns - 1].Dy + knots[rows - 4, columns - 5].Dy) +
                                     threeDiv7hy*(-knots[rows - 5, columns - 1].X + knots[rows - 5, columns - 5].X)
                                     + 3*threeDivHx*(knots[rows - 3, columns - 1].Dy + knots[rows - 3, columns - 5].Dy) +
                                     3*threeDiv7hxhy*(-knots[rows - 5, columns - 1].Z + knots[rows - 5, columns - 5].Z)
                                     +
                                     4*threeDiv7hx*(-knots[rows - 4, columns - 1].Dy - knots[rows - 4, columns - 5].Dy) +
                                     4*threeDiv7hy*(knots[rows - 5, columns - 2].X - knots[rows - 5, columns - 4].X)
                                     + threeDiv7hy*(knots[rows - 3, columns - 1].Dx + knots[rows - 3, columns - 5].Dx) +
                                     9*threeDiv7hxhy*(-knots[rows - 3, columns - 1].Z + knots[rows - 3, columns - 5].Z)
                                     -
                                     12*threeDiv7hxhy*
                                     (knots[rows - 4, columns - 1].Z - knots[rows - 4, columns - 5].Z +
                                      knots[rows - 5, columns - 2].Z - knots[rows - 5, columns - 4].Z)
                                     - 6*oneDivHx*knots[rows - 5, columns - 3].Dy +
                                     12*oneDivHy*(knots[rows - 3, columns - 2].Dx + knots[rows - 3, columns - 4].Dx) +
                                     36*threeDiv7hxhy*(knots[rows - 3, columns - 2].Z + knots[rows - 3, columns - 4].Z)
                                     - 18*oneDivHx*knots[rows - 3, columns - 3].Dy +
                                     48*threeDiv7hxhy*(-knots[rows - 4, columns - 2].Z + knots[rows - 4, columns - 4].Z) +
                                     24*oneDivHx*knots[rows - 4, columns - 3].Z - eta*dlast;
            for (int k = 1, j = 6; k < equationsCount - 1; k++,j += 2)
            {
                //var i2 = i * 2;
                rs[k] = oneDiv7*(knots[i - 2, j + 2].Dxy + knots[i - 2, j - 2].Dxy) - 2*knots[i - 1, j].Dxy
                        + threeDiv7hx*(knots[i - 1, j + 2].Dy + knots[i - 1, j - 2].Dy) +
                        threeDiv7hy*(-knots[i - 2, j + 2].X + knots[i - 2, j - 2].X)
                        + 3*threeDivHx*(knots[i, j + 2].Dy + knots[i, j - 2].Dy) +
                        3*threeDiv7hxhy*(-knots[i - 2, j + 2].Z + knots[i - 2, j - 2].Z)
                        + 4*threeDiv7hx*(-knots[i - 1, j + 2].Dy - knots[i - 1, j - 2].Dy) +
                        4*threeDiv7hy*(knots[i - 2, j + 1].X - knots[i - 2, j - 1].X)
                        + threeDiv7hy*(knots[i, j + 2].Dx + knots[i, j - 2].Dx) +
                        9*threeDiv7hxhy*(-knots[i, j + 2].Z + knots[i, j - 2].Z)
                        -
                        12*threeDiv7hxhy*
                        (knots[i - 1, j + 2].Z - knots[i - 1, j - 2].Z + knots[i - 2, j + 1].Z - knots[i - 2, j - 1].Z)
                        - 6*oneDivHx*knots[i - 2, j].Dy + 12*oneDivHy*(knots[i, j + 1].Dx + knots[i, j - 1].Dx) +
                        36*threeDiv7hxhy*(knots[i, j + 1].Z + knots[i, j - 1].Z)
                        - 18*oneDivHx*knots[i, j].Dy + 48*threeDiv7hxhy*(-knots[i - 1, j + 1].Z + knots[i - 1, j - 1].Z) +
                        24*oneDivHx*knots[i - 1, j].Z;
            }
            return rs;
        }

        private void FillXDerivations(KnotMatrix values)
        {
            for (var j = 0; j < values.Columns; j++)
            {
                FillXDerivations(j, values);
            }
            var h = values[1, 0].X - values[0, 0].X;
            for (var i = 1; i < values.Rows - 1; i += 2)
            {
                for (var j = 1; j < values.Columns - 1; j += 2)
                {
                    values[i, j].Dx = 0.75*h*(values[i + 1, j].Z - values[i - 1, j].Z)
                                      - 0.25*(values[i + 1, j].Dx + values[i - 1, j].Dx);
                }
            }
        }

        private void FillYDerivations(KnotMatrix values)
        {
            for (var i = 0; i < values.Rows; i++)
            {
                FillYDerivations(i, values);
            }
            var h = values[0, 1].X - values[0, 0].X;
            for (var j = 1; j < values.Columns - 1; j += 2)
            {
                for (var i = 1; i < values.Rows - 1; i += 2)
                {
                    values[i, j].Dx = 0.75*h*(values[i, j + 1].Z - values[i, j - 1].Z)
                                      - 0.25*(values[i, j + 1].Dy + values[i, j + 1].Dy);
                }
            }
        }

        private void FillYXDerivations(KnotMatrix values)
        {
            for (var i = 2; i < values.Rows - 1; i += 2)
            {
                FillYXDerivations(i, values);
            }

            var oneDiv16 = 1d/16d;
            var threeDiv16 = oneDiv16*3;
            var hx = values[1, 0].X - values[0, 0].X;
            var hy = values[0, 1].Y - values[0, 0].Y;
            var oneDivHx = 1d/hx;
            var oneDivHy = 1d/hy;

            for (var i = 1; i < values.Rows - 1; i += 2)
            {
                for (var j = 1; j < values.Columns - 1; j += 2)
                {
                    values[i, j].Dxy = oneDiv16*
                                       (values[i + 1, j + 1].Dxy + values[i + 1, j - 1].Dxy + values[i - 1, j + 1].Dxy +
                                        values[i - 1, j - 1].Dxy)
                                       -
                                       threeDiv16*oneDivHy*
                                       (values[i + 1, j + 1].Dx + values[i + 1, j - 1].Dx + values[i - 1, j + 1].Dx +
                                        values[i - 1, j - 1].Dx)
                                       -
                                       threeDiv16*oneDivHx*
                                       (values[i + 1, j + 1].Dy + values[i + 1, j - 1].Dy + values[i - 1, j + 1].Dy +
                                        values[i - 1, j - 1].Dy)
                                       +
                                       3*threeDiv16*oneDivHx*oneDivHy*
                                       (values[i + 1, j + 1].Dy - values[i + 1, j - 1].Dy - values[i - 1, j + 1].Dy*
                                        values[i - 1, j - 1].Dy);
                }
            }

            for (var i = 1; i < values.Rows - 1; i += 2)
            {
                for (var j = 2; j < values.Columns - 2; j += 2)
                {
                    values[i, j].Dxy = 0.75*oneDivHy*(values[i, j + 1].Dx - values[i, j - 1].Dx)
                                       - 0.25*(values[i, j + 1].Dxy - values[i, j - 1].Dxy);
                }
            }

            for (var i = 2; i < values.Rows - 2; i += 2)
            {
                for (var j = 1; j < values.Columns - 2; j += 2)
                {
                    values[i, j].Dxy = 0.75*oneDivHy*(values[i, j + 1].Dx - values[i, j - 1].Dx)
                                       - 0.25*(values[i, j + 1].Dxy - values[i, j - 1].Dxy);
                }
            }
        }

        private void FillXYDerivations(KnotMatrix values)
        {
            _initializator.FillXYDerivations(0, values);
            _initializator.FillXYDerivations(values.Columns - 1, values);
            _initializator.FillYXDerivations(0, values);
            _initializator.FillYXDerivations(values.Rows - 1, values);
        }

        private void FillYXDerivations(int rowIndex, KnotMatrix values)
        {
            var unknownsCount = values.Columns;// - 2;
            if (unknownsCount == 2) return;
            //Action<int, double> dset = (idx, value) => values[rowIndex, idx].Dxy = value;
            //Func<int, double> rget = idx => values[rowIndex, idx].Dx;
            var h = values[0, 1].Y - values[0, 0].Y;
            var dfirst = values[rowIndex, 0].Dxy;
            var dlast = values[rowIndex, values.Columns - 1].Dxy;

            var result = RightSideCross(values, rowIndex, dfirst, dlast, unknownsCount);
            LinearSystemSolver.TridiagonalSystem(_initializator.UpperDiagonal(unknownsCount), MainDiagonal(unknownsCount, values.Columns%2==0),
                _initializator.LowerDiagonal(unknownsCount), result);

            for (var i = 0; i < result.Length; i++)
            {
                values[rowIndex, i+1].Dxy=result[i];
            }
        }

        private void FillYDerivations(int rowIndex, KnotMatrix values)
        {
            var unknownsCount = values.Columns/2 - 1;
            if (unknownsCount == 0) return;
            Action<int, double> dset = (idx, value) => values[rowIndex, idx].Dy = value;
            Func<int, double> rget = idx => values[rowIndex, idx].Z;
            var h = values[0, 1].Y - values[0, 0].Y;
            var dfirst = values[rowIndex, 0].Dy;
            var dlast = values[rowIndex, values.Columns - 1].Dy;

           SolveTridiagonal(rget, h, dfirst, dlast, unknownsCount, values.Columns % 2 == 0, dset);
        }

        private void SolveTridiagonal(Func<int, double> rightSideValuesToGet, double h, double dfirst, double dlast, int unknownsCount,bool even, Action<int, double> unknownsToSet)
        {
            var result = RightSide(rightSideValuesToGet, h, dfirst, dlast, unknownsCount,even);
            LinearSystemSolver.TridiagonalSystem(_initializator.UpperDiagonal(unknownsCount), MainDiagonal(unknownsCount,even),
                _initializator.LowerDiagonal(unknownsCount), result);

            for (var i = 0; i < result.Length; i++)
            {
                unknownsToSet(i + 1, result[i]);
            }
        }

        //private void FillXYDerivations(int rowIndex, KnotMatrix values)
        //{
        //    var unknownsCount = values.Rows - 2;
        //    if (unknownsCount == 0) return;
        //    Action<int, double> dset = (i, value) => values[i, rowIndex].Dxy = value;
        //    Func<int, double> rget = i => values[i, rowIndex].Dy;
        //    var h = values[1, 0].X - values[0, 0].X;
        //    var dlast = values[values.Rows - 1, rowIndex].Dxy;
        //    var dfirst = values[0, rowIndex].Dxy;

        //    SolveTridiagonal(rget, h, dfirst, dlast, unknownsCount, dset);
        //}

        private void FillXDerivations(int columnIndex, KnotMatrix values)
        {
            //var unknownsCount = values.Rows%2 == 0 ? values.Rows/2 - 0 : values.Rows/2 - 1;
            var unknownsCount = values.Rows / 2 - 1;
            if (unknownsCount == 0) return;
            Action<int, double> dset = (idx, value) => values[idx, columnIndex].Dx = value;
            Func<int, double> rget = idx => values[idx, columnIndex].Z;
            var h = values[1, 0].X - values[0, 0].X;
            var dlast = values[values.Rows - 1, columnIndex].Dx;
            var dfirst = values[0, columnIndex].Dx;

            SolveTridiagonal(rget, h, dfirst, dlast, unknownsCount, values.Rows % 2==0, dset);
        }

        protected struct EquationParams
        {
            /// <summary>
            /// </summary>
            /// <param name="mu">Matrix parameter</param>
            /// <param name="tau">Right side index parameter</param>
            /// <param name="eta">Right side parameter</param>
            /// <param name="upsilon">Number of equations</param>
            //public EquationParams(int mu, int tau, int eta, int upsilon) : this()
            //{
            //    Mu = mu;
            //    Tau = tau;
            //    Eta = eta;
            //    Upsilon = upsilon;
            //}
            public EquationParams(bool even) : this()
            {
                if (even)
                {
                    Mu = 15;
                    Tau = 0;
                    Eta = -4;
                    //Upsilon = length;
                }
                else
                {
                    Mu = -14;
                    Tau = 2;
                    Eta = 1;
                    //Upsilon = length - 1;
                }
            }

            public int Mu { get; private set; }
            public int Tau { get; private set; }
            public int Eta { get; private set; }
        }
    }
}