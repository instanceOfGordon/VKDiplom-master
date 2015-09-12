using HermiteInterpolation.MathFunctions;
using HermiteInterpolation.SplineKnots;

namespace HermiteInterpolation.Shapes.SplineInterpolation
{
    public delegate CompositeSurface SplineFactory(SurfaceDimension uDimension, SurfaceDimension vDimension, KnotsGenerator knotsGenerator, Derivation derivation = Derivation.Zero);

    //public delegate CompositeSurface SplineFactory(SurfaceDimension uDimension, SurfaceDimension vDimension, string mathExpression, string variableX, string variableY, Derivation derivation = Derivation.Zero);

}
