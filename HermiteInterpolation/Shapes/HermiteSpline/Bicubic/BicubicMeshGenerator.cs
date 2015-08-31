using System;
using HermiteInterpolation.Functions;
using HermiteInterpolation.Primitives;
using HermiteInterpolation.SplineKnots;
using Microsoft.Xna.Framework;

namespace HermiteInterpolation.Shapes.HermiteSpline.Bicubic
{
    public class BicubicMeshGenerator : HermiteMeshGenerator
    {
        

        public BicubicMeshGenerator(SurfaceDimension uDimension, SurfaceDimension vDimension, KnotsGenerator knotsGenerator, Derivation derivation) 
            : base(uDimension, vDimension, knotsGenerator, derivation)
        {
        }


     


        /// <summary>
        /// </summary>
        /// <param name="uIdx"></param>
        /// <param name="i1"></param>
        /// <param name="vIdx"></param>
        /// <param name="j1"></param>
        /// <returns>SimpleSurface mesh</returns>
        protected override ISurface CreateMeshSegment(int uIdx, int vIdx)
        {
            var afv = Knots;
            var u0 = afv[uIdx][vIdx].X;
            var u1 = afv[uIdx+1][vIdx].X;
            var v0 = afv[uIdx][vIdx].Y;
            var v1 = afv[uIdx][vIdx+1].Y;
            var meshDensity = MeshDensity;
            var uKnotsDistance = Math.Abs(u1-u0);
           
            var xCount = Math.Ceiling(uKnotsDistance/meshDensity);
            //var xMeshDensity = (float)(uKnotsDistance / xCount);

            var yKnotDistance =  Math.Abs(v1 - v0);
            var yCount = Math.Ceiling(yKnotDistance / meshDensity);
            //var yMeshDensity = (float)(yKnotDistance / yCount);
            var verticesCount = (int)((++xCount) * (++yCount));



            var segmentMeshVertices = new VertexPositionNormalColor[verticesCount];

            var basisCalculator = new BicubicBasis(afv,Derivation);//BasisFactory.CreateBasis(Surface.Type);
            
            var phi = basisCalculator.Matrix(uIdx, vIdx);
            var k = 0;
            var x = (float) u0 ;
            for (var i = 0; i < xCount; i++,x += meshDensity)
            {
                var lambda1 = basisCalculator.Vector(x, u0, u1);
                var basis = phi.LeftMultiply(lambda1);       
                var y = (float) v0;
                for (var j = 0; j < yCount; j++,y += meshDensity)
                {
                    var lambda2 = basisCalculator.Vector(y, v0, v1);
                    var zv = basis.PointwiseMultiply(lambda2);
                    var z = (float) zv.Sum();                
                    segmentMeshVertices[k++] = new VertexPositionNormalColor(new Vector3(x, y, z), DefaultNormal,
                        DefaultColor);
                  
                }
               
            }

            return new SimpleSurface(segmentMeshVertices, (int)xCount, (int)yCount);
          
        }

        
    }
}