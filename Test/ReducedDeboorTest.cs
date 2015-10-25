using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HermiteInterpolation.Numerics;
using HermiteInterpolation.Shapes.SplineInterpolation;
using HermiteInterpolation.SplineKnots;

namespace Test
{
    [TestClass]
    public class ReducedDeboorTest
    {
        [TestMethod]
        public void Test()
        {
            var mathExpression = MathExpression.CreateDefault("sin(sqrt(x^2+y^2))", "x", "y");
            //var aproximationFunction = InterpolativeMathFunction.FromExpression("x^4+y^4", "x", "y");
            //            var shape = new SegmentSurface(new double[] {-3, -2, -1, 0, 1, 2, 3},
            //                new double[] {-3, -2, -1, 0, 1, 2, 3},
            //  aproximationFunction, derivation);
            //var shape = new ClassicHermiteSurface(new double[] { -2, -1, 0, 1 }, new double[] { -2, -1, 0, 1 },
            // var shape = new HermiteShape(new double[] { -2, -1 }, new double[] { -2, -1 },
            var shape = new BicubicHermiteSurface(new SurfaceDimension(-3, 3, 7), new SurfaceDimension(-3, 3, 7),new ReducedDeBoorKnotsGenerator(mathExpression)
                //var shape = HermiteSurfaceFactoryHolder.CreateBiquartic(-3, 1, 7, -3, 1, 7,
               );
        }
    }
}
