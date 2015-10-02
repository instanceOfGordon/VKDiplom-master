using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using HermiteInterpolation.MathFunctions;
using HermiteInterpolation.Numerics;

using HermiteInterpolation.Shapes.SplineInterpolation.Bicubic;
using HermiteInterpolation.SplineKnots;
using HermiteInterpolation.Utils;

namespace HermiteInterpolation.Shapes.SplineInterpolation
{
    public abstract class Spline : CompositeSurface
    {
       
        //public string Name => MathExpression.Expression;

        //public MathExpression MathExpression { get; }

        //public KnotsGenerator KnotsGenerator { get; }

        public SurfaceDimension UDimension { get;  }
        public SurfaceDimension VDimension { get; }
        public Derivation Derivation { get; }


        // InterpolativeMathFunction InterpolatedMathFunction => KnotsGenerator.Function;


        //private Spline(SurfaceDimension uDimension, SurfaceDimension vDimension,
        //    InterpolativeMathFunction function, Derivation derivation = Derivation.Zero)
        //    : this(uDimension, vDimension, new DirectKnotsGenerator(function), derivation)
        //{

        //}

        //protected InterpolativeMathFunction InterpolativeFunction { get; }

        protected Spline(SurfaceDimension uDimension, SurfaceDimension vDimension,
           MathExpression mathExpression, Derivation derivation = Derivation.Zero)
            :this(uDimension,vDimension,InterpolativeMathFunction.FromExpression(mathExpression),derivation)
        {
            Name = mathExpression.Expression;
        }

        protected Spline(SurfaceDimension uDimension, SurfaceDimension vDimension,
           InterpolativeMathFunction interpolativeMathFunction, Derivation derivation = Derivation.Zero)
            : this(uDimension, vDimension,new DirectKnotsGenerator(interpolativeMathFunction),derivation)
        {

        }

        protected Spline(SurfaceDimension uDimension, SurfaceDimension vDimension,
           KnotsGenerator knotsGenerator,Derivation derivation = Derivation.Zero )
        {
            
            //MathExpression = expression;
            //KnotsGenerator = knotsGenerator.;
            //MeshDensity = 0.1f;
            //InterpolativeFunction = knotsGenerator.Function;
            //            _uKnotsDistance = Math.Abs(uMax - uMin) / uCount;
            //            
            //  _vKnotsDistance = Math.Abs(vMax - vMin) / vCount;
            //_interpolatedFunction = InterpolativeMathFunction;
            //Knots = knotsGenerator.GenerateKnots(uDimension, vDimension);

            UDimension = uDimension;
            VDimension = vDimension;
            Derivation = derivation;
            Knots = knotsGenerator.GenerateKnots(uDimension, vDimension);
            Segments = CreateMesh();
        }

        public Knot[][] Knots { get;}

        protected Spline(SurfaceDimension uDimension, SurfaceDimension vDimension, Knot[][] knots, Derivation derivation = Derivation.Zero)
        {
            //KnotsGenerator = knotsGenerator;
            //MeshDensity = 0.1f;
            //InterpolativeFunction = knotsGenerator.Function;
            //            _uKnotsDistance = Math.Abs(uMax - uMin) / uCount;
            //            
            //  _vKnotsDistance = Math.Abs(vMax - vMin) / vCount;
            //_interpolatedFunction = InterpolativeMathFunction;
            //Knots = knotsGenerator.GenerateKnots(uDimension, vDimension);
            Knots = knots;
            UDimension = uDimension;
            VDimension = vDimension;
            Derivation = derivation;
            Segments = CreateMesh();
        }



        //        protected double UKnotsDistance { get { return _uKnotsDistance; } }
        //        protected double VKnotsDistance { get { return _vKnotsDistance; } }



        //protected Knot[][] Knots { get; set; }
        internal IEnumerable<ISurface> CreateMesh()
        {
            //MyArrays.WriteArray(knots);
            //var surface = Surface;
            var uCount_min_1 = Knots.Length - 1;//surface.UKnotsCount-1;
            var vCount_min_1 = Knots[0].Length - 1;//surface.VKnotsCount-1;

            var segments = new List<ISurface>(uCount_min_1 * vCount_min_1);

            for (int i = 0; i < uCount_min_1; i++)
            {
                for (int j = 0; j < vCount_min_1; j++)
                {
                    var segment = CreateSegment(i, j);
                    segments.Add(segment);
                }
            }

            return segments;
        }

        //internal IEnumerable<ISurface> CreateMesh(Knot[][] knots)
        //{
        //    //MyArrays.WriteArray(knots);
        //    //var surface = Surface;
        //    var uCount_min_1 = knots.Length - 1;//surface.UKnotsCount-1;
        //    var vCount_min_1 = knots[0].Length - 1;//surface.VKnotsCount-1;

        //    var segments = new List<ISurface>(uCount_min_1 * vCount_min_1);

        //    for (int i = 0; i < uCount_min_1; i++)
        //    {
        //        for (int j = 0; j < vCount_min_1; j++)
        //        {
        //            var segment = CreateSegment(i, j, knots);
        //            segments.Add(segment);
        //        }
        //    }

        //    return segments;
        //}

        // public float MeshDensity { get; }

        //protected IEnumerable<ISurface> CreateMesh()
        //{
        //    return CreateSegments();

        //    //return new CompositeSurface(segments);
        //}

        protected abstract ISurface CreateSegment(int uIdx, int vIdx);

        //public static abstract Spline operator -(Spline s1, Spline s2)
        //public abstract Spline Subtract(Spline s1, Spline s2);
        public static Spline operator -(Spline s1, Spline s2)
        {
            //var subtractedFunction = s1.KnotsGenerator.Function - s2.KnotsGenerator.Function;
            //var generator = (KnotsGenerator)Activator.CreateInstance(s1.KnotsGenerator.GetType(), subtractedFunction);
            //return (Spline)Activator.CreateInstance(s1.GetType(), s1.UDimension, s1.VDimension, generator, s1.Derivation);

            var knots1 = s1.Knots;
            var knots2 = s2.Knots;
            var knots = SuptractKnots(knots1, knots2);
            var resultSpline = (Spline)Activator.CreateInstance(s1.GetType(), s1.UDimension, s1.VDimension, knots, s1.Derivation);
            return resultSpline;
        }

        public static Spline operator -(InterpolativeMathFunction f, Spline s)
        {
            //var subtractedFunction = f - s.KnotsGenerator.Function;
            //var generator = (KnotsGenerator)Activator.CreateInstance(s.KnotsGenerator.GetType(), subtractedFunction);
            //return (Spline)Activator.CreateInstance(s.GetType(), s.UDimension, s.VDimension, generator, s.Derivation);
            var knots1 = new DirectKnotsGenerator(f).GenerateKnots(s.UDimension,s.VDimension);
            var knots2 = s.Knots;
            var knots = SuptractKnots(knots1, knots2);
            var resultSpline = (Spline)Activator.CreateInstance(s.GetType(), s.UDimension, s.VDimension, knots, s.Derivation);
            return resultSpline;
        }

        private static Knot[][] SuptractKnots(Knot[][] knots1, Knot[][] knots2)
        {
            int i1, j1, i2, j2;
            KnotsArrays.KnotsArraysIntersectionIndexes(knots1, knots2, out i1, out j1, out i2, out j2);
            return MyArrays.ArraysOperation(knots1, knots2, (k1, k2) => k1 - k2, i1, j1);
           
        }

        public static Spline operator -(Spline s, InterpolativeMathFunction f)
        {
            //var subtractedFunction = s.KnotsGenerator.Function - f;
            //var generator = (KnotsGenerator)Activator.CreateInstance(s.KnotsGenerator.GetType(), subtractedFunction);
            //return (Spline)Activator.CreateInstance(s.GetType(), s.UDimension, s.VDimension, generator, s.Derivation);
            var knots2 = new DirectKnotsGenerator(f).GenerateKnots(s.UDimension, s.VDimension);
            var knots1 = s.Knots;
            var knots = SuptractKnots(knots1, knots2);
            var resultSpline = (Spline)Activator.CreateInstance(s.GetType(), s.UDimension, s.VDimension, knots, s.Derivation);
            return resultSpline;
        }

        //public CompositeSurface InterpolationDifference()
        //{

        //}

    }
}
