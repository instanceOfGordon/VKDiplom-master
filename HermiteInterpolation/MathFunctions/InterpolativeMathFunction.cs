using System;
using MathNet.Numerics;

namespace HermiteInterpolation.MathFunctions
{
    public delegate double MathFunction(params double[] variables);

    /// <summary>
    ///     Defines function and it's derivations in form of delegates.
    /// </summary>
    public class InterpolativeMathFunction
    {
        // Constant for calculating derivations.
        //private const double H = 0.00001;

        /// <summary>
        ///     Parameter z specifies function to be approximated Hermite spline.
        ///     All required derivations are calculated automatically.
        /// </summary>
        /// <param name="z"></param>
        public InterpolativeMathFunction(MathFunction z)
        {
            //const double h = Epsilon;
            //const double h2 = h*2;
            Z = z;
            // Partial derivations 
            //const double h2 = 2*H;
            //Differentiate.FirstPa
            Dx = new MathFunction(Differentiate.FirstPartialDerivativeFunc(new Func<double[], double>(z), 0));
            //(a,b)=>Differentiate.FirstPartialDerivative2Func((x,y)=>z(x,y), 0)(a,b);
            //(x, y) => (z(x + x*h, y) - z(x - x*h, y)) / (h2*x*x);

            Dy = new MathFunction(Differentiate.FirstPartialDerivativeFunc(new Func<double[], double>(z), 1));
                //(x, y) => (z(x, y + y*h) - z(x, y - y*h)) / (y*y*h2);//

            Dxy = new MathFunction(Differentiate.FirstPartialDerivativeFunc(new Func<double[], double>(Dx), 1));
            //Differentiate.FirstPartialDerivative2Func(Dx, 1);
            //(x, y) => (Dx(x, y + y*h) - Dx(x, y - y*h)) / (y*y*h2);//

            // better derivation [f(x-2h) - 8f(x-h) + 8f(x+h) - f(x+2h)] / 12h
        }

        /// <summary>
        ///     Parameter z specifies function to be approximated Hermite spline.
        ///     All required derivations are exactly specified.
        /// </summary>
        /// <param name="z"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dxy"></param>
        public InterpolativeMathFunction(MathFunction z, MathFunction dx,
            MathFunction dy, MathFunction dxy)
        {
            Z = z;
            Dx = dx;
            Dy = dy;
            Dxy = dxy;
        }

        public MathFunction Z { get; }
        public MathFunction Dx { get; }
        public MathFunction Dy { get; }
        public MathFunction Dxy { get; }

        //public MathFunction Get(Derivation derivation)
        //{
        //    switch (derivation)
        //    {
                    
        //    }
        //}

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
        public static InterpolativeMathFunction FromString(string mathExpression, string variableX, string variableY)
        {
            // TODO: osetrit vynimky pre neplatne vstupy

            var function = MathFunctions.FromString(mathExpression, variableX, variableY);
            return function == null ? null : new InterpolativeMathFunction(function);
        }
    }
}