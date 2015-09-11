using System.Collections.Generic;
using HermiteInterpolation.MathFunctions;
using HermiteInterpolation.SplineKnots;

namespace HermiteInterpolation.Shapes.SplineInterpolation
{
    public abstract class Spline : CompositeSurface
    {
       

        protected Spline(SurfaceDimension uDimension, SurfaceDimension vDimension,
            string functionExpression, string variableX, string variableY, Derivation derivation = Derivation.Zero)
            : this(uDimension, vDimension, new DirectKnotsGenerator(InterpolativeMathFunction.FromString(functionExpression,variableX,variableY)), derivation)
        {
            if (Name == null)
                Name = functionExpression;
        }


        protected Spline(SurfaceDimension uDimension, SurfaceDimension vDimension,
            InterpolativeMathFunction function, Derivation derivation = Derivation.Zero)
            : this(uDimension, vDimension, new DirectKnotsGenerator(function), derivation)
        {

        }

        //protected InterpolativeMathFunction InterpolativeFunction { get; }

        protected Spline(SurfaceDimension uDimension, SurfaceDimension vDimension,
            KnotsGenerator knotsGenerator, Derivation derivation = Derivation.Zero)
        {
            MeshDensity = 0.1f;
            //InterpolativeFunction = knotsGenerator.Function;
            //            _uKnotsDistance = Math.Abs(uMax - uMin) / uCount;
            //            
            //  _vKnotsDistance = Math.Abs(vMax - vMin) / vCount;
            //_interpolatedFunction = InterpolativeMathFunction;
            //Knots = knotsGenerator.GenerateKnots(uDimension, vDimension);

            Derivation = derivation;
            Segments = CreateMesh(knotsGenerator.GenerateKnots(uDimension, vDimension));
        }

       


        //        protected double UKnotsDistance { get { return _uKnotsDistance; } }
        //        protected double VKnotsDistance { get { return _vKnotsDistance; } }

        public Derivation Derivation { get; protected set; }


        //protected Knot[][] Knots { get; set; }


        internal IEnumerable<ISurface> CreateMesh(Knot[][] knots)
        {
            //var surface = Surface;
            var uCount_min_1 = knots.Length - 1;//surface.UKnotsCount-1;
            var vCount_min_1 = knots[0].Length - 1;//surface.VKnotsCount-1;

            var segments = new List<ISurface>(uCount_min_1 * vCount_min_1);

            for (int i = 0; i < uCount_min_1; i++)
            {
                for (int j = 0; j < vCount_min_1; j++)
                {
                    var segment = CreateSegment(i, j, knots);
                    segments.Add(segment);
                }
            }

            return segments;
        }

        public float MeshDensity { get; }

        //protected IEnumerable<ISurface> CreateMesh()
        //{
        //    return CreateSegments();

        //    //return new CompositeSurface(segments);
        //}

        protected abstract ISurface CreateSegment(int uIdx, int vIdx, Knot[][] knots);


      
    }
}
