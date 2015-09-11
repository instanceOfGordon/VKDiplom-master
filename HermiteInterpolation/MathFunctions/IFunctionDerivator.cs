namespace HermiteInterpolation.MathFunctions
{
    public interface IFunctionDerivator
    {
        string PartialDerivateExpression(string functionExpression, string respectToVariable);

        MathFunction PartialDerivative(string functionExpression, string respectToVariable, params double[] variables);
    }
}