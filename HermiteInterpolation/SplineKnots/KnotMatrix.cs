using System.Collections;
using System.Collections.Generic;
using HermiteInterpolation.Utils;

namespace HermiteInterpolation.SplineKnots
{
    public delegate Knot KnotOperation(Knot left, Knot right);

    public sealed class KnotMatrix : IEnumerable<Knot>
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
            set { _matrix[i][j] = value; }
        }

        public IEnumerator<Knot> GetEnumerator()
        {
            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Columns; j++)
                {
                    yield return _matrix[i][j];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static KnotMatrix Operation(KnotMatrix left, KnotMatrix right,
            KnotOperation operation)
        {
            var result = new KnotMatrix(left.Rows, left.Columns);
            for (var i = 0; i < result.Rows; i++)
            {
                for (var j = 0; j < result.Columns; j++)
                {
                    result[i, j] = j < right.Columns && i < right.Rows
                        ? operation(left[i, j], right[i, j])
                        : left[i, j];
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
    }
}