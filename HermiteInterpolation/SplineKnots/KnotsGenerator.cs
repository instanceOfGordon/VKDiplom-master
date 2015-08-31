using HermiteInterpolation.Functions;
using HermiteInterpolation.Shapes.HermiteSpline;

namespace HermiteInterpolation.SplineKnots
{
    public abstract class KnotsGenerator
    {
        public InterpolatedFunction Function { get; }

        protected KnotsGenerator(InterpolatedFunction function)
        {
            Function = function;
        }
        public abstract Knot[][] GenerateKnots(SurfaceDimension uDimension, SurfaceDimension vDimension);
    }

    internal enum UnknownVariableType
    {
        Dx,
        Dxy,
        Dy,
        Dyx
    }
}