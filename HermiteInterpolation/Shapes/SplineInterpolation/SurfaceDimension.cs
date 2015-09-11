namespace HermiteInterpolation.Shapes.SplineInterpolation
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

        public static SurfaceDimension operator /(SurfaceDimension surfaceDimension, int countDivider)
        {
            var count = surfaceDimension.KnotCount%2 == 0
                ? surfaceDimension.KnotCount/2
                : surfaceDimension.KnotCount/2 + 1;
            return new SurfaceDimension(surfaceDimension.Min,surfaceDimension.Max,count);
        }

        public static SurfaceDimension operator *(SurfaceDimension surfaceDimension, int countMultiplier)
        {
            return new SurfaceDimension(surfaceDimension.Min, surfaceDimension.Max, surfaceDimension.KnotCount * countMultiplier);
        }
    }
}