using System;
using System.Threading;
using HermiteInterpolation.Numerics;
using HermiteInterpolation.Numerics.MathFunctions;
using MathNet.Numerics;
//using SymbolicDifferentiation;

namespace HermiteInterpolation.MathFunctions
{
    /// <summary>
    ///     Defines expression and it's derivations in form of delegates.
    /// </summary>
    public class InterpolativeMathFunction
    {
        /// <summary>
        ///     Parameter z specifies expression to be approximated Hermite spline.
        ///     All required derivations are calculated automatically.
        /// </summary>
        /// <param name="z"></param>
        public InterpolativeMathFunction(MathFunction z)
        {
            Z = z;
            Dx = new MathFunction(Differentiate.FirstPartialDerivativeFunc(new Func<double[], double>(z), 0));
            Dy = new MathFunction(Differentiate.FirstPartialDerivativeFunc(new Func<double[], double>(z), 1));
            Dxy = new MathFunction(Differentiate.FirstPartialDerivativeFunc(new Func<double[], double>(Dx), 1));
        }

        /// <summary>
        ///     Parameter z specifies expression to be approximated Hermite spline.
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

        public static InterpolativeMathFunction operator +(InterpolativeMathFunction f1, InterpolativeMathFunction f2)
        {
            return new InterpolativeMathFunction(vars => f1.Z(vars) + f2.Z(vars));
        }

        public static InterpolativeMathFunction operator -(InterpolativeMathFunction f1, InterpolativeMathFunction f2)
        {
            return new InterpolativeMathFunction(vars => f1.Z(vars) - f2.Z(vars));
        }

        public static InterpolativeMathFunction operator *(InterpolativeMathFunction f1, InterpolativeMathFunction f2)
        {
            return new InterpolativeMathFunction(vars => f1.Z(vars)*f2.Z(vars));
        }

        public static InterpolativeMathFunction operator /(InterpolativeMathFunction f1, InterpolativeMathFunction f2)
        {
            return new InterpolativeMathFunction(vars => f1.Z(vars)/f2.Z(vars));
        }

           
        /// <summary>
        ///     Parse aproximation Z from string.
        ///     All required derivations are calculated automatically.
        /// </summary>
        /// <param name="mathExpression">String form of aproximated Z.</param>
        /// <param name="variableX">Substring of mathExpression used as first variable.</param>
        /// <param name="variableY">Substring of mathExpression used as second variable.</param>
        /// <returns>
        ///     Instance of class if mathExpression is correct expression Z, othervise
        ///     returns null;
        /// </returns>
        public static InterpolativeMathFunction FromExpression(string mathExpression, string variableX,
            string variableY)
        {
            //// TODO: osetrit vynimky pre neplatne vstupy           
            return FromExpression(MathExpression.CreateDefault(mathExpression, variableX, variableY));
        }

      

        public static InterpolativeMathFunction FromExpression(MathExpression expression)
        {
            // TODO: osetrit vynimky pre neplatne vstupy
            var f = expression.Compile();
            var dx = expression.CompileDerivative("x");
            var dy = expression.CompileDerivative("y");
            var dxy = expression.CompileDerivative("x", "y");
            
            return new InterpolativeMathFunction(f,dx,dy,dxy);
        }
    }

}