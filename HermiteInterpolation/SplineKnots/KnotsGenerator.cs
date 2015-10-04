using System.Collections.Generic;
using System.Runtime.InteropServices;
using HermiteInterpolation.MathFunctions;
using HermiteInterpolation.Numerics;

using HermiteInterpolation.Shapes.SplineInterpolation;

namespace HermiteInterpolation.SplineKnots
{
    public abstract class KnotsGenerator
    {
        public InterpolativeMathFunction Function { get;}

        protected KnotsGenerator(InterpolativeMathFunction function)
        {
            Function = function;
        }

        protected KnotsGenerator()
        {
            
        }

        protected KnotsGenerator(MathExpression expression)
            :this(InterpolativeMathFunction.FromExpression(expression))
        {
           
        }
        public abstract KnotMatrix GenerateKnots(SurfaceDimension uDimension, SurfaceDimension vDimension);

        public static KnotsGenerator operator +(KnotsGenerator leftOp, KnotsGenerator rightOp)
        {
            var result = new ChainedKnotsGenerator(leftOp) {{rightOp, (l, r) => l + r}};
            return result;
        }

        public static KnotsGenerator operator -(KnotsGenerator leftOp, KnotsGenerator rightOp)
        {
            var result = new ChainedKnotsGenerator(leftOp) {{rightOp, (l, r) => l - r}};
            return result;
        }
    }
}