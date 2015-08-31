using System;
using HermiteInterpolation.SplineKnots;

namespace HermiteInterpolation.Shapes.HermiteSpline.Biquartic
{
    public class BiquarticMeshGenerator2 : HermiteMeshGenerator
    {
        public BiquarticMeshGenerator2(SurfaceDimension uDimension, SurfaceDimension vDimension, KnotsGenerator knotsGenerator, Derivation derivation) 
            : base(uDimension, vDimension, knotsGenerator, derivation)
        {
        }

        protected override ISurface CreateMeshSegment(int uIdx, int vIdx)
        {
            throw new NotImplementedException();
        }
    }
}