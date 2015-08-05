using HermiteInterpolation.Functions;
using HermiteInterpolation.Shapes.HermiteSpline.Bicubic;
using HermiteInterpolation.Shapes.HermiteSpline.Biquartic;
using HermiteInterpolation.SplineKnots;

namespace HermiteInterpolation.Shapes.HermiteSpline
{
    public static class HermiteSurfaceFactory
    {

        public static HermiteSurface CreateBicubic(double uMin, double uMax, int uCount, double vMin, double vMax, int vCount, InterpolatedFunction interpolatedFunction)
        {
            return new BicubicMeshGenerator(uMin, uMax, uCount, vMin, vMax, vCount, KnotsGeneratorFactory.DefaultImplementation(interpolatedFunction), Derivation.Zero).CreateSurface();
        }

        public static HermiteSurface CreateBicubic(double uMin, double uMax, int uCount, double vMin, double vMax, int vCount, IKnotsGenerator knotsGenerator)
        {
            return new BicubicMeshGenerator(uMin, uMax, uCount, vMin, vMax, vCount, knotsGenerator, Derivation.Zero).CreateSurface();
        }

        public static HermiteSurface CreateBicubic(double uMin, double uMax, int uCount, double vMin, double vMax, int vCount, InterpolatedFunction interpolatedFunction, Derivation derivation)
        {
            var gen = new BicubicMeshGenerator(uMin, uMax, uCount, vMin, vMax, vCount, KnotsGeneratorFactory.DefaultImplementation(interpolatedFunction), derivation);
            return gen.CreateSurface();
        }
        public static HermiteSurface CreateBicubic(double uMin, double uMax, int uCount, double vMin, double vMax, int vCount, IKnotsGenerator knotsGenerator, Derivation derivation)
        {
            var gen = new BicubicMeshGenerator(uMin, uMax, uCount, vMin, vMax, vCount, knotsGenerator, derivation);
            return gen.CreateSurface();
        }

        public static HermiteSurface CreateBiquartic(double uMin, double uMax, int uCount, double vMin, double vMax, int vCount, InterpolatedFunction interpolatedFunction)
        {
            return new BiquarticMeshGenerator(uMin, uMax, uCount, vMin, vMax, vCount, interpolatedFunction, KnotsGeneratorFactory.DefaultImplementation(interpolatedFunction), Derivation.Zero).CreateSurface();
        }

        public static HermiteSurface CreateBiquartic(double uMin, double uMax, int uCount, double vMin, double vMax, int vCount, InterpolatedFunction interpolatedFunction, IKnotsGenerator knotsGenerator)
        {
            return new BiquarticMeshGenerator(uMin, uMax, uCount, vMin, vMax, vCount, interpolatedFunction, knotsGenerator, Derivation.Zero).CreateSurface();
        }

        public static HermiteSurface CreateBiquartic(double uMin, double uMax, int uCount, double vMin, double vMax, int vCount, InterpolatedFunction interpolatedFunction, Derivation derivation)
        {
            var gen = new BiquarticMeshGenerator(uMin, uMax, uCount, vMin, vMax, vCount, interpolatedFunction, KnotsGeneratorFactory.DefaultImplementation(interpolatedFunction), derivation);
            return gen.CreateSurface();
        }

        public static HermiteSurface CreateBiquartic(double uMin, double uMax, int uCount, double vMin, double vMax, int vCount, InterpolatedFunction interpolatedFunction, IKnotsGenerator knotsGenerator, Derivation derivation)
        {
            var gen = new BiquarticMeshGenerator(uMin, uMax, uCount, vMin, vMax, vCount, interpolatedFunction, knotsGenerator, derivation);
            return gen.CreateSurface();
        }
    }
}
