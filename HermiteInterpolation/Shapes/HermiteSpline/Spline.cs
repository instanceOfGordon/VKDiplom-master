using System.Collections.Generic;
using System.Linq;
using HermiteInterpolation.Functions;
using HermiteInterpolation.SplineKnots;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Xna.Framework;

namespace HermiteInterpolation.Shapes.HermiteSpline
{


    public abstract class Spline : ISurface
    {
       

        protected static readonly Color DefaultColor = Color.FromNonPremultiplied(128, 128, 128, 255);
        protected static readonly Vector3 DefaultNormal = Vector3.Zero;


        private Color _color = DefaultColor;

        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                ColoredByShades(value);
            }
        }
        private readonly CompositeSurface _segments;

        public string Name { get; set; } = null;

        protected Spline(SurfaceDimension uDimension, SurfaceDimension vDimension,
            string functionExpression,string variableX,string variableY, Derivation derivation = Derivation.Zero)
            : this(uDimension, vDimension, new DirectKnotsGenerator(InterpolatedFunction.FromString(functionExpression,variableX,variableY)), derivation)
        {
            if (Name == null)
                Name = functionExpression;
        }


        protected Spline(SurfaceDimension uDimension, SurfaceDimension vDimension,
            InterpolatedFunction function, Derivation derivation = Derivation.Zero)
            : this(uDimension, vDimension, new DirectKnotsGenerator(function), derivation)
        {
            
        }

        protected Spline(SurfaceDimension uDimension, SurfaceDimension vDimension,
            KnotsGenerator knotsGenerator, Derivation derivation = Derivation.Zero) 
        {
            MeshDensity = 0.1f;

            //            _uKnotsDistance = Math.Abs(uMax - uMin) / uCount;
            //            
            //  _vKnotsDistance = Math.Abs(vMax - vMin) / vCount;
            //_interpolatedFunction = InterpolatedFunction;
            Knots = knotsGenerator.GenerateKnots(uDimension, vDimension);

            Derivation = derivation;
            _segments = CreateMesh();
        }

        public float MeshDensity { get; }


//        protected double UKnotsDistance { get { return _uKnotsDistance; } }
//        protected double VKnotsDistance { get { return _vKnotsDistance; } }

        public Derivation Derivation { get; protected set; }
        

        protected Knot[][] Knots { get; set; }


        internal virtual List<ISurface> CreateSegments()
        {
            //var surface = Surface;
            var uCount_min_1 = Knots.Length-1;//surface.UKnotsCount-1;
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

        protected CompositeSurface CreateMesh()
        {
            var segments = CreateSegments();
            
            return new CompositeSurface(segments);
        }

        protected abstract ISurface CreateSegment(int uIdx, int vIdx);

        protected delegate Vector<double> BasisVector(double t, double t0, double t1);


        public void Draw()
        {
            _segments.Draw();
        }

        public DrawStyle DrawStyle { get; set; }
        public float MinHeight => _segments.MinHeight;
        public float MaxHeight => _segments.MaxHeight ;
        public void ColoredSimple(Color color)
        {
            _color = color;
           _segments.ColoredSimple(color);
        }

        public void ColoredHeight()
        {
            _segments.ColoredHeight();
        }

        public void ColoredHeight(float fromHue, float toHue)
        {

           _segments.ColoredHeight(fromHue,toHue);
        }

        public void ColoredByShades(Color baseColor)
        {
            _color = baseColor;
            _segments.ColoredByShades(baseColor);
        }

        public void ColoredByShades(float baseHue)
        {
            _segments.ColoredByShades(baseHue);
        }
    }
}