
using System;
using System.Threading.Tasks;
using HermiteInterpolation.Functions;
using HermiteInterpolation.Shapes.HermiteSpline;
using HermiteInterpolation.Utils;

namespace HermiteInterpolation.SplineKnots
{
    public class DirectKnotsGenerator : KnotsGenerator
    {

        public DirectKnotsGenerator(InterpolatedFunction function) : base(function)
        {
        }


        public override Knot[][] GenerateKnots(SurfaceDimension uDimension, SurfaceDimension vDimension)
        {
            var values = MyArrays.JaggedArray<Knot>(uDimension.KnotCount, vDimension.KnotCount);
            var uSize = Math.Abs(uDimension.Max - uDimension.Min) / (uDimension.KnotCount - 1);
            var vSize = Math.Abs(vDimension.Max - vDimension.Min) / (vDimension.KnotCount - 1);
            var u = uDimension.Min;
           
            //for (int i = 0; i < uDimension.KnotCount; i++, u += uSize)
            Parallel.For(0, uDimension.KnotCount, i =>
            {
                var v = vDimension.Min;
                for (int j = 0; j < vDimension.KnotCount; j++, v += vSize)
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
