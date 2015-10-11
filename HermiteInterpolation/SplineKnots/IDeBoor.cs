namespace HermiteInterpolation.SplineKnots
{
    public interface IDeBoor
    {
        void FillXDerivations(KnotMatrix values);
        void FillYDerivations(KnotMatrix values);
        void FillXYDerivations(KnotMatrix values);
        void FillYXDerivations(KnotMatrix values);
    }
}