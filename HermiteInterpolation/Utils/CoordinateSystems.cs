using System;
using MathNet.Numerics.Financial;
using Microsoft.Xna.Framework;

namespace HermiteInterpolation.Utils
{
    public static class CoordinateSystems
    {
        public static Vector3 FromSphericalToCartesian(Vector3 coordinates)
        {
            return FromSphericalToCartesian(coordinates.X, coordinates.Y, coordinates.Z);
        }

        public static Vector3 FromCartesianToSpherical(float x, float y, float z)
        {
            var radius = (float)Math.Sqrt(x*x + y*y + z*z);
//            var inclination = (float)Math.Acos(z/radius);
//            var azimuth = (float)Math.Atan(y / x);
            var inclination = (float)Math.Acos(y / radius);
            var azimuth = (float)Math.Atan(z / x);
            //return new Vector3(radius, azimuth, inclination);
            return new Vector3(radius,inclination,azimuth);
        }

        public static Vector3 FromCartesianToSpherical(Vector3 coordinates)
        {
            return FromCartesianToSpherical(coordinates.X, coordinates.Y, coordinates.Z);
        }

        public static Vector3 FromSphericalToCartesian(float radius, float inclination, float azimuth)
        {
            var radiusMulSinInclination = radius*Math.Sin(inclination);
            var x = (float)(radiusMulSinInclination * Math.Cos(azimuth));
            var z = (float)(radiusMulSinInclination * Math.Sin(azimuth));
            var y = (float) (radius*Math.Cos(inclination));
            //return new Vector3(x,y,z);
            return new Vector3(x, y, z);
        }
    }
}