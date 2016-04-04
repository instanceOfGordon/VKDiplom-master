using HermiteInterpolation.Numerics;
using HermiteInterpolation.Numerics.MathFunctions;
using HermiteInterpolation.Shapes.SplineInterpolation;

namespace HermiteInterpolation.SplineKnots
{
    public abstract class KnotsGenerator
    {
        protected KnotsGenerator(InterpolativeMathFunction function)
        {
            Function = function;
        }

        protected KnotsGenerator()
        {
        }

        protected KnotsGenerator(MathExpression expression)
            : this(InterpolativeMathFunction.FromExpression(expression))
        {
        }

        public InterpolativeMathFunction Function { get; }

        public abstract KnotMatrix GenerateKnots(SurfaceDimension uDimension,
            SurfaceDimension vDimension);

        public static KnotsGenerator operator +(
            KnotsGenerator leftOp, KnotsGenerator rightOp)
        {
            return new ChainedKnotsGenerator(leftOp)
            {
                {rightOp, (l, r) => l + r}
            };
        }

        public static KnotsGenerator operator -(
            KnotsGenerator leftOp, KnotsGenerator rightOp)
        {
            return new ChainedKnotsGenerator(leftOp)
            {
                {rightOp, (l, r) => l - r}
            };
        }
    }
}