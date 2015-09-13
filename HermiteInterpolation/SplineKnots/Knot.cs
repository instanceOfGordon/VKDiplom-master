namespace HermiteInterpolation.SplineKnots
{
    public class Knot
    {

        public double X { get; internal set; }
        public double Y { get; internal set; }
        public double Z { get; internal set; }
        public double Dx { get; internal set; }
        public double Dy { get; internal set; }
        public double Dxy { get; internal set; }

        /// <summary>
        /// Defines known function value with it's derivation.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dxy"></param>
        public Knot(double x, double y, double z, double dx, double dy, double dxy)
        {
            X = x;
            Y = y;
            Z = z;
            Dx = dx;
            Dy = dy;
            Dxy = dxy;
        }

        public Knot(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Knot()
        {
            
        }

        public override string ToString()
        {
            return $"X: {X:0.00}, Y: {Y:0.00}, Z: {Z:0.00}, Dx: {Dx:0.00}, Dy: {Dy:0.00}, Dxy: {Dxy:0.00}";
        }
    }
}