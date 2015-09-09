using HermiteInterpolation.Functions;
using HermiteInterpolation.SplineKnots;
using MathNet.Numerics.LinearAlgebra;

namespace HermiteInterpolation.Shapes.HermiteSpline.Bicubic
{
    internal class BicubicBasis : Basis
    {

        public BicubicBasis(Knot[][] knots, Derivation derivation) : base(knots,derivation)
        {
        }

        protected override Vector<double> FunctionVector(double t, double t0, double t1)
        {
            var m = Vector<double>.Build.Random(4, 0);
            var h = t1 - t0;
            var t_min_t0 = t - t0;
            var t_min_t1 = t - t1;
            var sqr_t_min_t0 = t_min_t0*t_min_t0;
            var sqr_t_min_t1 = t_min_t1*t_min_t1;
            var sqr_h = h*h;
            m[0] = (1 + 2*t_min_t0/h)*sqr_t_min_t1/sqr_h;
            m[1] = (1 - 2*t_min_t1/h)*sqr_t_min_t0/sqr_h;
            m[2] = t_min_t0*sqr_t_min_t1/sqr_h;
            m[3] = sqr_t_min_t0*t_min_t1/sqr_h;
            return m;
        }
        
        protected override Vector<double> FirstDerivationVector(double t, double t0, double t1)
        {
            var m = Vector<double>.Build.Random(4, 0);
            var h = t1 - t0;
            var t_min_t0 = t - t0;
            var t_min_t1 = t - t1;
            var sqr_h = h*h;
            var cube_h = sqr_h*h;
            m[0] = 2*t_min_t1*(3*t - t1 + h - 2*t0)/cube_h;
            m[1] = 2*t_min_t0*(-3*t + t0 + h + 2*t1)/cube_h;
            m[2] = t_min_t1*(3*t - t1 - 2*t0)/sqr_h;
            m[3] = t_min_t0*(3*t - 2*t1 - t0)/sqr_h;
            return m;
        }

        protected override Vector<double> SecondDerivationVector(double t, double t0, double t1)
        {
            var m = Vector<double>.Build.Random(4, 0);
            var h = t1 - t0;
            var sqr_h = h*h;
            var cube_h = sqr_h*h;
            m[0] = 2*(h + 2*(3*t - t0 - 2*t1))/cube_h;
            m[1] = 2*(h + 2*(-3*t + 2*t0 + t1))/cube_h;
            m[2] = 2*(3*t - t0 - 2*t1)/sqr_h;
            m[3] = 2*(3*t - 2*t0 - t1)/sqr_h;
            return m;
        }

        internal override Matrix<double> Matrix(int uIdx, int vIdx)
        {
            var m = Matrix<double>.Build.Random(4, 4, 0);

            var k00 = Knots[uIdx][vIdx];
            var k01 = Knots[uIdx][vIdx + 1];
            var k10 = Knots[uIdx + 1][vIdx];
            var k11 = Knots[uIdx + 1][vIdx + 1];
            m[0, 0] = k00.Z; //SafeCall(f, u0, v0);
            m[0, 1] = k01.Z; //SafeCall(f, u0, v1);
            m[0, 2] = k00.Dy; //SafeCall(dy, u0, v0);
            m[0, 3] = k01.Dy; //SafeCall(dy, u0, v1);
            m[1, 0] = k10.Z; //SafeCall(f, u1, v0);
            m[1, 1] = k11.Z; //SafeCall(f, u1, v1);
            m[1, 2] = k10.Dy; //SafeCall(dy, u1, v0);
            m[1, 3] = k11.Dy; //SafeCall(dy, u1, v1);

            m[2, 0] = k00.Dx; //SafeCall(dx, u0, v0);
            m[2, 1] = k01.Dx; //SafeCall(dx, u0, v1);
            m[2, 2] = k00.Dxy; //SafeCall(dxy, u0, v0);
            m[2, 3] = k01.Dxy; //SafeCall(dxy, u0, v1);
            m[3, 0] = k10.Dx; //SafeCall(dx, u1, v0);
            m[3, 1] = k11.Dx; //SafeCall(dx, u1, v1);
            m[3, 2] = k10.Dxy; //SafeCall(dxy, u1, v0);
            m[3, 3] = k11.Dxy; //SafeCall(dxy, u1, v1);

            return m;
        }
    }
}