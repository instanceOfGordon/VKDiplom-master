using System;
using HermiteInterpolation.SplineKnots;

namespace HermiteInterpolation.Functions
{
    public class ReducedDeBoorDirectKnotsGenerator : DeBoorDirectKnotsGenerator
    {
        public ReducedDeBoorDirectKnotsGenerator(InterpolatedFunction function)
            : base(function)
        {
        }
        public override Knot[][] ComputeKnots(double uMin, double uMax, int uCount, double vMin, double vMax, int vCount)
        {
            return base.ComputeKnots(uMin, uMax, uCount, vMin, vMax, vCount);
        }

        
    }
}
