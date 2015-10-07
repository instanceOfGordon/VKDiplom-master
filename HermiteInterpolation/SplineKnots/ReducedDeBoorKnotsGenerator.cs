using System;
using HermiteInterpolation.MathFunctions;
using HermiteInterpolation.Numerics;

using HermiteInterpolation.Shapes.SplineInterpolation;
using HermiteInterpolation.Utils;

namespace HermiteInterpolation.SplineKnots
{
    public class ReducedDeBoorKnotsGenerator : DeBoorKnotsGenerator
    {
        //        private readonly int _uCount;
        //        private readonly int _vCount;
        //
        //        public ReducedDeBoorKnotsGenerator(InterpolativeMathFunction function, int uCount, int vCount) : base(function)
        //        {
        //            _uCount = uCount;
        //            _vCount = vCount;
        //        }

        public override KnotMatrix GenerateKnots(SurfaceDimension uDimension, SurfaceDimension vDimension)
        {
            if (uDimension.KnotCount < 4 || vDimension.KnotCount < 4)
            {
                return new DirectKnotsGenerator(Function).GenerateKnots(uDimension, vDimension);
            }

            var values = new KnotMatrix(uDimension.KnotCount, vDimension.KnotCount);
            InitializeKnots(uDimension, vDimension, values);

            FillXDerivations(values);
            FillYDerivations(values);
            FillDxyDerivations(values);      
            FillDyxDerivations(values);

            return values;
        }

        public ReducedDeBoorKnotsGenerator(InterpolativeMathFunction function) : base(function)
        {
        }

        public ReducedDeBoorKnotsGenerator(MathExpression expression) : base(expression)
        {
        }

        protected override double[] MainDiagonal(int equationsCount)
        {
            return MainDiagonal(equationsCount,false);
        }

        protected double[] MainDiagonal(int equationsCount, bool even)
        {
            var diag = MyArrays.InitalizedArray<double>(equationsCount, -14);
            if (even)
                diag[equationsCount - 1] = -15;
            return diag;
        }

        protected override double[] RightSide(Func<int, double> rightSide, double h, double dfirst, double dlast,
            int equationsCount, bool even = false)
        {
            var length = even ? 2*equationsCount : 2*equationsCount + 1;
            var equationParams = new EquationParams(length);

            var rs = new double[equationsCount];
            var div3h = 3/h;
            var div12h = div3h*4;
            rs[0] = div3h*(rightSide(4) - rightSide(0)) - div12h*(rightSide(3) - rightSide(1)) - dfirst;
            rs[equationsCount - 1] = div3h*
                                     (rightSide(equationParams.Upsilon + equationParams.Tau) -
                                      rightSide(equationParams.Upsilon - 2))
                                     -
                                     div12h*
                                     (rightSide(equationParams.Upsilon + 1) - rightSide(equationParams.Upsilon - 1)) -
                                     dlast;
            for (var i = 1; i < equationsCount; i++)
            {
                var i2 = i*2;
                rs[i] = div3h*(rightSide(i2 + 4) - rightSide(i2)) - div12h*(rightSide(i2 + 3) - rightSide(i2 + 1));
            }
            return rs;
        }

        protected override void FillXDerivations(KnotMatrix values)
        {
            for (var j = 0; j < values.Columns; j++)
            {
                FillXDerivations(j, values);
            }
            var h = values[1, 0].X - values[0, 0].X;
            for (int i = 1; i < values.Rows-1; i++)
            {
                
                for (int j = 1; j < values.Columns-1; j++)
                {
                    values[i,j].Dx = 0.75*h*(values[i+1,j].Z-values[i-1,j].Z)
                        -0.25*(values[i+1, j].Dx+ values[i-1, j].Dx);
                }
            }
        }

        protected override void FillYDerivations(KnotMatrix values)
        {
            for (var i = 0; i < values.Rows; i++)
            {
                FillYDerivations(i, values);
            }
            var h = values[0, 1].X - values[0, 0].X;
            for (int j = 1; j < values.Columns - 1; j++)              
            {
                for (int i = 1; i < values.Rows - 1; i++)
                {
                    values[i, j].Dx = 0.75 * h * (values[i, j+1].Z - values[i, j-1].Z)
                        - 0.25 * (values[i, j+1].Dy + values[i, j+1].Dy);
                }
            }
        }

        protected override void FillDyxDerivations(KnotMatrix values)
        {
            for (var i = 2; i < values.Rows; i+=2)
            {
                FillDyxDerivations(i, values);
            }
        }


        protected override void FillDxyDerivations(KnotMatrix values)
        {
            base.FillDxyDerivations(0, values);
            base.FillDxyDerivations(values.Columns - 1, values);
            base.FillDyxDerivations(0, values);
            base.FillDyxDerivations(values.Rows - 1, values);
        }

        protected override void FillDyxDerivations(int rowIndex, KnotMatrix values)
        {
            throw new NotImplementedException();
        }

        protected override void FillYDerivations(int rowIndex, KnotMatrix values)
        {
            var equationsCount = values.Columns/2 - 2;
            if (equationsCount == 0) return;
            Action<int, double> dset = (idx, value) => values[rowIndex, 2*idx].Dy = value;
            Func<int, double> rget = idx => values[rowIndex, 2*idx].Z;
            var h = values[0, 1].Y - values[0, 0].Y;
            var dfirst = values[rowIndex, 0].Dy;
            var dlast = values[rowIndex, values.Columns - 1].Dy;

            SolveTridiagonal(rget, h, dfirst, dlast, equationsCount, dset);
        }

        //protected override void FillDxyDerivations(int rowIndex, KnotMatrix values)
        //{
        //    var equationsCount = values.Rows - 2;
        //    if (equationsCount == 0) return;
        //    Action<int, double> dset = (idx, value) => values[idx, rowIndex].Dxy = value;
        //    Func<int, double> rget = idx => values[idx, rowIndex].Dy;
        //    var h = values[1, 0].X - values[0, 0].X;
        //    var dlast = values[values.Rows - 1, rowIndex].Dxy;
        //    var dfirst = values[0, rowIndex].Dxy;

        //    SolveTridiagonal(rget, h, dfirst, dlast, equationsCount, dset);
        //}

        protected override void FillXDerivations(int columnIndex, KnotMatrix values)
        {
            var equationsCount = values.Rows/2 - 2;
            if (equationsCount == 0) return;
            Action<int, double> dset = (idx, value) => values[2*idx, columnIndex].Dx = value;
            Func<int, double> rget = idx => values[2*idx, columnIndex].Z;
            var h = values[1, 0].X - values[0, 0].X;
            var dlast = values[values.Rows - 1, columnIndex].Dx;
            var dfirst = values[0, columnIndex].Dx;

            SolveTridiagonal(rget, h, dfirst, dlast, equationsCount, dset);
        }

        

        protected struct EquationParams
        {
            /// <summary>
            /// </summary>
            /// <param name="mu">Matrix parameter</param>
            /// <param name="tau">Right side index parameter</param>
            /// <param name="eta">Right side parameter</param>
            /// <param name="upsilon">Number of equations</param>
            public EquationParams(int mu, int tau, int eta, int upsilon) : this()
            {
                Mu = mu;
                Tau = tau;
                Eta = eta;
                Upsilon = upsilon;
            }

            public EquationParams(int length) : this()
            {
                if (length%2 == 0)
                {
                    Mu = 15;
                    Tau = 0;
                    Eta = -4;
                    Upsilon = length;
                }
                else
                {
                    Mu = -14;
                    Tau = 2;
                    Eta = 1;
                    Upsilon = length - 1;
                }
            }

            public int Mu { get; private set; }
            public int Tau { get; private set; }
            public int Eta { get; private set; }
            public int Upsilon { get; private set; }
        }

        
    }
}