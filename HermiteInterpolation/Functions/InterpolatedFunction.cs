using System;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics;

namespace HermiteInterpolation.Functions
{
    public delegate double Function(double x, double y);

    /// <summary>
    ///     Defines function and it's derivations in form of delegates.
    /// </summary>
    public class InterpolatedFunction
    {
        // Constant for calculating derivations.
        //private const double H = 0.00001;

        /// <summary>
        ///     Parameter z specifies function to be approximated Hermite spline.
        ///     All required derivations are calculated automatically.
        /// </summary>
        /// <param name="z"></param>
        public InterpolatedFunction(Function z)
        {
           
            //const double h2 = 0.0001;
            Z = z;
            // Partial derivations 
            //const double h2 = 2*H;
            Dx = new Function(Differentiate.FirstPartialDerivative2Func(new Func<double, double, double>(z), 0));
                //(a,b)=>Differentiate.FirstPartialDerivative2Func((x,y)=>z(x,y), 0)(a,b);//(x, y) => (z(x + H, y) - z(x - H, y)) / h2;

            Dy = new Function(Differentiate.FirstPartialDerivative2Func(new Func<double, double, double>(z), 1));
                //(x, y) => (z(x, y + H) - z(x, y - H)) / h2;//

            Dxy = new Function(Differentiate.FirstPartialDerivative2Func(new Func<double, double, double>(Dx), 1));
                //Differentiate.FirstPartialDerivative2Func(Dx, 1);//(x, y) => (Dx(x, y + H) - Dx(x, y - H)) / h2;//
        }

        /// <summary>
        ///     Parameter z specifies function to be approximated Hermite spline.
        ///     All required derivations are exactly specified.
        /// </summary>
        /// <param name="z"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dxy"></param>
        public InterpolatedFunction(Function z, Function dx,
            Function dy, Function dxy)
        {
            Z = z;
            Dx = dx;
            Dy = dy;
            Dxy = dxy;
        }

        public Function Z { get; }
        public Function Dx { get; }
        public Function Dy { get; }
        public Function Dxy { get; }

        /// <summary>
        ///     Parse aproximation Z from string.
        ///     All required derivations are calculated automatically.
        /// </summary>
        /// <param name="mathExpression">String form of aproximated Z.</param>
        /// <param name="variableX">Substring of mathExpression used as first variable.</param>
        /// <param name="variableY">Substring of mathExpression used as second variable.</param>
        /// <returns>
        ///     Instance of class if mathExpression is correct math Z, othervise
        ///     returns null;
        /// </returns>
        public static InterpolatedFunction FromString(string mathExpression, string variableX, string variableY)
        {
            // TODO: osetrit vynimky pre neplatne vstupy

            var function = Functions.FromString(mathExpression, variableX, variableY);
            return function == null ? null : new InterpolatedFunction(function);
        }
    }
}