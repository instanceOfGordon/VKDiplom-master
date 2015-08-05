using System;
using MathNet.Numerics;

namespace HermiteInterpolation.Functions
{
    /// <summary>
    /// Defines function and it's derivations in form of delegates.
    /// </summary>
    public class InterpolatedFunction
    {
        // Constant for calculating derivations.
        private const double H = 0.01;

        /// <summary>
        ///     Z to approximate by Hermite splines.
        ///     All required derivations are calculated automatically.
        /// </summary>
        /// <param name="z"></param>
        public InterpolatedFunction(Func<double, double, double> z)
        {
            Z = z;
            // Partial derivations 
           // const double h2 = 2*H;
            Dx = Differentiate.FirstPartialDerivative2Func(z, 0);//(x, y) => (z(x + H, y) - z(x - H, y))/(h2);
           
            Dy = Differentiate.FirstPartialDerivative2Func(z, 1);//(x, y) => (z(x, y + H) - z(x, y - H))/(h2);
            Dxy = Differentiate.FirstPartialDerivative2Func(Dx, 1);//(x, y) => (Dx(x, y + H) - Dx(x, y - H))/(h2);
        }

        /// <summary>
        ///     Z to approximate by Hermite splines.
        ///     All required derivations are exactly specified.
        /// </summary>
        /// <param name="z"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dxy"></param>
        public InterpolatedFunction(Func<double, double, double> z, Func<double, double, double> dx,
            Func<double, double, double> dy, Func<double, double, double> dxy)
        {
            Z = z;
            Dx = dx;
            Dy = dy;
            Dxy = dxy;
        }

       public Func<double, double, double> Z { get; private set; }
 
        public Func<double, double, double> Dx { get; private set; }
        public Func<double, double, double> Dy { get; private set; }

        public Func<double, double, double> Dxy { get; private set; }

        /// <summary>
        ///     Parse aproximation Z from string.
        ///     All required derivations are calculated automatically.
        /// </summary>
        /// <param name="expression">String form of aproximated Z.</param>
        /// <param name="xVariable">Substring of expression used as first variable.</param>
        /// <param name="yVariable">Substring of expression used as second variable.</param>
        /// <returns>
        ///     Instance of class if expression is correct math Z, othervise
        ///     returns null;
        /// </returns>
        public static InterpolatedFunction FromString(string expression, string xVariable, string yVariable)
        {
            // TODO: osetrit vynimky pre neplatne vstupy

            var function = HermiteInterpolation.Functions.Functions.FromString(expression, xVariable, yVariable);
            return function == null ? null : new InterpolatedFunction(function);
        }

        
    }
}