using System.Collections.Generic;
using System.Linq;
using HermiteInterpolation.Functions;
using HermiteInterpolation.SplineKnots;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Xna.Framework;

namespace HermiteInterpolation.Shapes.HermiteSpline
{


    public abstract class HermiteMeshGenerator
    {
       

        protected static readonly Color DefaultColor = Color.FromNonPremultiplied(128, 128, 128, 255);
        protected static readonly Vector3 DefaultNormal = Vector3.Zero;

       
        protected HermiteMeshGenerator(SurfaceDimension uDimension, SurfaceDimension vDimension,
            KnotsGenerator knotsGenerator, Derivation derivation)
        {
            MeshDensity = 0.1f;

            //            _uKnotsDistance = Math.Abs(uMax - uMin) / uCount;
            //            
            //  _vKnotsDistance = Math.Abs(vMax - vMin) / vCount;
            //_interpolatedFunction = InterpolatedFunction;
            Knots = knotsGenerator.GenerateKnots(uDimension, vDimension);

            Derivation = derivation;
        }

        public float MeshDensity { get; }


//        protected double UKnotsDistance { get { return _uKnotsDistance; } }
//        protected double VKnotsDistance { get { return _vKnotsDistance; } }

        public Derivation Derivation { get; protected set; }
        

        protected Knot[][] Knots { get; set; }


        internal virtual List<ISurface> CreateMesh()
        {
            //var surface = Surface;
            var uCount_min_1 = Knots.Length-1;//surface.UKnotsCount-1;
            var vCount_min_1 = Knots[0].Length - 1;//surface.VKnotsCount-1;
        
            var segments = new List<ISurface>(uCount_min_1 * vCount_min_1);

            for (int i = 0; i < uCount_min_1; i++)
            {
                for (int j = 0; j < vCount_min_1; j++)
                {
                    var segment = CreateMeshSegment(i, j);
                    segments.Add(segment);
                }
            }

            return segments;
        }

        internal HermiteSurface CreateSurface()
        {
            var segments = CreateMesh();
            var afv = Knots;
            var umin = afv[0][0].X;
            var vmin = afv[0][0].Y;
            var umax = afv.Last().Last().X;
            var vmax = afv.Last().Last().Y;
            var ucount = afv.Length;
            var vcount = afv[0].Length;
            return new HermiteSurface(new SurfaceDimension(umin, umax,ucount),new SurfaceDimension(vmin,vmax,vcount),segments,Derivation);
        }

        protected abstract ISurface CreateMeshSegment(int uIdx, int vIdx);

        protected delegate Vector<double> BasisVector(double t, double t0, double t1);

       
    }
}