using System;
using MathNet.Numerics;

//using SymbolicDifferentiation;

namespace HermiteInterpolation.Numerics.MathFunctions
{
    /// <summary>
    ///     Defines expression and it's derivations in form of delegates.
    /// </summary>
    public class InterpolativeMathFunction
    {
        /// <summary>
        ///     All required derivations are calculated automatically.
        /// </summary>
        /// <param name="z">
        /// Math function to be approximated by Hermite spline.
        /// MathFunction z is delegate to method with two doubles as attributes, which returns double.
        /// </param>
        public InterpolativeMathFunction(MathFunction z)
        {
            Z = z;
            Dx = new MathFunction(Differentiate.FirstPartialDerivativeFunc(new Func<double[], double>(z), 0));
            Dy = new MathFunction(Differentiate.FirstPartialDerivativeFunc(new Func<double[], double>(z), 1));
            Dxy = new MathFunction(Differentiate.FirstPartialDerivativeFunc(new Func<double[], double>(Dx), 1));
        }

        /// <summary>
        /// </summary>
        /// <param name="z">
        /// Math function to be approximated by Hermite spline.
        /// MathFunction z is delegate to method with two doubles as attributes, which returns double.
        /// </param>
        /// <param name="dx">Partial differentiation with respect to first variable.</param>
        /// <param name="dy">Partial differentiation with respect to second variable.</param>
        /// <param name="dxy">Partial differentiation with respect to both variables.</param>
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
        ///     Returns instance of InterpolativeMathFunction which interprets string as math function.
        ///     All required derivations are interpreted automatically.
        /// </summary>
        /// <param name="mathExpression">Interpreted math function of two real variables i.e. "x^2 + y^2".</param>
        /// <param name="variableX">Substring of mathExpression used as first variable i.e. "x".</param>
        /// <param name="variableY">Substring of mathExpression used as second variable i.e. "y".</param>
        /// <returns>
        ///     Instance of class if mathExpression is in correct format, othervise
        ///     returns null;
        /// </returns>
        public static InterpolativeMathFunction FromExpression(string mathExpression, string variableX,
            string variableY)
        {
            //// TODO: osetrit vynimky pre neplatne vstupy           
            return FromExpression(MathExpression.CreateDefault(mathExpression, variableX, variableY));
        }

        /// <summary>
        ///     Returns instance of InterpolativeMathFunction with automatically generated derivations.
        /// </summary>
        /// <param name="expression">Interpreted math function of two real variables i.e. "x^2 + y^2".</param>
        /// <returns>
        ///     Instance of class if mathExpression is in correct format, othervise
        ///     returns null;
        /// </returns>
        public static InterpolativeMathFunction FromExpression(MathExpression expression)
        {
            // TODO: osetrit vynimky pre neplatne vstupy
            var f = expression.Interpret();
            var dx = expression.InterpretMathDifferentiation(expression.Variables[0]);
            var dy = expression.InterpretMathDifferentiation(expression.Variables[1]);
            var dxy = expression.InterpretMathDifferentiation(expression.Variables[0], expression.Variables[1]);
            
            return new InterpolativeMathFunction(f,dx,dy,dxy);
        }
    }

}