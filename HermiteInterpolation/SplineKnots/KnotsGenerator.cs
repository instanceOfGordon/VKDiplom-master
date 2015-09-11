using HermiteInterpolation.MathFunctions;
using HermiteInterpolation.Shapes.SplineInterpolation;

namespace HermiteInterpolation.SplineKnots
{
    public abstract class KnotsGenerator
    {
        public InterpolativeMathFunction Function { get; }

        protected KnotsGenerator(InterpolativeMathFunction function)
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