using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CalculationEngine;

namespace HermiteInterpolation.MathFunctions
{
    public class MathExpression
    {
        public string Expression { get; }
        public string[] Variables { get; }
        //public string VariableX { get; }

        /// <summary>
        ///    Class describing symbolic mathematic function.
        /// </summary>
        /// <param name="expression">String form of aproximated MathFunction.</param>
        /// <param name="variables">Substrings of expression used as variables.</param>
        /// <returns>
        ///     MathFunction delegate if mathExpression is correct math MathFunction, othervise
        ///     returns null;
        /// </returns>
        public MathExpression(string expression,params string[] variables)
        {
            Expression = expression;
            Variables = variables;
        }

        public MathFunction Compile()
        {
           return MathFunctions.CompileFromString(Expression, Variables);
        }
    }

         
           

}