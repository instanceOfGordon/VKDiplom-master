using HermiteInterpolation.Functions;
using HermiteInterpolation.Shapes.HermiteSpline.Bicubic;
using HermiteInterpolation.SplineKnots;

namespace HermiteInterpolation.Shapes.HermiteSpline.Biquartic
{
    internal class ClampedBicubicBasisNOTUSE : BicubicBasis
    {
        

//        internal override BasisVector<double> FunctionVector(double t, double t0, double h)
//        {
//            var m = BasisVector<double>.Build.Random(4, 0);
//            var t1 = t + h;
//            var t_min_t0 = t - t0;
//            var t_min_t1 = t - t1;
//
//            var sqr_t_min_t0 = Math.Pow(t_min_t0, 2);
//            var sqr_t_min_t1 = Math.Pow(t_min_t1, 2);
//            var sqr_h = Math.Pow(h, 2);
//
//
//            m[0] = ((1 + 2*(t_min_t0/h))*sqr_t_min_t1)/
//                   sqr_h;
//            m[1] = (sqr_t_min_t0*(1 - 2*(t_min_t1/h)))/
//                   sqr_h;
//            m[2] = (t_min_t0*sqr_t_min_t1)/
//                   sqr_h;
//            m[3] = (sqr_t_min_t0*t_min_t1)/
//                   sqr_h;
//
//            return m;
//        }
//
//        internal override BasisVector<double> FirstDerivationVector(double t, double t0, double h)
//        {
//            var m = BasisVector<double>.Build.Random(4, 0);
//
//            var t1 = t + h;
//            var t_min_t0 = t - t0;
//            var t_min_t1 = t - t1;
//            var cube_h = Math.Pow(h, 3);
//
//
//            m[0] = (6*t_min_t1*t_min_t0)/
//                   cube_h;
//            m[1] = (-t_min_t1*t_min_t0)/
//                   cube_h;
//            m[2] = (t_min_t1*(3*t_min_t0 - h))/
//                   cube_h;
//            m[3] = (t_min_t0*(2*h - 3*(t_min_t0)))/
//                   cube_h;
//            return m;
//        }
//
//        internal override BasisVector<double> SecondDerivationVector(double t, double t0, double h)
//        {
//            var m = BasisVector<double>.Build.Random(4, 0);
//            var t1 = t + h;
//            var t_min_t0 = t - t0;
//            var t_min_t1 = t - t1;
//            var cube_h = Math.Pow(h, 3);
//
//
//            m[0] = (6*(2*t_min_t1 - h))/
//                   cube_h;
//            m[1] = (3*(h - 2*t_min_t0))/
//                   cube_h;
//            m[2] = (2*(3*t_min_t0 - 2*h))/
//                   cube_h;
//            m[3] = (2*(3*t_min_t0 - h))/
//                   cube_h;
//
//            return m;
//        }
        public ClampedBicubicBasisNOTUSE(Knot[][] knots, Derivation derivation) : base(knots,derivation)
        {
        }
    }
}