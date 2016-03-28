using HermiteInterpolation.SplineKnots;

namespace HermiteInterpolation.Shapes.SplineInterpolation
{
    public delegate MathSurface SplineFactory(SurfaceDimension uDimension, SurfaceDimension vDimension, KnotsGenerator knotsGenerator, Derivation derivation = Derivation.Zero);
}
