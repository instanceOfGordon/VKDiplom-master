namespace HermiteInterpolation.SplineKnots
{
    public interface IKnotsGenerator
    {
        Knot[][] ComputeKnots(double uMin, double uMax, int uCount, double vMin,
            double vMax, int vCount);
    }

    internal enum UnknownVariableType
    {
        Dx,
        Dxy,
        Dy,
        Dyx
    }
}