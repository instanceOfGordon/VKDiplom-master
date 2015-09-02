using System;
using HermiteInterpolation.Functions;
using HermiteInterpolation.SplineKnots;
using MathNet.Numerics.LinearAlgebra;

namespace HermiteInterpolation.Shapes.HermiteSpline.Biquartic
{
    internal class BiquarticBasis : Basis
    {
        public BiquarticBasis(Knot[][] knots, Derivation derivation)
            : base(knots, derivation)
        {
        }

        protected override Vector<double> FunctionVector(double t, double t0, double h)
        {
            var m = Vector<double>.Build.Random(5, 0);
            var t1 = t0 + h;
            var t2 = t0 + 2*h;
            var t_min_t0 = t - t0;
            var t_min_t1 = t - t1;
            var t_min_t2 = t - t2;
            var sqr_t_min_t0 = Math.Pow(t_min_t0, 2);
            var sqr_t_min_t2 = Math.Pow(t_min_t2, 2);
            var four_mul_cube_h = 4*Math.Pow(h, 3);


            m[0] = (-(1 + 2*(t_min_t0/h))*t_min_t1*sqr_t_min_t2)/
                   four_mul_cube_h;
            m[1] = (sqr_t_min_t0*sqr_t_min_t2)/
                   Math.Pow(h, 4);
            m[2] = (-(1 + 2*(t_min_t0/h))*t_min_t1*Math.Pow(t_min_t2, 2))/
                   four_mul_cube_h;
            m[3] = (-t_min_t0*t_min_t1*sqr_t_min_t2)/
                   four_mul_cube_h;
            m[4] = (sqr_t_min_t0*t_min_t1*t_min_t2)/
                   four_mul_cube_h;

            return m;
        }

        protected override Vector<double> FirstDerivationVector(double t, double t0, double h)
        {
            var m = Vector<double>.Build.Random(5, 0);
            var t1 = t0 + h;
            var t2 = t0 + 2*h;
            var t_min_t0 = t - t0;
            var t_min_t1 = t - t1;
            var t_min_t2 = t - t2;
            var sqr_t_min_t0 = Math.Pow(t_min_t0, 2);

            var four_mul_cube_h = 4*Math.Pow(h, 3);
            var pow4_h = Math.Pow(h, 4);
            var pow4_h_mul_4 = 4*Math.Pow(h, 4);

            var sqr_h = h*h;

            m[0] = (sqr_h + 4*h*t_min_t0 - 3*sqr_t_min_t0)/
                   pow4_h_mul_4;
            m[1] = (4*t_min_t0*t_min_t2*t_min_t1)/
                   pow4_h;
            m[2] = (-t_min_t0*(2*(5*sqr_h + 4*sqr_t_min_t0) - 21*h*t_min_t0))/
                   pow4_h_mul_4;
            m[3] = (-t_min_t2*(2*(sqr_h + 2*sqr_t_min_t0) - 7*h*t_min_t0))/
                   four_mul_cube_h;
            m[4] = (-2*h*t_min_t0 + 3*sqr_t_min_t0 - 1)/
                   four_mul_cube_h;

            return m;
        }

        protected override Vector<double> SecondDerivationVector(double t, double t0, double h)
        {
            var m = Vector<double>.Build.Random(5, 0);
            var t_min_t0 = t - t0;
            var sqr_t_min_t0 = Math.Pow(t_min_t0, 2);

            var two_mul_cube_h = 2*Math.Pow(h, 3);
            var pow4_h = Math.Pow(h, 4);
            var pow4_h_mul_2 = 2*Math.Pow(h, 4);
            var sqr_h = h*h;
            var h_mul_2 = 2*h;

            m[0] = (h_mul_2 - 3*t_min_t0)/
                   pow4_h_mul_2;
            m[1] = (4*(h_mul_2*(h - 3*t_min_t0) + 3*sqr_t_min_t0))/
                   pow4_h;
            m[2] = (h*(21*t_min_t0 - 5*h) - 12*sqr_t_min_t0)/
                   pow4_h_mul_2;
            m[3] = (h*(15*t_min_t0 - 8*h) - 6*sqr_t_min_t0)/
                   two_mul_cube_h;
            m[4] = (h - 3*t_min_t0)/
                   two_mul_cube_h;

            return m;
        }

       
        internal override Matrix<double> Matrix(int uIdx, int vIdx)
        {
            var m = Matrix<double>.Build.Random(5, 5, 0);

            var k00 = Knots[uIdx][vIdx];
            var k01 = Knots[uIdx][vIdx + 1];
            var k02 = Knots[uIdx][vIdx + 2];
            var k10 = Knots[uIdx + 1][vIdx];
            var k11 = Knots[uIdx + 1][vIdx + 1];
            var k12 = Knots[uIdx + 1][vIdx + 2];
            var k20 = Knots[uIdx + 2][vIdx];
            var k21 = Knots[uIdx + 2][vIdx + 1];
            var k22 = Knots[uIdx + 2][vIdx + 2];

            m[0, 0] = k00.Z; //NaNSafeCall(f, u0, v0);
            m[0, 1] = k01.Z; //NaNSafeCall(f, u0, v1);
            m[0, 2] = k02.Z; //NaNSafeCall(f, u0, v2);
            m[0, 3] = k00.Dy; //NaNSafeCall(dy, u0, v0);
            m[0, 4] = k02.Dy; //NaNSafeCall(dy, u0, v2);
            m[1, 0] = k10.Z; //NaNSafeCall(f, u1, v0);
            m[1, 1] = k11.Z; //NaNSafeCall(f, u1, v1);
            m[1, 2] = k12.Z; //NaNSafeCall(f, u1, v2);
            m[1, 3] = k10.Dy; //NaNSafeCall(dy, u1, v0);
            m[1, 4] = k12.Dy; //NaNSafeCall(dy, u1, v2);
            m[2, 0] = k20.Z; //NaNSafeCall(f, u2, v0);
            m[2, 1] = k21.Z; //NaNSafeCall(f, u2, v1);
            m[2, 2] = k22.Z; //NaNSafeCall(f, u2, v2);
            m[2, 3] = k20.Dy; //NaNSafeCall(dy, u2, v0);
            m[2, 4] = k22.Dy; //NaNSafeCall(dy, u2, v2);

            m[2, 0] = k00.Dx; //NaNSafeCall(dx, u0, v0);
            m[2, 1] = k01.Dx; //NaNSafeCall(dx, u0, v1); 
            m[2, 2] = k02.Dx; //NaNSafeCall(dx, u0, v2);
            m[2, 3] = k00.Dxy; //NaNSafeCall(dxy, u0, v0);
            m[2, 4] = k02.Dxy; //NaNSafeCall(dxy, u0, v2);
            m[3, 0] = k20.Dx; //NaNSafeCall(dx, u2, v0);
            m[3, 1] = k21.Dx; //NaNSafeCall(dx, u2, v1);
            m[3, 2] = k22.Dx; //NaNSafeCall(dx, u2, v2);
            m[3, 3] = k20.Dxy; //NaNSafeCall(dxy, u2, v0);
            m[3, 4] = k22.Dxy; //NaNSafeCall(dxy, u2, v2);

            return m;
        }
    }
}