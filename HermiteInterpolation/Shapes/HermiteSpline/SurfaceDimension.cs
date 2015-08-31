namespace HermiteInterpolation.Shapes.HermiteSpline
{
    public struct SurfaceDimension
    {
        public double Min { get; }
        public double Max { get; }

        public int KnotCount { get; }

        public SurfaceDimension(double min, double max, int knotCount)
        {
            Min = min;
            Max = max;
            KnotCount = knotCount;
        }
    }
}