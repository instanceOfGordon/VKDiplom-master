
using System;
using HermiteInterpolation.MathFunctions;
using HermiteInterpolation.Numerics;
using HermiteInterpolation.Shapes.SplineInterpolation;
using HermiteInterpolation.Utils;

namespace HermiteInterpolation.SplineKnots
{
    public class ReducedDeBoorKnotsGenerator2 : DeBoorKnotsGenerator
    {
        public ReducedDeBoorKnotsGenerator2(InterpolativeMathFunction function) : base(function)
        {
        }

        public ReducedDeBoorKnotsGenerator2(MathExpression expression) : base(expression)
        {
        }

        public override KnotMatrix GenerateKnots(SurfaceDimension uDimension, SurfaceDimension vDimension)
        {
            if (uDimension.KnotCount < 4 || vDimension.KnotCount < 4)
            {
                return new DirectKnotsGenerator(Function).GenerateKnots(uDimension, vDimension);
            }

            var values = new KnotMatrix(uDimension.KnotCount, vDimension.KnotCount);
            //_rowsEven = uDimension.KnotCount % 2 == 0;
            //_columnsEven = vDimension.KnotCount % 2 == 0;

            InitializeKnots(uDimension, vDimension, values);

            FillXDerivations(values);
            FillYDerivations(values);
            FillXYDerivations(values);
            FillYXDerivations(values);

            return values;
        }
    }

    //{
    //    private readonly DeBoorKnotsGenerator _deBoor;
    //    public ReducedDeBoorKnotsGenerator(InterpolativeMathFunction function) : base(function)
    //    {
    //        _deBoor = new DeBoorKnotsGenerator(function);
    //    }

    //    public ReducedDeBoorKnotsGenerator(MathExpression expression)
    //    {
    //        _deBoor = new DeBoorKnotsGenerator(expression);
    //    }

    //    public override KnotMatrix GenerateKnots(SurfaceDimension uDimension, SurfaceDimension vDimension)
    //    {
    //        if (uDimension.KnotCount < 4 || vDimension.KnotCount < 4)
    //        {
    //            return new DirectKnotsGenerator(Function).GenerateKnots(uDimension, vDimension);
    //        }

    //        var values = new KnotMatrix(uDimension.KnotCount, vDimension.KnotCount);
    //        _deBoor.InitializeKnots(uDimension, vDimension, values);

    //        FillXDerivations(values);
    //        FillYDerivations(values);
    //        FillXYDerivations(values);
    //        FillYXDerivations(values);

    //        return values;
    //    }

    //    public void FillXDerivations(KnotMatrix values)
    //    {
    //        for (var j = 0; j < values.Columns; j++)
    //        {
    //            FillXDerivations(j, values);
    //        }
    //        var h = values[1, 0].X - values[0, 0].X;
    //        for (int i = 1; i < values.Rows - 1; i++)
    //        {

    //            for (int j = 1; j < values.Columns - 1; j++)
    //            {
    //                values[i, j].Dx = 0.75 * h * (values[i + 1, j].Z - values[i - 1, j].Z)
    //                    - 0.25 * (values[i + 1, j].Dx + values[i - 1, j].Dx);
    //            }
    //        }
    //    }

    //    private void FillXDerivations(int columnIndex, KnotMatrix values)
    //    {
    //        var equationsCount = values.Rows / 2 - 2;
    //        if (equationsCount == 0) return;
    //        Action<int, double> dset = (idx, value) => values[2 * idx, columnIndex].Dx = value;
    //        Func<int, double> rget = idx => values[2 * idx, columnIndex].Z;
    //        var h = values[1, 0].X - values[0, 0].X;
    //        var dlast = values[values.Rows - 1, columnIndex].Dx;
    //        var dfirst = values[0, columnIndex].Dx;

    //        SolveTridiagonal(rget, h, dfirst, dlast, equationsCount, dset);
    //    }

    //    private void SolveTridiagonal(Func<int, double> rightSideValuesToGet, double h, double dfirst, double dlast,
    //        int equationsCount, Action<int, double> unknownsToSet)
    //    {
    //        var result = RightSide(rightSideValuesToGet, h, dfirst, dlast, equationsCount);
    //        LinearSystemSolver.TridiagonalSystem(UpperDiagonal(equationsCount), MainDiagonal(equationsCount),
    //            LowerDiagonal(equationsCount), result);

    //        for (var i = 0; i < result.Length; i++)
    //        {
    //            unknownsToSet(i + 1, result[i]);
    //        }
    //    }

    //    private double[] MainDiagonal(int equationsCount,bool even)
    //    {
    //        var diag= MyArrays.InitalizedArray<double>(equationsCount, -14);
    //        diag[equationsCount - 1] = even ? -15 : -14;
    //        return diag;
    //    }

    //    private double[] LowerDiagonal(int equationsCount)
    //    {
    //        return MyArrays.InitalizedArray<double>(equationsCount, 1);
    //    }

    //    private double[] UpperDiagonal(int equationsCount)
    //    {
    //        return MyArrays.InitalizedArray<double>(equationsCount, 1);
    //    }

    //    private double[] RightSide(Func<int, double> rightSideValuesToGet, double h, double dfirst, double dlast, int equationsCount, bool even)
    //    {
            
    //        //var equationParams = new EquationParams(length);

    //        var rs = new double[equationsCount];
    //        var tau = even ? 0 : 2;
    //        var eta = even ? -4 : 1;
    //        var div3h = 3 / h;
    //        var div12h = div3h * 4;
    //        rs[0] = div3h * (rightSideValuesToGet(4) - rightSideValuesToGet(0)) - div12h * (rightSideValuesToGet(3) - rightSideValuesToGet(1)) - dfirst;
    //        rs[equationsCount - 1] = div3h *
    //                                 (rightSideValuesToGet(equationParams.Upsilon + equationParams.Tau) -
    //                                  rightSideValuesToGet(equationParams.Upsilon - 2))
    //                                 -
    //                                 div12h *
    //                                 (rightSideValuesToGet(equationParams.Upsilon + 1) - rightSideValuesToGet(equationParams.Upsilon - 1)) -
    //                                 dlast;
    //        for (var i = 1; i < equationsCount; i++)
    //        {
    //            var i2 = i * 2;
    //            rs[i] = div3h * (rightSideValuesToGet(i2 + 4) - rightSideValuesToGet(i2)) - div12h * (rightSideValuesToGet(i2 + 3) - rightSideValuesToGet(i2 + 1));
    //        }
    //        return rs;
    //    }

    //    public void FillYDerivations(KnotMatrix values)
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public void FillXYDerivations(KnotMatrix values)
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public void FillYXDerivations(KnotMatrix values)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}
}
