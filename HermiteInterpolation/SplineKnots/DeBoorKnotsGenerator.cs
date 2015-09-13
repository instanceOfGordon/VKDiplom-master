using System;
using HermiteInterpolation.MathFunctions;
using HermiteInterpolation.Numerics;
using HermiteInterpolation.Shapes.SplineInterpolation;
using HermiteInterpolation.Utils;

namespace HermiteInterpolation.SplineKnots
{
    public class DeBoorKnotsGenerator : KnotsGenerator
    {
       

        protected virtual double[] MainDiagonal(int equationsCount, bool even = false)
        {
            return MyArrays.InitalizedArray<double>(equationsCount, 4);
        }

        protected virtual double[] UpperDiagonal(int equationsCount)
        {
            return MyArrays.InitalizedArray<double>(equationsCount, 1);
        }

        protected virtual double[] LowerDiagonal(int equationsCount)
        {
            return MyArrays.InitalizedArray<double>(equationsCount, 1);
        }

        protected virtual double[] RightSide(Func<int, double> rightSide, double h, double dfirst, double dlast,
            int equationsCount, bool even = false)
        {
            var rs = new double[equationsCount];
            h = 3;///h;
            rs[0] = h*(rightSide(2) - rightSide(0)) - dfirst;
            rs[equationsCount - 1] = h*(rightSide(equationsCount + 1) - rightSide(equationsCount - 1)) - dlast;
            for (var i = 1; i < equationsCount - 1; i++)
            {
                rs[i] = h*(rightSide(i + 2) - rightSide(i));
            }
            return rs;
        }

        public override Knot[][] GenerateKnots(SurfaceDimension uDimension, SurfaceDimension vDimension)
        {
            if (uDimension.KnotCount < 4 || vDimension.KnotCount < 4)
            {
                return new DirectKnotsGenerator(Function).GenerateKnots(uDimension, vDimension);
            }

            var values = MyArrays.JaggedArray<Knot>(uDimension.KnotCount, vDimension.KnotCount);
            var uSize = Math.Abs(uDimension.Max - uDimension.Min)/(uDimension.KnotCount - 1);
            var vSize = Math.Abs(vDimension.Max - vDimension.Min)/(vDimension.KnotCount - 1);
            var u = uDimension.Min;

            // Init Z
            for (var i = 0; i < uDimension.KnotCount; i++, u += uSize)
                //Parallel.For(0,uDimension.KnotCount,i=>)
            {
                var v = vDimension.Min;
                for (var j = 0; j < vDimension.KnotCount; j++, v += vSize)
                {
                    var z = MathFunctions.MathFunctions.SafeCall(Function.Z, u, v);//Function.Z(u,v); //Z(u, v);

                    values[i][j] = new Knot(u, v, z);
                }
            }
            // Init Dx
            var uKnotCountMin1 = uDimension.KnotCount - 1;
            for (var j = 0; j < vDimension.KnotCount; j++)
            {
                values[0][j].Dx = MathFunctions.MathFunctions.SafeCall(Function.Dx, values[0][j].X, values[0][j].Y); //Function.Dx(values[0][j].X, values[0][j].Y);
                values[uKnotCountMin1][j].Dx = MathFunctions.MathFunctions.SafeCall(Function.Dx, values[uKnotCountMin1][j].X,
                    values[uKnotCountMin1][j].Y);
                //Function.Dx(values[uKnotCountMin1][j].X, values[uKnotCountMin1][j].Y);
            }
            // Init Dy
            var vKnotCountMin1 = vDimension.KnotCount - 1;
            for (var i = 0; i < uDimension.KnotCount; i++)
            {
                values[i][0].Dy = MathFunctions.MathFunctions.SafeCall(Function.Dy, values[i][0].X, values[i][0].Y);
                values[i][vKnotCountMin1].Dy = MathFunctions.MathFunctions.SafeCall(Function.Dy, values[i][vKnotCountMin1].X,
                    values[i][vKnotCountMin1].Y);
            }
            // Init Dxy
            values[0][0].Dxy = MathFunctions.MathFunctions.SafeCall(Function.Dxy, values[0][0].X, values[0][0].Y);
            values[uKnotCountMin1][0].Dxy = MathFunctions.MathFunctions.SafeCall(Function.Dxy, values[uKnotCountMin1][0].X,
                values[uKnotCountMin1][0].Y);
            values[0][vKnotCountMin1].Dxy = MathFunctions.MathFunctions.SafeCall(Function.Dxy, values[0][vKnotCountMin1].X,
                values[0][vKnotCountMin1].Y);
            values[uKnotCountMin1][vKnotCountMin1].Dxy = MathFunctions.MathFunctions.SafeCall(Function.Dxy,
                values[uKnotCountMin1][vKnotCountMin1].X, values[uKnotCountMin1][vKnotCountMin1].Y);

            FillXDerivations(values);
            FillDxyDerivations(values);
            FillYDerivations(values);
            FillDyxDerivations(values);

            return values;
        }

        protected void FillXDerivations(Knot[][] values)
        {
            for (var j = 0; j < values[0].Length; j++)
            {
                FillXDerivations(j, values);
            }
        }

        protected void FillDxyDerivations(Knot[][] values)
        {
            FillDxyDerivations(0, values);
            FillDxyDerivations(values[0].Length - 1, values);
        }

        protected void FillYDerivations(Knot[][] values)
        {
            for (var i = 0; i < values.Length; i++)
            {
                FillYDerivations(i, values);
            }
        }

        protected void FillDyxDerivations(Knot[][] values)
        {
            for (var i = 0; i < values.Length; i++)
            {
                FillDyxDerivations(i, values);
            }
        }

        protected virtual void FillXDerivations(int rowOrColumnIdx, Knot[][] values)
        {
            var equationsCount = values.Length - 2;
            if (equationsCount == 0) return;
            Action<int, double> dset = (idx, value) => values[idx][rowOrColumnIdx].Dx = value;
            Func<int, double> rget = idx => values[idx][rowOrColumnIdx].Z;
            var h = values[1][0].X - values[0][0].X;
            var dlast = values[values.Length - 1][rowOrColumnIdx].Dx;
            var dfirst = values[0][rowOrColumnIdx].Dx;

            //var equationsCount = values.Length - 2;
            //if (equationsCount == 0) return;
            //Action<int, double> unknownsToSet = (idx, value) => values[rowOrColumnIdx][idx].Dx = value;
            //Func<int, double> rightSideValuesToGet = idx => values[rowOrColumnIdx][idx].Z;
            //var h = values[1][0].X - values[0][0].X;
            //var dlast = values[rowOrColumnIdx][values[0].Length-1].Dx;
            //var dfirst = values[rowOrColumnIdx][0].Dx;

            SolveTridiagonal(rget, h, dfirst, dlast, equationsCount, dset);
        }

       

        protected virtual void FillYDerivations(int rowOrColumnIdx, Knot[][] values)
        {
            var equationsCount = values[0].Length - 2;
            if (equationsCount == 0) return;
            Action<int, double> dset = (idx, value) => values[rowOrColumnIdx][idx].Dy = value;
            Func<int, double> rget = idx => values[rowOrColumnIdx][idx].Z;
            var h = values[0][1].Y - values[0][0].Y;
            var dfirst = values[rowOrColumnIdx][0].Dy;
            var dlast = values[rowOrColumnIdx][values[0].Length - 1].Dy;

            //var equationsCount = values[0].Length - 2;
            //if (equationsCount == 0) return;
            //Action<int, double> unknownsToSet = (idx, value) => values[idx][rowOrColumnIdx].Dy = value;
            //Func<int, double> rightSideValuesToGet = idx => values[idx][rowOrColumnIdx].Z;
            //var h = values[0][1].Y - values[0][0].Y;
            //var dfirst = values[0][rowOrColumnIdx].Dy;
            //var dlast = values[values.Length-1][rowOrColumnIdx].Dy;

            SolveTridiagonal(rget, h, dfirst, dlast, equationsCount, dset);
        }

        protected virtual void FillDxyDerivations(int rowOrColumnIdx, Knot[][] values)
        {
            var equationsCount = values.Length - 2;
            if (equationsCount == 0) return;
            Action<int, double> dset = (idx, value) => values[idx][rowOrColumnIdx].Dxy = value;
            Func<int, double> rget = idx => values[idx][rowOrColumnIdx].Dy;
            var h = values[1][0].X - values[0][0].X;
            var dlast = values[values.Length - 1][rowOrColumnIdx].Dxy;
            var dfirst = values[0][rowOrColumnIdx].Dxy;

            //var equationsCount = values.Length - 2;
            //if (equationsCount == 0) return;
            //Action<int, double> unknownsToSet = (idx, value) => values[rowOrColumnIdx][idx].Dxy = value;
            //Func<int, double> rightSideValuesToGet = idx => values[rowOrColumnIdx][idx].Dx;
            //var h = values[1][0].X - values[0][0].X;
            //var dlast = values[rowOrColumnIdx][values[0].Length - 1].Dxy;
            //var dfirst = values[rowOrColumnIdx][0].Dxy;

            SolveTridiagonal(rget, h, dfirst, dlast, equationsCount, dset);
        }

        protected virtual void FillDyxDerivations(int rowOrColumnIdx, Knot[][] values)
        {
            var equationsCount = values[0].Length - 2;
            if (equationsCount == 0) return;
            Action<int, double> dset = (idx, value) => values[rowOrColumnIdx][idx].Dxy = value;
            Func<int, double> rget = idx => values[rowOrColumnIdx][idx].Dx;
            var h = values[0][1].Y - values[0][0].Y;
            var dfirst = values[rowOrColumnIdx][0].Dxy;
            var dlast = values[rowOrColumnIdx][values[0].Length - 1].Dxy;

            SolveTridiagonal(rget, h, dfirst, dlast, equationsCount, dset);
        }

        protected void SolveTridiagonal(Func<int, double> rightSideValuesToGet, double h, double dfirst, double dlast,
            int equationsCount, Action<int, double> unknownsToSet)
        {
            var result = RightSide(rightSideValuesToGet, h, dfirst, dlast, equationsCount);
            LinearSystemSolver.TridiagonalSystem(UpperDiagonal(equationsCount), MainDiagonal(equationsCount),
                LowerDiagonal(equationsCount), result);

            for (var i = 0; i < result.Length; i++)
            {
                unknownsToSet(i + 1, result[i]);
            }
        }

        public DeBoorKnotsGenerator(InterpolativeMathFunction function) : base(function)
        {
        }

        public DeBoorKnotsGenerator(MathExpression expression) : base(expression)
        {
        }
    }
}