using System;
using HermiteInterpolation.MathFunctions;
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
        public ReducedDeBoorKnotsGenerator(InterpolativeMathFunction function) : base(function)
        {
        }

        public ReducedDeBoorKnotsGenerator(MathExpression expression) : base(expression)
        {
        }


        protected override double[] MainDiagonal(int equationsCount, bool even = false)
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

        protected override void FillDyxDerivations(int rowOrColumnIdx, Knot[][] values)
        {
            throw new NotImplementedException();
        }

        protected override void FillYDerivations(int rowOrColumnIdx, Knot[][] values)
        {
            throw new NotImplementedException();
        }

        protected override void FillDxyDerivations(int rowOrColumnIdx, Knot[][] values)
        {
            throw new NotImplementedException();
        }

        protected override void FillXDerivations(int rowOrColumnIdx, Knot[][] values)
        {
            throw new NotImplementedException();
        }

        public override Knot[][] GenerateKnots(SurfaceDimension uDimension, SurfaceDimension vDimension)
        {
            return null;
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