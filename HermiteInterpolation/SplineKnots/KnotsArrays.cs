

using System;

namespace HermiteInterpolation.SplineKnots
{
    public static class KnotsArrays
    {

        public static bool KnotsArraysIntersectionIndexes(Knot[] leftOp, Knot[] rightOp, out int leftOpIntersectIdx, out int rightOpIntersectIdx)
        {
            for (int i = 0; i < leftOp.Length; i++)
            {
                for (int j = 0; j < rightOp.Length; j++)
                {
                    if (!leftOp[i].EqualsPosition(rightOp[j])) continue;
                    leftOpIntersectIdx = i;
                    rightOpIntersectIdx = j;
                    return true;
                }
            }
            leftOpIntersectIdx = -1;
            rightOpIntersectIdx = -1;
            return false;
        }

        public static bool KnotsArraysIntersectionIndexes(Knot[][] leftOp, Knot[][] rightOp,
            out int leftOpIntersectIdx0, out int leftOpIntersectIdx1, out int rightOpIntersectIdx0, out int rightOpIntersectIdx1)
        {
            for (int i = 0; i < leftOp.Length; i++)
            {
                for (int j = 0; j < rightOp.Length; j++)
                {

                    if (!KnotsArraysIntersectionIndexes(leftOp[i],rightOp[j],out leftOpIntersectIdx1,out rightOpIntersectIdx1)) continue;
                    leftOpIntersectIdx0 = i;
                    rightOpIntersectIdx0 = j;
                    return true;
                }
            }
            leftOpIntersectIdx0 = -1;
            rightOpIntersectIdx0 = -1;
            leftOpIntersectIdx1 = -1;
            rightOpIntersectIdx1 = -1;
            return false;
        }
    }
}
