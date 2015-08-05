
using System;
using System.Threading.Tasks;
using HermiteInterpolation.Functions;
using HermiteInterpolation.Shapes.HermiteSpline;
using HermiteInterpolation.Utils;

namespace HermiteInterpolation.SplineKnots
{
    public class DirectKnotsGenerator : IKnotsGenerator
    {
        public InterpolatedFunction Function { get; protected set; }

        public DirectKnotsGenerator(InterpolatedFunction function)
        {
            Function = function;
        }

        public Knot[][] ComputeKnots(HermiteSurface surface)
        {
            return ComputeKnots(surface.UMinKnot, surface.UMaxKnot, surface.UKnotsCount, surface.VMinKnot, surface.VMaxKnot,
                surface.VKnotsCount);
        }

        public virtual Knot[][] ComputeKnots(double uMin, double uMax, int uCount, double vMin,
            double vMax, int vCount)
        {
            var values = MyArrays.JaggedArray<Knot>(uCount, vCount);
            var uSize = Math.Abs(uMax - uMin) / (uCount - 1);
            var vSize = Math.Abs(vMax - vMin) / (vCount - 1);
            var u = uMin;
           
            //for (int i = 0; i < uCount; i++, u += uSize)
            Parallel.For(0, uCount, i =>
            {
                var v = vMin;
                for (int j = 0; j < vCount; j++, v += vSize)
                {
                    var z = Functions.Functions.NaNSafeCall(Function.Z, u, v); //Z(u, v);
                    var dx = Functions.Functions.NaNSafeCall(Function.Dx, u, v); //Dx(u, v);
                    var dy = Functions.Functions.NaNSafeCall(Function.Dy, u, v); //Dy(u, v);
                    var dxy = Functions.Functions.NaNSafeCall(Function.Dxy, u, v); //Dxy(u, v);
                    values[i][j] = new Knot(u, v, z, dx, dy, dxy);

                }
                u += uSize;
            });
            return values;
        }

        
    }
}
