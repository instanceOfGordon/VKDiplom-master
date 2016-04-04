using System.Collections.Generic;
using HermiteInterpolation.Numerics;
using HermiteInterpolation.Numerics.MathFunctions;
using HermiteInterpolation.SplineKnots;

namespace HermiteInterpolation.Shapes.SplineInterpolation
{
    public abstract class Spline : MathSurface
    {
        protected Spline(SurfaceDimension uDimension,
            SurfaceDimension vDimension,
            MathExpression mathExpression,
            Derivation derivation = Derivation.Zero)
            : this(
                uDimension, vDimension,
                InterpolativeMathFunction.FromExpression(mathExpression),
                derivation)
        {
            Name = mathExpression.Expression;
        }

        protected Spline(SurfaceDimension uDimension,
            SurfaceDimension vDimension,
            InterpolativeMathFunction interpolativeMathFunction,
            Derivation derivation = Derivation.Zero)
            : this(
                uDimension, vDimension,
                new DirectKnotsGenerator(interpolativeMathFunction), derivation)
        {
        }

        protected Spline(SurfaceDimension uDimension,
            SurfaceDimension vDimension,
            KnotsGenerator knotsGenerator,
            Derivation derivation = Derivation.Zero)
        {
            KnotsGenerator = knotsGenerator;
            UDimension = uDimension;
            VDimension = vDimension;
            Derivation = derivation;
            Segments =
                CreateMesh(knotsGenerator.GenerateKnots(uDimension, vDimension));
        }

        public KnotsGenerator KnotsGenerator { get; }

        internal IEnumerable<ISurface> CreateMesh(KnotMatrix knots)
        {
            var uCount_min_1 = knots.Rows - 1;
            var vCount_min_1 = knots.Columns - 1;

            var segments = new List<ISurface>(uCount_min_1*vCount_min_1);

            for (var i = 0; i < uCount_min_1; i++)
            {
                for (var j = 0; j < vCount_min_1; j++)
                {
                    var segment = CreateSegment(i, j, knots);
                    segments.Add(segment);
                }
            }

            return segments;
        }

        protected abstract ISurface CreateSegment(int uIdx, int vIdx,
            KnotMatrix knots);
    }
}