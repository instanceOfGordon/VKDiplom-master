using System;
using HermiteInterpolation.Utils;

namespace HermiteInterpolation.SplineKnots
{
    public delegate Knot KnotOperation(Knot left, Knot right);

    public class KnotMatrix
    {
        private readonly Knot[][] _matrix;

        public KnotMatrix(int rows, int columns)
        {
            _matrix = MyArrays.JaggedArray<Knot>(rows, columns);
        }

        public int Rows => _matrix.Length;
        public int Columns => _matrix[0].Length;

        public Knot this[int i, int j]
        {
            get { return _matrix[i][j]; }
            set
            {
                //Knot ll, lr, tl, tr;
                //if(i== Rows - 1)


                //var ll = _matrix[i - 1][j-1];
                //var lr = _matrix[i - 1][j + 1];
                //var tl = _matrix[i+1]

                _matrix[i][j] = value;
            }
        }

        public void ForEach(Action<Knot> operation)
        {
            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Columns; j++)
                {
                    operation(_matrix[i][j]);
                }
            }
        }

        public static KnotMatrix Operation(KnotMatrix left, KnotMatrix right, KnotOperation operation)
        {
            var result = new KnotMatrix(left.Rows, left.Columns);
            for (var i = 0; i < result.Rows; i++)
            {
                
                for (var j = 0; j < result.Columns; j++)
                {
                    result[i, j] = j < right.Columns && i < right.Rows ? operation(left[i, j], right[i, j]) : left[i, j];
                }
              
            }
            return result;
        }

        public static KnotMatrix operator -(KnotMatrix left, KnotMatrix right)
        {
            return Operation(left, right, (l, r) => l - r);
        }
        public static KnotMatrix operator +(KnotMatrix left, KnotMatrix right)
        {
            return Operation(left, right, (l, r) => l + r);
        }

        //private bool KnotsArraysIntersectionIndexes(Knot[] leftOp, Knot[] rightOp, out int leftOpIntersectIdx, out int rightOpIntersectIdx)
        //{
        //    for (int i = 0; i < leftOp.Length; i++)
        //    {
        //        for (int j = 0; j < rightOp.Length; j++)
        //        {
        //            if (!leftOp[i].EqualsPosition(rightOp[j])) continue;
        //            leftOpIntersectIdx = i;
        //            rightOpIntersectIdx = j;
        //            return true;
        //        }
        //    }
        //    leftOpIntersectIdx = -1;
        //    rightOpIntersectIdx = -1;
        //    return false;
        //}

        //public static bool IntersectionIndexes(KnotMatrix leftOp, KnotMatrix rightOp,
        //    out int leftOpIntersectIdx0, out int leftOpIntersectIdx1, out int rightOpIntersectIdx0, out int rightOpIntersectIdx1)
        //{
        //    for (int i = 0; i < leftOp.Rows; i++)
        //    {
        //        for (int j = 0; j < rightOp.Rows; j++)
        //        {

        //            if (!KnotsArraysIntersectionIndexes(leftOp[i], rightOp[j], out leftOpIntersectIdx1, out rightOpIntersectIdx1)) continue;
        //            leftOpIntersectIdx0 = i;
        //            rightOpIntersectIdx0 = j;
        //            return true;
        //        }
        //    }
        //    leftOpIntersectIdx0 = -1;
        //    rightOpIntersectIdx0 = -1;
        //    leftOpIntersectIdx1 = -1;
        //    rightOpIntersectIdx1 = -1;
        //    return false;
        //}

    }
}