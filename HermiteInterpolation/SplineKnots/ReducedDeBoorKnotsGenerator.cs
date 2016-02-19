using System;
using HermiteInterpolation.Numerics;
using HermiteInterpolation.Numerics.MathFunctions;
using HermiteInterpolation.Shapes.SplineInterpolation;
using HermiteInterpolation.Utils;

namespace HermiteInterpolation.SplineKnots
{
    public sealed class ReducedDeBoorKnotsGenerator : DeBoorKnotsGenerator
    {
        public ReducedDeBoorKnotsGenerator(InterpolativeMathFunction function) : base(function)
        {
        }

        public ReducedDeBoorKnotsGenerator(MathExpression expression) : base(expression)
        {
        }

        public override KnotMatrix GenerateKnots(SurfaceDimension uDimension, SurfaceDimension vDimension)
        {
            if (uDimension.KnotCount < 6 || vDimension.KnotCount < 6)
            {
                return new DeBoorKnotsGenerator(Function).GenerateKnots(uDimension, vDimension);
            }

            var values = new KnotMatrix(uDimension.KnotCount, vDimension.KnotCount);
            InitializeKnots(uDimension, vDimension, values);

            FillXDerivations(values);
            FillYDerivations(values);
            FillXYDerivations(values);
            FillYXDerivations(values);

            return values;
        }


        protected override double[] MainDiagonal(int unknownsCount)
        {
            var diag = MyArrays.InitalizedArray<double>(unknownsCount % 2 == 0 ? unknownsCount / 2 - 1 : unknownsCount / 2, -14);
            diag[diag.Length - 1] = unknownsCount % 2 == 0 ? -15 : -14;
            return diag;
        }

        protected override double[] LowerDiagonal(int unknownsCount)
        {
            return base.LowerDiagonal(unknownsCount % 2 == 0 ? unknownsCount / 2 - 1 : unknownsCount / 2);
        }

        protected override double[] UpperDiagonal(int unknownsCount)
        {
            return base.UpperDiagonal(unknownsCount % 2 == 0 ? unknownsCount / 2 - 1 : unknownsCount / 2);
        }

        protected override void FillXDerivations(KnotMatrix values)
        {
            var h = values[1, 0].X - values[0, 0].X;
            var oneDivH = 1 / h;
            var threeDiv4H = 0.75 * oneDivH;
            for (var j = 0; j < values.Columns; j++)
            {
                FillXDerivations(j, values);
                for (var i = 1; i < values.Rows - 1; i += 2)
                {
                    values[i, j].Dx = threeDiv4H * (values[i + 1, j].Z - values[i - 1, j].Z)
                                      - 0.25 * (values[i + 1, j].Dx + values[i - 1, j].Dx);
                }
            }
        }

        protected override void FillYDerivations(KnotMatrix values)
        {
            var h = values[0, 1].Y - values[0, 0].Y;
            var oneDivH = 1 / h;
            var threeDiv4H = 0.75 * oneDivH;
            for (var i = 0; i < values.Rows; i++)
            {
                FillYDerivations(i, values);
                //for (var j = 1; j < values.Columns - 1; j += 2)
                //{
                //    values[i, j].Dy = threeDiv4H * (values[i, j + 1].Z - values[i, j - 1].Z)
                //                      - 0.25 * (values[i, j + 1].Dy + values[i, j - 1].Dy);
                //}
            }

            for (var i = 0; i < values.Rows; i++)
            {
                for (var j = 1; j < values.Columns - 1; j += 2)
                {
                    values[i, j].Dy = threeDiv4H * (values[i, j + 1].Z - values[i, j - 1].Z)
                                      - 0.25 * (values[i, j + 1].Dy + values[i, j - 1].Dy);
                }
            }
        }

        protected override void FillYXDerivations(KnotMatrix values)
        {
            for (var i = 2; i < values.Rows - 1; i += 2)
            {
                FillYXDerivations(i, values);
            }

            var oneDiv16 = 1d / 16d;
            var threeDiv16 = oneDiv16 * 3;
            var hx = values[1, 0].X - values[0, 0].X;
            var hy = values[0, 1].Y - values[0, 0].Y;
            var oneDivHx = 1d / hx;
            var oneDivHy = 1d / hy;

            for (var i = 1; i < values.Rows - 1; i += 2)
            {
                for (var j = 1; j < values.Columns - 1; j += 2)
                {
                    values[i, j].Dxy = oneDiv16 *
                                       (values[i + 1, j + 1].Dxy + values[i + 1, j - 1].Dxy + values[i - 1, j + 1].Dxy +
                                        values[i - 1, j - 1].Dxy)
                                       -
                                       threeDiv16 * oneDivHy *
                                       (values[i + 1, j + 1].Dx + values[i + 1, j - 1].Dx + values[i - 1, j + 1].Dx +
                                        values[i - 1, j - 1].Dx)
                                       -
                                       threeDiv16 * oneDivHx *
                                       (values[i + 1, j + 1].Dy + values[i + 1, j - 1].Dy + values[i - 1, j + 1].Dy +
                                        values[i - 1, j - 1].Dy)
                                       +
                                       3 * threeDiv16 * oneDivHx * oneDivHy *
                                       (values[i + 1, j + 1].Dy - values[i + 1, j - 1].Dy - values[i - 1, j + 1].Dy *
                                        values[i - 1, j - 1].Dy);
                }
            }

            for (var j = 2; j < values.Columns - 2; j += 2)
            {
                values[1, j].Dxy = 0.75 * oneDivHy * (values[1, j + 1].Dx - values[1, j - 1].Dx)
                    - 0.25 * (values[1, j + 1].Dxy - values[1, j - 1].Dxy);
            }

            for (var i = 2; i < values.Rows - 2; i += 2)
            {
                for (var j = 2; j < values.Columns - 2; j += 2)
                {
                    values[i + 1, j].Dxy = 0.75 * oneDivHy * (values[i + 1, j + 1].Dx - values[i + 1, j - 1].Dx)
                        - 0.25 * (values[i + 1, j + 1].Dxy - values[i + 1, j - 1].Dxy);
                }
                for (var j = 1; j < values.Columns - 2; j += 2)
                {
                    values[i, j].Dxy = 0.75 * oneDivHy * (values[i, j + 1].Dx - values[i, j - 1].Dx)
                        - 0.25 * (values[i, j + 1].Dxy - values[i, j - 1].Dxy);
                }
            }
        }

        protected override void FillXYDerivations(KnotMatrix values)
        {
            base.FillXYDerivations(0, values,
                LowerDiagonal(values.Rows-2), MainDiagonal(values.Rows - 2), UpperDiagonal(values.Rows - 2));
            base.FillXYDerivations(values.Columns - 1, values,
                LowerDiagonal(values.Rows - 2), MainDiagonal(values.Rows - 2), UpperDiagonal(values.Rows - 2));
            base.FillYXDerivations(0, values,
                LowerDiagonal(values.Columns - 2), MainDiagonal(values.Columns - 2), UpperDiagonal(values.Columns - 2));
            base.FillYXDerivations(values.Rows - 1, values,
                 LowerDiagonal(values.Columns - 2), MainDiagonal(values.Columns - 2), UpperDiagonal(values.Columns - 2));
        }

        protected override void FillYXDerivations(int rowIndex, KnotMatrix values,
             double[] lowerDiagonal = null, double[] mainDiagonal = null, double[] upperDiagonal = null)
        {

            var unknownsCount = values.Columns - 2;
            if (unknownsCount == 0) return;
            var h = values[0, 1].Y - values[0, 0].Y;
            var dfirst = values[rowIndex, 0].Dxy;
            var dlast = values[rowIndex, values.Columns - 1].Dxy;
            lowerDiagonal = lowerDiagonal ?? LowerDiagonal(unknownsCount);
            mainDiagonal = mainDiagonal ?? MainDiagonal(unknownsCount);
            upperDiagonal = upperDiagonal ?? UpperDiagonal(unknownsCount);
            var result = RightSideCross(values, rowIndex, dfirst, dlast, unknownsCount);
            LinearSystems.SolveTridiagonalSystem(lowerDiagonal, mainDiagonal,
                upperDiagonal, result);

            for (var i = 0; i < result.Length; i++)
            {
                values[rowIndex, 2 * i + 1].Dxy = result[i];
            }
        }


        protected override double[] RightSide(Func<int, double> rightSideVariables, double h, double dfirst, double dlast,
            int unknownsCount)
        {
            var even = unknownsCount % 2 == 0;
            var tau = even ? 0 : 2;
            var eta = even ? -4 : 1;
            var upsilon = even ? unknownsCount : unknownsCount - 1;
            int equationsCount = even ? unknownsCount / 2 - 1 : unknownsCount / 2;
            var rs = new double[equationsCount];
            var threeDivH = 3 / h;
            var twelveDivH = threeDivH * 4;
            rs[0] = threeDivH * (rightSideVariables(4) - rightSideVariables(0)) - twelveDivH * (rightSideVariables(3) - rightSideVariables(1)) - dfirst;

            rs[equationsCount - 1] = threeDivH *
                                     (rightSideVariables(upsilon + tau) -
                                      rightSideVariables(upsilon - 2))
                                     -
                                     twelveDivH *
                                     (rightSideVariables(upsilon + 1) - rightSideVariables(upsilon - 1)) -
                                      eta * dlast;
            for (var k = 2; k < equationsCount; k++)
            {
                var k2 = k * 2;
                rs[k - 1] = threeDivH * (rightSideVariables(2 * (k + 1)) - rightSideVariables(2 * (k - 1)) - twelveDivH * (rightSideVariables(k2 + 1) - rightSideVariables(k2 - 1)));
            }

            //I do not know (yet) why but these must be half of values designed by L. Mino
            //This cycle shouldn't be here
            for (int i = 1; i < rs.Length - 1; i++)
            {
                rs[i] *= 0.5;
            }

            return rs;
        }

        private double[] RightSideCross(KnotMatrix knots, int i, double dfirst, double dlast,
            int unknownsCount)
        {

            var even = unknownsCount % 2 == 0;
            var equationsCount = even ? unknownsCount / 2 - 1 : unknownsCount / 2;
            var eta = even ? -4 : 1;
            var rs = new double[equationsCount];
            var oneDiv7 = 1 / 7;
            var hx = knots[1, 0].X - knots[0, 0].X;
            var hy = knots[0, 1].Y - knots[0, 0].Y;
            var oneDivHx = 1d / hx;
            var oneDivHy = 1d / hy;
            var threeDivHx = 3 / hx;
            var threeDivHy = 3 / hy;
            var threeDiv7hx = oneDiv7 * threeDivHx;
            var threeDiv7hy = oneDiv7 * threeDivHy;
            var threeDiv7hxhy = threeDiv7hy / hy;

            var columns = knots.Columns;

            var iMin1 = i - 1;
            var iMin2 = i - 2;

            for (int k = 0, j = 4; k < equationsCount - 1; k++, j += 2)
            {
                rs[k] = oneDiv7 * (knots[iMin2, j + 2].Dxy + knots[iMin2, j - 2].Dxy) - 2 * knots[iMin1, j].Dxy
                        + threeDiv7hx * (knots[iMin1, j + 2].Dy + knots[iMin1, j - 2].Dy) +
                        threeDiv7hy * (-knots[iMin2, j + 2].X + knots[iMin2, j - 2].X)
                        + 3 * threeDivHx * (knots[i, j + 2].Dy + knots[i, j - 2].Dy) +
                        3 * threeDiv7hxhy * (-knots[iMin2, j + 2].Z + knots[iMin2, j - 2].Z)
                        + 4 * threeDiv7hx * (-knots[iMin1, j + 2].Dy - knots[iMin1, j - 2].Dy) +
                        4 * threeDiv7hy * (knots[iMin2, j + 1].X - knots[iMin2, j - 1].X)
                        + threeDiv7hy * (knots[i, j + 2].Dx + knots[i, j - 2].Dx) +
                        9 * threeDiv7hxhy * (-knots[i, j + 2].Z + knots[i, j - 2].Z)
                        -
                        12 * threeDiv7hxhy *
                        (knots[iMin1, j + 2].Z - knots[iMin1, j - 2].Z + knots[iMin2, j + 1].Z - knots[iMin2, j - 1].Z)
                        - 6 * oneDivHx * knots[iMin2, j].Dy + 12 * oneDivHy * (knots[i, j + 1].Dx + knots[i, j - 1].Dx) +
                        36 * threeDiv7hxhy * (knots[i, j + 1].Z + knots[i, j - 1].Z)
                        - 18 * oneDivHx * knots[i, j].Dy + 48 * threeDiv7hxhy * (-knots[iMin1, j + 1].Z + knots[iMin1, j - 1].Z) +
                        24 * oneDivHx * knots[iMin1, j].Z;
            }
            rs[0] -= dfirst;
            rs[equationsCount - 1] = oneDiv7 * (knots[iMin2, columns - 1].Dxy + knots[iMin2, columns - 5].Dxy) -
                                     2 * knots[iMin1, columns - 3].Dxy
                                     + threeDiv7hx * (knots[iMin1, columns - 1].Dy + knots[iMin1, columns - 5].Dy) +
                                     threeDiv7hy * (-knots[iMin2, columns - 1].X + knots[iMin2, columns - 5].X)
                                     + 3 * threeDivHx * (knots[i, columns - 1].Dy + knots[i, columns - 5].Dy) +
                                     3 * threeDiv7hxhy * (-knots[iMin2, columns - 1].Z + knots[iMin2, columns - 5].Z)
                                     +
                                     4 * threeDiv7hx * (-knots[iMin1, columns - 1].Dy - knots[iMin1, columns - 5].Dy) +
                                     4 * threeDiv7hy * (knots[iMin2, columns - 2].X - knots[iMin2, columns - 4].X)
                                     + threeDiv7hy * (knots[i, columns - 1].Dx + knots[i, columns - 5].Dx) +
                                     9 * threeDiv7hxhy * (-knots[i, columns - 1].Z + knots[i, columns - 5].Z)
                                     -
                                     12 * threeDiv7hxhy *
                                     (knots[iMin1, columns - 1].Z - knots[iMin1, columns - 5].Z +
                                      knots[iMin2, columns - 2].Z - knots[iMin2, columns - 4].Z)
                                     - 6 * oneDivHx * knots[iMin2, columns - 3].Dy +
                                     12 * oneDivHy * (knots[i, columns - 2].Dx + knots[i, columns - 4].Dx) +
                                     36 * threeDiv7hxhy * (knots[i, columns - 2].Z + knots[i, columns - 4].Z)
                                     - 18 * oneDivHx * knots[i, columns - 3].Dy +
                                     48 * threeDiv7hxhy * (-knots[iMin1, columns - 2].Z + knots[iMin1, columns - 4].Z) +
                                     24 * oneDivHx * knots[iMin1, columns - 3].Z - eta * dlast;
            return rs;
        }

        protected override void SolveTridiagonal(Func<int, double> rightSideValuesSelector, double h, double dfirst, double dlast, int unknownsCount, Action<int, double> unknownsSetter,
            double[] lowerDiagonal = null, double[] mainDiagonal = null, double[] upperDiagonal = null)
        {
            if (lowerDiagonal != null && mainDiagonal != null && upperDiagonal != null)
            {
                base.SolveTridiagonal(rightSideValuesSelector,h,dfirst,dlast,unknownsCount,unknownsSetter,lowerDiagonal,mainDiagonal,upperDiagonal);
                return;
            }
            lowerDiagonal = lowerDiagonal ?? LowerDiagonal(unknownsCount);
            mainDiagonal = mainDiagonal ?? MainDiagonal(unknownsCount);
            upperDiagonal = upperDiagonal ?? UpperDiagonal(unknownsCount);
            var result = RightSide(rightSideValuesSelector, h, dfirst, dlast, unknownsCount);
            LinearSystems.SolveTridiagonalSystem(lowerDiagonal, mainDiagonal,
                upperDiagonal, result);

            for (int k = 0; k < result.Length; k++)
            {
                unknownsSetter(2 * (k + 1), result[k]);
            }
        }
    }
}