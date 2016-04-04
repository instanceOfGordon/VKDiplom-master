using HermiteInterpolation.Shapes.SplineInterpolation;

namespace HermiteInterpolation.Shapes
{
    public abstract class MathSurface : CompositeSurface
    {
        public SurfaceDimension UDimension { get; protected set; }
        public SurfaceDimension VDimension { get; protected set; }
        public Derivation Derivation { get; protected set; }
    }
}