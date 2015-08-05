using HermiteInterpolation.Functions;

namespace HermiteInterpolation.SplineKnots
{
    public static class FunctionExtensions
    {
        public static DeBoorKnotsGenerator ToDeBoor(this DirectKnotsGenerator generator)
        {
            return new DeBoorKnotsGenerator(generator.Function);
        }

        public static DeBoorKnotsGenerator ToDeBoor(this ReducedDeBoorKnotsGenerator generator)
        {
            return new DeBoorKnotsGenerator(generator.Function);
        }

//        public static ReducedDeBoorKnotsGenerator ToReducedDeBoor(this DirectKnotsGenerator generator)
//        {
//            return new ReducedDeBoorKnotsGenerator(generator.Function);
//        }
//
//        public static ReducedDeBoorKnotsGenerator ToReducedDeBoor(this DeBoorKnotsGenerator generator)
//        {
//            return new ReducedDeBoorKnotsGenerator(generator.Function);
//        }
    }
}