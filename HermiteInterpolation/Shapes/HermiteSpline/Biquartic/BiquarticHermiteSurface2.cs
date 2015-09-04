using System;
using HermiteInterpolation.SplineKnots;

namespace HermiteInterpolation.Shapes.HermiteSpline.Biquartic
{
    public class BiquarticHermiteSurface2 : Spline
    {
        public BiquarticHermiteSurface2(SurfaceDimension uDimension, SurfaceDimension vDimension, KnotsGenerator knotsGenerator, Derivation derivation) 
            : base(uDimension, vDimension, knotsGenerator, derivation)
        {
        }

        protected override ISurface CreateSegment(int uIdx, int vIdx)
        {
            throw new NotImplementedException();
        }
    }
}