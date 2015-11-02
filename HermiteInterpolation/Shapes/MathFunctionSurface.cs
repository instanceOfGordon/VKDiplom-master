using System;
using System.Collections.Generic;
using HermiteInterpolation.Numerics;
using HermiteInterpolation.Numerics.MathFunctions;
using HermiteInterpolation.Primitives;
using HermiteInterpolation.Shapes.SplineInterpolation;
using Microsoft.Xna.Framework;

namespace HermiteInterpolation.Shapes
{
    public sealed class MathFunctionSurface : CompositeSurface
    {
        public SurfaceDimension UDimension { get; }
        public SurfaceDimension VDimension { get;  }

        public MathFunctionSurface(SurfaceDimension uDimension, SurfaceDimension vDimension,
            MathExpression mathExpression)
            : this(
                uDimension, vDimension, mathExpression.Compile()
                )
        {
           
            Name = mathExpression.Expression;
        }

        public MathFunctionSurface(SurfaceDimension uDimension, SurfaceDimension vDimension,
            MathFunction function)

        {
            UDimension = uDimension;
            VDimension = vDimension;
            var uCount_min_1 = uDimension.KnotCount - 1; //surface.UKnotsCount-1;
            var vCount_min_1 = vDimension.KnotCount - 1; //surface.VKnotsCount-1;

            var segments = new List<ISurface>(uCount_min_1*vCount_min_1);

            for (var i = 0; i < uCount_min_1; i++)
            {
                for (var j = 0; j < vCount_min_1; j++)
                {
                    var segment = CreateSegment(i, j, function);
                    segments.Add(segment);
                }
            }
            Segments = segments;         
        }

        private ISurface CreateSegment(int uIdx, int vIdx, 
            MathFunction function)
        {
            var meshDensity = Constants.MeshDensity;
            var uSize = Math.Abs(UDimension.Max - UDimension.Min)/(UDimension.KnotCount - 1);
            var vSize = Math.Abs(VDimension.Max - VDimension.Min)/(VDimension.KnotCount - 1);

            var u0 = UDimension.Min + uSize*uIdx; //afv[uIdx][vIdx].X;
            var u1 = UDimension.Min + uSize*(uIdx + 1);
            var v0 = VDimension.Min + vSize*vIdx;
            var v1 = VDimension.Min + vSize*(vIdx + 1);

            var uKnotsDistance = Math.Abs(u1 - u0);
            var xCount = Math.Ceiling(uKnotsDistance/meshDensity);
            var yKnotDistance = Math.Abs(v1 - v0);
            var yCount = Math.Ceiling(yKnotDistance/meshDensity);
            var verticesCount = (int) ((++xCount)*(++yCount));
            var segmentMeshVertices = new VertexPositionNormalColor[verticesCount];
            var k = 0;
            var x = (float) u0;
            for (var i = 0; i < xCount; i++, x += meshDensity)
            {
                x = x < u1 ? x : (float) u1;
                var y = (float) v0;
                for (var j = 0; j < yCount; j++, y += meshDensity)
                {
                    y = y < v1 ? y : (float) v1;
                    var z = (float) function.SafeCall(x, y);
                    segmentMeshVertices[k++] = new VertexPositionNormalColor(new Vector3(x, y, z), DefaultNormal,
                        DefaultColor);
                }
            }
            return new SimpleSurface(segmentMeshVertices, (int) xCount, (int) yCount);
        }
    }
}