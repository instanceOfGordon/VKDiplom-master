using HermiteInterpolation.SplineKnots;

namespace HermiteInterpolation.Shapes.SplineInterpolation
{
    public delegate CompositeSurface SplineFactory(SurfaceDimension uDimension, SurfaceDimension vDimension, KnotsGenerator knotsGenerator, Derivation derivation = Derivation.Zero);
}
