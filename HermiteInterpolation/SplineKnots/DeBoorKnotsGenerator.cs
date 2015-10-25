using System;
using HermiteInterpolation.MathFunctions;
using HermiteInterpolation.Numerics;

using HermiteInterpolation.Shapes.SplineInterpolation;
using HermiteInterpolation.Utils;

namespace HermiteInterpolation.SplineKnots
{
    public class DeBoorKnotsGenerator : KnotsGenerator//,IDeBoor
    {
        protected virtual double[] MainDiagonal(int unknownsCount)
        {
            return MyArrays.InitalizedArray<double>(unknownsCount, 4);
        }

        protected virtual double[] UpperDiagonal(int unknownsCount)
        {
            return MyArrays.InitalizedArray<double>(unknownsCount, 1);
        }

        protected virtual double[] LowerDiagonal(int unknownsCount)
        {
            return MyArrays.InitalizedArray<double>(unknownsCount, 1);
        }

        protected virtual double[] RightSide(Func<int, double> rightSide, double h, double dfirst, double dlast,
            int unknownsCount)
        {
            //var unknownsCount = unknownsCount - 2;
            var rs = new double[unknownsCount];
            //var equation
            h = 3/h;
            rs[0] = h*(rightSide(2) - rightSide(0)) - dfirst;
            rs[unknownsCount - 1] = h*(rightSide(unknownsCount + 1) - rightSide(unknownsCount - 1)) - dlast;
            for (var i = 1; i < unknownsCount-1; i++)
            {
                rs[i] = h*(rightSide(i + 2) - rightSide(i));
            }
            return rs;
        }

        public override KnotMatrix GenerateKnots(SurfaceDimension uDimension, SurfaceDimension vDimension)
        {
            if (uDimension.KnotCount < 4 || vDimension.KnotCount < 4)
            {
                return new DirectKnotsGenerator(Function).GenerateKnots(uDimension, vDimension);
            }

            var values =new KnotMatrix(uDimension.KnotCount, vDimension.KnotCount);
            InitializeKnots(uDimension, vDimension, values);

            FillXDerivations(values);
            FillXYDerivations(values);
            FillYDerivations(values);
            FillYXDerivations(values);

            return values;
        }

        protected virtual void InitializeKnots(SurfaceDimension uDimension, SurfaceDimension vDimension, KnotMatrix values)
        {
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
                    var z = Numerics.MathFunctions.MathFunctionExtensions.SafeCall(Function.Z, u, v);
                        //Function.Z(u,v); //Z(u, v);

                    values[i, j] = new Knot(u, v, z);
                }
            }
            // Init Dx
            var uKnotCountMin1 = uDimension.KnotCount - 1;
            for (var j = 0; j < vDimension.KnotCount; j++)
            {
                values[0, j].Dx = Numerics.MathFunctions.MathFunctionExtensions.SafeCall(Function.Dx, values[0, j].X,
                    values[0, j].Y); //Function.Dx(values[0,j].X, values[0,j].Y);
                values[uKnotCountMin1, j].Dx = Numerics.MathFunctions.MathFunctionExtensions.SafeCall(Function.Dx,
                    values[uKnotCountMin1, j].X,
                    values[uKnotCountMin1, j].Y);
            }
            // Init Dy
            var vKnotCountMin1 = vDimension.KnotCount - 1;
            for (var i = 0; i < uDimension.KnotCount; i++)
            {
                values[i, 0].Dy = Numerics.MathFunctions.MathFunctionExtensions.SafeCall(Function.Dy, values[i, 0].X,
                    values[i, 0].Y);
                values[i, vKnotCountMin1].Dy = Numerics.MathFunctions.MathFunctionExtensions.SafeCall(Function.Dy,
                    values[i, vKnotCountMin1].X,
                    values[i, vKnotCountMin1].Y);
            }
            // Init Dxy
            values[0, 0].Dxy = Numerics.MathFunctions.MathFunctionExtensions.SafeCall(Function.Dxy, values[0, 0].X,
                values[0, 0].Y);
            values[uKnotCountMin1, 0].Dxy = Numerics.MathFunctions.MathFunctionExtensions.SafeCall(Function.Dxy,
                values[uKnotCountMin1, 0].X,
                values[uKnotCountMin1, 0].Y);
            values[0, vKnotCountMin1].Dxy = Numerics.MathFunctions.MathFunctionExtensions.SafeCall(Function.Dxy,
                values[0, vKnotCountMin1].X,
                values[0, vKnotCountMin1].Y);
            values[uKnotCountMin1, vKnotCountMin1].Dxy = Numerics.MathFunctions.MathFunctionExtensions.SafeCall(Function.Dxy,
                values[uKnotCountMin1, vKnotCountMin1].X, values[uKnotCountMin1, vKnotCountMin1].Y);
        }

        protected virtual void FillXDerivations(KnotMatrix values)
        {
            for (var j = 0; j < values.Columns; j++)
            {
                FillXDerivations(j, values);
            }
        }

        protected virtual void FillXYDerivations(KnotMatrix values)
        {
            FillXYDerivations(0, values);
            FillXYDerivations(values.Columns - 1, values);
        }

        protected virtual void FillYDerivations(KnotMatrix values)
        {
            for (var i = 0; i < values.Rows; i++)
            {
                FillYDerivations(i, values);
            }
        }

        protected virtual void FillYXDerivations(KnotMatrix values)
        {
            for (var i = 0; i < values.Rows; i++)
            {
                FillYXDerivations(i, values);
            }
        }

        protected virtual void FillXDerivations(int columnIndex, KnotMatrix values)
        {
            var unknownsCount = values.Rows - 2;
            if (unknownsCount == 0) return;
            Action<int, double> dset = (idx, value) => values[idx,columnIndex].Dx = value;
            Func<int, double> rget = idx => values[idx,columnIndex].Z;
            var h = values[1,0].X - values[0,0].X;
            var dlast = values[values.Rows - 1,columnIndex].Dx;
            var dfirst = values[0,columnIndex].Dx;

            SolveTridiagonal(rget, h, dfirst, dlast, unknownsCount, dset);
        }



        protected virtual void FillYDerivations(int rowIndex, KnotMatrix values)
        {
            var unknownsCount = values.Columns - 2;
            if (unknownsCount == 2) return;
            Action<int, double> dset = (idx, value) => values[rowIndex,idx].Dy = value;
            Func<int, double> rget = idx => values[rowIndex,idx].Z;
            var h = values[0,1].Y - values[0,0].Y;
            var dfirst = values[rowIndex,0].Dy;
            var dlast = values[rowIndex,values.Columns - 1].Dy;

            SolveTridiagonal(rget, h, dfirst, dlast, unknownsCount, dset);
        }

        protected virtual void FillXYDerivations(int columnIndex, KnotMatrix values)
        {
            var unknownsCount = values.Rows - 2;
            if (unknownsCount == 2) return;
            Action<int, double> dset = (idx, value) => values[idx,columnIndex].Dxy = value;
            Func<int, double> rget = idx => values[idx,columnIndex].Dy;
            var h = values[1,0].X - values[0,0].X;
            var dlast = values[values.Rows - 1,columnIndex].Dxy;
            var dfirst = values[0,columnIndex].Dxy;

            SolveTridiagonal(rget, h, dfirst, dlast, unknownsCount, dset);
        }

        protected virtual void FillYXDerivations(int rowIndex, KnotMatrix values)
        {
            var unknownsCount = values.Columns - 2;
            if (unknownsCount == 2) return;
            Action<int, double> dset = (idx, value) => values[rowIndex,idx].Dxy = value;
            Func<int, double> rget = idx => values[rowIndex,idx].Dx;
            var h = values[0,1].Y - values[0,0].Y;
            var dfirst = values[rowIndex,0].Dxy;
            var dlast = values[rowIndex,values.Columns - 1].Dxy;

            SolveTridiagonal(rget, h, dfirst, dlast, unknownsCount, dset);
        }

        protected virtual void SolveTridiagonal(Func<int, double> rightSideValuesToGet, double h, double dfirst, double dlast,
            int unknownsCount, Action<int, double> unknownsToSet)
        {
            var result = RightSide(rightSideValuesToGet, h, dfirst, dlast, unknownsCount);
            LinearSystems.SolveTridiagonalSystem(UpperDiagonal(unknownsCount), MainDiagonal(unknownsCount),
                LowerDiagonal(unknownsCount), result);

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