using HermiteInterpolation.MathFunctions;


namespace HermiteInterpolation.Numerics
{
    

    public abstract class MathExpression
    {
        public string Expression { get; }
        public string[] Variables { get; }

        protected MathExpression(string expression, string[] variables)
        {
            Expression = expression;
            Variables = variables;
            
        }

        public abstract MathFunction Compile();
        public abstract MathFunction Differentiate(params string[] respectTovariables);

        public static MathExpression CreateDefault(string expression, params string[] variables)
        {
            return new NumericMathExpression(expression, variables);
            //return new SymbolicMathExpression(expression, variables);
        }


    }
}