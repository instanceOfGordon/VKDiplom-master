using HermiteInterpolation.Functions;
using HermiteInterpolation.Shapes.HermiteSpline.Bicubic;
using HermiteInterpolation.Shapes.HermiteSpline.Biquartic;
using HermiteInterpolation.SplineKnots;

namespace HermiteInterpolation.Shapes.HermiteSpline
{
    public static class HermiteSurfaceFactory
    {

        public static HermiteSurface CreateBicubic(SurfaceDimension uDimension, SurfaceDimension vDimension, InterpolatedFunction interpolatedFunction)
        {
            return new BicubicMeshGenerator(uDimension, vDimension, KnotsGeneratorFactory.DefaultImplementation(interpolatedFunction), Derivation.Zero).CreateSurface();
        }

        public static HermiteSurface CreateBicubic(SurfaceDimension uDimension, SurfaceDimension vDimension, KnotsGenerator knotsGenerator)
        {
            return new BicubicMeshGenerator(uDimension, vDimension, knotsGenerator, Derivation.Zero).CreateSurface();
        }

        public static HermiteSurface CreateBicubic(SurfaceDimension uDimension, SurfaceDimension vDimension, InterpolatedFunction interpolatedFunction, Derivation derivation)
        {
            var gen = new BicubicMeshGenerator(uDimension, vDimension, KnotsGeneratorFactory.DefaultImplementation(interpolatedFunction), derivation);
            return gen.CreateSurface();
        }
        public static HermiteSurface CreateBicubic(SurfaceDimension uDimension, SurfaceDimension vDimension, KnotsGenerator knotsGenerator, Derivation derivation)
        {
            var gen = new BicubicMeshGenerator(uDimension, vDimension, knotsGenerator, derivation);
            return gen.CreateSurface();
        }

        public static HermiteSurface CreateBiquartic(SurfaceDimension uDimension, SurfaceDimension vDimension, InterpolatedFunction interpolatedFunction)
        {
            return new BiquarticMeshGenerator(uDimension, vDimension, interpolatedFunction, KnotsGeneratorFactory.DefaultImplementation(interpolatedFunction), Derivation.Zero).CreateSurface();
        }

        public static HermiteSurface CreateBiquartic(SurfaceDimension uDimension, SurfaceDimension vDimension, InterpolatedFunction interpolatedFunction, KnotsGenerator knotsGenerator)
        {
            return new BiquarticMeshGenerator(uDimension, vDimension, interpolatedFunction, knotsGenerator, Derivation.Zero).CreateSurface();
        }

        public static HermiteSurface CreateBiquartic(SurfaceDimension uDimension, SurfaceDimension vDimension, InterpolatedFunction interpolatedFunction, Derivation derivation)
        {
            var gen = new BiquarticMeshGenerator(uDimension, vDimension, interpolatedFunction, KnotsGeneratorFactory.DefaultImplementation(interpolatedFunction), derivation);
            return gen.CreateSurface();
        }

        public static HermiteSurface CreateBiquartic(SurfaceDimension uDimension, SurfaceDimension vDimension, InterpolatedFunction interpolatedFunction, KnotsGenerator knotsGenerator, Derivation derivation)
        {
            var gen = new BiquarticMeshGenerator(uDimension, vDimension, interpolatedFunction, knotsGenerator, derivation);
            return gen.CreateSurface();
        }
    }
}
