using System;
using HermiteInterpolation.Functions;
using HermiteInterpolation.Shapes.HermiteSpline.Bicubic;
using HermiteInterpolation.SplineKnots;
using HermiteInterpolation.Utils;

namespace HermiteInterpolation.Shapes.HermiteSpline.Biquartic
{
    internal class TwoBiquarticMeshGenerator : BicubicMeshGenerator
    {
        private readonly InterpolatedFunction _interpolatedFunction;
        private readonly double _uSize;
        private readonly double _vSize;
        //private readonly Knot[][] _newAproximationValues;
        internal TwoBiquarticMeshGenerator(double uMin, double uMax, int uCount, double vMin, double vMax, int vCount,
            InterpolatedFunction interpolatedFunction, KnotsGenerator knotsGenerator, Derivation derivation)
            : base(uMin, uMax, uCount, vMin, vMax, vCount, knotsGenerator, derivation)
        {
            _interpolatedFunction = interpolatedFunction;
            _uSize = Math.Abs(uMax - uMin)/(uCount - 1);
            _vSize = Math.Abs(vMax - vMin)/(vCount - 1);
            Knots = ClampedAproximationValues();
        }

//        internal override List<ISurface> CreateMesh()
//        {
//            var afvNew = ClampedAproximationValues();
////            var uCountNew_min_1 = uCountNew - 1;
////            var vCountNew_min_1 = vCountNew - 1;
//            var surface = Surface;
//            var uCount_min_1 = afvNew.Length;
//
//            var vCount_min_1 = afvNew[0].Length;
//
//            
//
//            var segments = new List<ISurface>(uCount_min_1 * vCount_min_1);
//
//            for (int i = 0; i < uCount_min_1; i++)
//            {
//                for (int j = 0; j < vCount_min_1; j++)
//                {
//                    var segment = CreateMeshSegment(i, i + 1, j, j + 1);
//                    segments.Add(segment);
//                }
//            }
//
//            return segments;
//
//            return null;
//        }

        private Knot[][] ClampedAproximationValues()
        {
            var afvOld = Knots;
            var uCountOld = afvOld.Length;
            var vCountOld = afvOld[0].Length;
            var uCountNew = (4*uCountOld - 3);
            var vCountNew = (2*vCountOld - 1);
            var afvNew = MyArrays.JaggedArray<Knot>(uCountNew, vCountNew);

            FillNewKnots(afvNew, afvOld);

          
            return afvNew;
        }

        private void FillNewKnots(Knot[][] newKnots,
            Knot[][] oldKnots)
        {
            var uCountOld = oldKnots.Length;
            var vCountOld = oldKnots[0].Length;
            var uCountNew = newKnots.Length;
            var vCountNew = newKnots[0].Length;

            var hx4 = _uSize;
            var hx = hx4/4;


            // z, dx, dy, dxy initialization
            OriginalKnots(newKnots, oldKnots, uCountOld, vCountOld);


            // z, dx, deltay, deltaxy
            double hy;
            double hy4;
            VerticalKnots(newKnots, uCountNew, vCountNew, out hy, out hy4);

            // z, Dx, dy, Dxy
            double hx2;
            HorizontalKnots(newKnots, hx, uCountNew, vCountNew, hx4, out hx2);

            // z, Dx, deltay, deltaxy
            InnerKnots(newKnots, uCountNew, vCountNew, hx, hy4, hy, hx2, hx4);

//            for (int i = 0; i < uCountNew; i++)
//            {
//                for (int j = 0; j <vCountNew; j++)
//                {
//                    if (i%4 == 0 && j%2 == 0) newKnots[i][j] = oldKnots[i/4][j/2];
//                    else if(i%2==0&&j%2==0) newKnots[i][j] = new Knot
//                    {
//                        X=oldKnots[]
//                    } 
//                }
//            }
        }

        private void InnerKnots(Knot[][] newKnots, int uCountNew, int vCountNew, double hx, double hy4,
            double hy,
            double hx2, double hx4)
        {
            for (var i = 2; i < uCountNew; i += 4)
            {
                for (var j = 1; j < vCountNew; j += 2)
                {
                    var u0v0 = newKnots[i - 2][j - 1];
                    var u1v0 = newKnots[i - 1][j - 1];
                    var u2v0 = newKnots[i][j - 1];
                    var u3v0 = newKnots[i + 1][j - 1];
                    var u4v0 = newKnots[i + 2][j - 1];

                    var u0v1 = newKnots[i - 2][j];
                    var u1v1 = new Knot();
                    var u2v1 = new Knot();
                    var u3v1 = new Knot();
                    var u4v1 = newKnots[i + 2][j];

                    var u0v2 = newKnots[i - 2][j + 1];
                    var u1v2 = newKnots[i - 1][j + 1];
                    var u2v2 = newKnots[i][j + 1];
                    var u3v2 = newKnots[i + 1][j + 1];
                    var u4v2 = newKnots[i + 2][j + 1];


                    var x = u0v1.X + hx;
                    var y = u0v1.Y;

                    // u1v1 not finished
                    u1v1.X = x;
                    u1v1.Y = y;
                    u1v1.Z = _interpolatedFunction.Z(x, y);
                    u1v1.Dy = -(3*(u1v0.Z - u1v2.Z) + hx*(u1v0.Dx + u1v2.Dx))/
                              hy4;
                    u1v1.Dxy = (u0v0.Dxy + u2v0.Dxy + u0v2.Dxy + u2v2.Dxy +
                                (3*
                                 ((u0v0.Dy - u2v0.Dy + u0v2.Dy - u2v2.Dy)/hx +
                                  (u0v0.Dx + u2v0.Dx - u0v2.Dx - u2v2.Dx)/hy +
                                  (3*(u0v0.Z - u2v0.Z - u0v2.Z + u2v2.Z))/(hx*hy))))/
                               16;

                    x += hx2;

                    // u3v1 not finished
                    u3v1.X = x;
                    u3v1.Y = y;
                    u3v1.Z = _interpolatedFunction.Z(x, y);
                    u3v1.Dy = -(3*(u3v0.Z - u3v2.Z) + hx*(u3v0.Dx + u3v2.Dx))/
                              hy4;
                    u3v1.Dxy = (u2v0.Dxy + u4v0.Dxy + u2v2.Dxy + u4v2.Dxy +
                                (3*
                                 ((u2v0.Dy - u4v0.Dy + u2v2.Dy - u4v2.Dy)/hx +
                                  (u2v0.Dx + u4v0.Dx - u2v2.Dx - u4v2.Dx)/hy +
                                  (3*(u2v0.Z - u4v0.Z - u2v2.Z + u4v2.Z))/(hx*hy))))/
                               16;

                    x -= hx;
                    // u2v1 finished
                    u2v1.X = x;
                    u2v1.Y = y;
                    u2v1.Z = _interpolatedFunction.Z(x, y);
                    u2v1.Dx = (u0v1.Dx + u4v1.Dx + (3*(u0v1.Z + 4*(u3v1.Z - u1v1.Z) - u4v1.Z))/hx)/
                              14;
                    u2v1.Dy = -(3*(u2v0.Z - u2v2.Z) + hx*(u2v0.Dx + u2v2.Dx))/
                              hy4;
                    u2v1.Dxy = -(3*(u2v0.Dx - u2v2.Dx) + hy*(u2v0.Dxy + u2v2.Dxy))/
                               hy4;

                    // u1v1 finished
                    u1v1.Dx = -(3*(u0v1.Z - u2v1.Z) + hx*(u0v1.Dx + u2v1.Dx))/
                              hx4;

                    // u3v1 finished
                    u3v1.Dx = -(3*(u2v1.Z - u4v1.Z) + hx*(u2v1.Dx + u4v1.Dx))/
                              hx4;

                    newKnots[i - 1][j] = u1v1;
                    newKnots[i][j] = u2v1;
                    newKnots[i + 1][j] = u3v1;
                }
            }
        }

        private void HorizontalKnots(Knot[][] newKnots, double hx, int uCountNew, int vCountNew, double hx4,
            out double hx2)
        {
            hx2 = hx*2;
//           var basisCalculator = new BiquarticBasis();
//            var basisVector = GetBasisVector(basisCalculator);

            for (var i = 2; i < uCountNew; i += 4)
            {
                for (var j = 0; j < vCountNew; j += 2)
                {
                    // finished
                    var u0 = newKnots[i - 2][j];
                    var u1 = new Knot();
                    var u2 = new Knot();
                    var u3 = new Knot();
                    // finished
                    var u4 = newKnots[i + 2][j];

                    //var x = u0.X + hx2;
                    //var y = u0.Y;
                    // z values
                    //var L = basisVector
//                    u2.Z = InterpolatedFunction.Z(x, y);
//                    x -= hx;
//                    u1.Z = basis.FunctionVector()
//                    x += hx2;
//                    u3.Z = InterpolatedFunction.Z(x, y);

                    var x = u0.X + hx;
                    var y = u0.Y;

                    // u1 not finished
                    u1.X = x;
                    u1.Y = y;
                    u1.Z = _interpolatedFunction.Z(x, y);
                    u1.Dy = _interpolatedFunction.Dy(x, y);

                    x += hx2;
                    // u3 not finished
                    u3.X = x;
                    u3.Y = y;
                    u3.Z = _interpolatedFunction.Z(x, y);
                    u3.Dy = _interpolatedFunction.Dy(x, y);

                    x -= hx;
                    // u2 finished
                    u2.X = x;
                    u2.Y = y;
                    u2.Z = _interpolatedFunction.Z(x, y);
                    u2.Dy = _interpolatedFunction.Dy(x, y);
                    u2.Dx = (u0.Dx + u4.Dx + (3*(u0.Z + 4*(u3.Z - u1.Z) - u4.Z))/hx)/
                            14;
                    u2.Dxy = (u0.Dxy + u4.Dxy + (3*(u0.Dy + 4*(u3.Dy - u1.Dy) - u4.Dy))/hx)/
                             14;

                    // u1 finished
                    u1.Dx = -(3*(u0.Z - u2.Z) + hx*(u0.Dx + u2.Dx))/
                            hx4;
                    u1.Dxy = -(3*(u0.Dy - u2.Dy) + hx*(u0.Dxy + u2.Dxy))/
                             hx4;

                    // u3 finished
                    u3.Dx = -(3*(u2.Z - u4.Z) + hx*(u2.Dx + u4.Dx))/
                            hx4;
                    u3.Dxy = -(3*(u2.Dy - u4.Dy) + hx*(u2.Dxy + u4.Dxy))/
                             hx4;

                    newKnots[i - 1][j] = u1;
                    newKnots[i][j] = u2;
                    newKnots[i + 1][j] = u3;
                }
            }
        }

        private void VerticalKnots(Knot[][] newKnots, int uCountNew, int vCountNew, out double hy,
            out double hy4)
        {
            hy = _vSize/2;
            hy4 = hy*4;
            for (var i = 0; i < uCountNew; i += 4)
            {
                for (var j = 1; j < vCountNew; j += 2)
                {
                    // finished
                    var v0 = newKnots[i][j - 1];
                    var v1 = new Knot();
                    // finished
                    var v2 = newKnots[i][j + 1];

                    var x = v0.X;
                    var y = v0.Y + hy;

                    v1.X = x;
                    v1.Y = y;
                    v1.Z = _interpolatedFunction.Z(x, y);
                    v1.Dx = _interpolatedFunction.Dx(x, y);
                    v1.Dy = -(3*(v0.Z - v2.Z) + hy*(v0.Dy + v2.Dy))/
                            hy4;
                    v1.Dxy = -(3*(v0.Dx - v2.Dx) + hy*(v0.Dxy + v2.Dxy))/
                             hy4;
                    // finished
                    newKnots[i][j] = v1;
                }
            }
        }

        private static void OriginalKnots(Knot[][] newKnots, Knot[][] oldKnots, int uCountOld,
            int vCountOld)
        {
            for (var i = 0; i < uCountOld; i++)
            {
                for (var j = 0; j < vCountOld; j++)
                {
                    newKnots[4*i][2*j] = oldKnots[i][j];
                }
            }
        }
    }
}