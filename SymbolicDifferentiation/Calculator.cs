using System;
using System.Collections.Generic;

namespace SymbolicDifferentiation
{
    public class Calculator
    {
        public static int Calculate(string input, string[] variables, double[] values, ref double output)
        {
            if (variables.Length != values.Length) return -1;
            //output = -1;
            //string input = lpcsInput;
            // remove spaces
            input = input.Replace(" ","");
            // make all characters lower case
            input = input.ToLower();
            // Optimize "--"
            input = input.OptimizeSign();

            int nError;
            var vStack = new List<ExpressionItem>();
            // parse input equation and fill stack with operators and operands
            //var mathStack = new MathStack();
            if ((nError = MathStack.FillStack(input, vStack)) < 0)
                return nError;

            int nExpression = 0;
            // apply operators to operands
            nError = CalculateStack(vStack, variables, values, ref nExpression, ref output);

            return nError;
        }

        private static int CalculateStack(List<ExpressionItem> vStack, string[] variables, double[] values, ref int nExpression, ref double output)
        {
            // output = -1;
            var pQI = vStack[nExpression++];
            int nError;
            if (pQI.MCOperator != null)
            {
                double cu = 0, cv = 0;
                // get left operand calculation
                if ((nError = CalculateStack(vStack, variables, values, ref nExpression, ref cu)) < 0)
                    return nError;
                // get right operand calculation
                if ((nError = CalculateStack(vStack, variables, values, ref nExpression, ref cv)) < 0)
                    return nError;

                switch (pQI.MCOperator)
                {
                    case '-':   // c(u-v) = cu-cv
                        pQI.MNOutput = cu - cv;
                        break;
                    case '+':   // c(u+v) = cu+cv
                        pQI.MNOutput = cu + cv;
                        break;
                    case '*':   // c(u*v) = cu*cv
                        pQI.MNOutput = cu * cv;
                        break;
                    case '/':   // c(u/v) = cu/cv
                        if (cv == 0)
                            return (int)ErrorArgument.DivideByZero;
                        pQI.MNOutput = cu / cv;
                        break;
                    case '^':   // d(u^v) = cu^cv
                        if (cu < 0 && (int)cv != (double)cv)
                            return (int)ErrorArgument.InvalidArgument;
                        pQI.MNOutput = Math.Pow(cu, cv);
                        break;
                    case '%':   // d(u%v) = cu%cv
                        pQI.MNOutput = (int)cu % (int)cv;
                        break;
                }
            }
            else
                // get Expression calculation
                if ((nError = pQI.GetCalculation(variables, values, true)) < 0)
                return nError;
            // return resultant calculation
            output = pQI.MNOutput;

            return 0;
        }
        private static int CalculateStack(List<ExpressionItem> vStack, string[] variables, double[] values, ref int nExpression, ref string output)
        {
            var pQI = vStack[nExpression++];
            int nError;
            //output = "";
            if (pQI.MCOperator != null)
            {
                string cu = "", cv = "";
                // get left operand calculation
                if ((nError = CalculateStack(vStack, variables, values, ref nExpression, ref cu)) < 0)
                    return nError;
                // get right operand calculation
                if ((nError = CalculateStack(vStack, variables, values, ref nExpression, ref cv)) < 0)
                    return nError;

                if (!cu.IsNumeric() || !cv.IsNumeric())
                {
                    switch (pQI.MCOperator)
                    {
                        case '-':   // c(u-v) = cu-cv
                        case '+':   // c(u+v) = cu+cv
                            pQI.MStrOutput = cu + pQI.MCOperator + cv;
                            break;
                        case '*':   // c(u*v) = cu*cv
                            pQI.MStrOutput = cu + '*' + cv;
                            break;
                        case '/':   // c(u/v) = cu/cv
                            pQI.MStrOutput = cu + '/' + cv;
                            break;
                        case '^':   // c(u^v) = cu^cv
                            pQI.MStrOutput = cu + '^' + cv;
                            break;
                        case '%':   // c(u%v) = cu%cv
                            pQI.MStrOutput = cu + '%' + cv;
                            break;
                    }
                    pQI.MStrOutput = '(' + pQI.MStrOutput + ')';
                }
                else
                    switch (pQI.MCOperator)
                    {
                        case '-':   // c(u-v) = cu-cv
                            pQI.MStrOutput = string.Format(pQI.MStrOutput, "%s",
                                MathStack.TrimFloat(SupportedCalculationFunctions.Atof(cu) - SupportedCalculationFunctions.Atof(cv)));
                            //pQI.MStrOutput.Format("%s", TrimFloat(ATOF(cu) - ATOF(cv)));
                            break;
                        case '+':   // c(u+v) = cu+cv
                            pQI.MStrOutput = string.Format(pQI.MStrOutput, "%s",
                               MathStack.TrimFloat(SupportedCalculationFunctions.Atof(cu) + SupportedCalculationFunctions.Atof(cv)));
                            break;
                        case '*':   // c(u*v) = cu*cv
                            pQI.MStrOutput = string.Format(pQI.MStrOutput, "%s",
                                      MathStack.TrimFloat(SupportedCalculationFunctions.Atof(cu) * SupportedCalculationFunctions.Atof(cv)));
                            break;
                        case '/':   // c(u/v) = cu/cv
                            if (Math.Abs(SupportedCalculationFunctions.Atof(cv)) < 0.001)
                                return (int)ErrorArgument.DivideByZero;
                            pQI.MStrOutput = string.Format(pQI.MStrOutput, "%s",
                                         MathStack.TrimFloat(SupportedCalculationFunctions.Atof(cu) / SupportedCalculationFunctions.Atof(cv)));
                            break;
                        case '^':   // c(u^v) = cu^cv
                            pQI.MStrOutput = string.Format(pQI.MStrOutput, "%s",
                                      MathStack.TrimFloat(Math.Pow(SupportedCalculationFunctions.Atof(cu), SupportedCalculationFunctions.Atof(cv))));
                            break;
                        case '%':   // c(u%v) = cu%cv
                            pQI.MStrOutput = string.Format(pQI.MStrOutput, "%s",
                                    MathStack.TrimFloat((int)SupportedCalculationFunctions.Atof(cu) % (int)SupportedCalculationFunctions.Atof(cv)));
                            break;
                    }
            }
            else
                // get Expression calculation
                if ((nError = pQI.GetCalculation(variables,values)) < 0)
            {
                output = "Error evaluating expression";
                return nError;
            }
            // return resultant calculation
            output = pQI.MStrOutput;

            return 0;
        }


        public static string Calculate(string input,string[] variables,double[] values, bool bOptimize = false)
        {
            // remove spaces
            input = input.Remove(' ');
            // make all characters lower case
            input = input.ToLower();
            // Optimize "--"
            input = input.OptimizeSign();
            int nError;
            var vStack = new List<ExpressionItem>();
            // var mathStack = new MathStack();
            if ((nError = MathStack.FillStack(input, vStack)) < 0)
                switch (nError)
                {
                    case (int)ErrorArgument.NotNumeric:
                        throw new InvalidCastException("Not numeric");
                    case (int)ErrorArgument.DivideByZero:
                        throw new DivideByZeroException();
                    case (int)ErrorArgument.InvalidArgument:
                        throw new ArgumentException("Invalid argument");
                }
            string output = "";
            double nOutput = 0;
            int nExpression = 0;
            if (CalculateStack(vStack, variables, values, ref nExpression, ref nOutput) < 0)
            {
                nExpression = 0;
                // apply operators to operands
                CalculateStack(vStack, variables, values, ref nExpression, ref output);
            }
            else
            {
                output = MathStack.TrimFloat(nOutput);
            }

            //foreach (var expressionItem in vStack)
            //{
            //    if (expressionItem.MCOperator == null)
            //    {
            //        strStack += "c(" + expressionItem.MStrInput + ") = " + expressionItem.MStrOutput + "\r\n";
            //    }
            //    else
            //    {
            //        strStack+=expressionItem.MCOperator + "\r\n";
            //    }
            //    expressionItem.MStrStack = expressionItem.MStrStack.InsertTabs();
            //    strStack += expressionItem.MStrStack;
            //}
            if (bOptimize)
            {
                output = output.Optimize();
            }
            return output;
        }


    }

    static class SupportedCalculationFunctions
    {
        public static readonly Func<string, string> Acos = x => "log(" + x + "+sqrt(" + x + "*" + x + "-1))";

        public static readonly Func<string, string> Acosech =
            x => "log((sign(" + x + ")*sqrt(" + x + "*" + x + "+1)+1)/" + x + ")";

        public static readonly Func<string, double> Atof = x =>
        {
            switch (x)
            {
                case "E":
                    return Math.E;
                case "PI":
                    return Math.PI;
            }
            return Atof(x);
        };
        public static readonly Func<string, string> Acoth = x => "log((" + x + "+1)/(" + x + "-1))/2";
        public static readonly Func<string, string> Asech = x => "log((sqrt(-" + x + "*" + x + "+1)+1)/" + x + ")";
        public static readonly Func<string, string> Asinh = x => "log(" + x + "+sqrt(" + x + "*" + x + "+1))";
        public static readonly Func<string, string> Acosh = x => "log(" + x + "+sqrt(" + x + "*" + x + "-1))";
        public static readonly Func<string, string> Atanh = x => "log((1+" + x + ")/(1-" + x + "))/2";
        // supported functions calculation
        public static readonly Func<string,string[],double[], string> C = (u,vars,values) => Calculator.Calculate(u,vars,values);
        public static readonly Func<string,string[],double[], string> CSin = (u,vars,values)=> "sin(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CCos = (u,vars,values)=> "cos(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CTan = (u,vars,values)=> "tan(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CSec = (u,vars,values)=> "sec(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CCosec = (u,vars,values)=> "cosec(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CCot = (u,vars,values)=> "cot(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CSinh = (u,vars,values)=> "sinh(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CCosh = (u,vars,values)=> "cosh(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CTanh = (u,vars,values)=> "tanh(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CSech = (u,vars,values)=> "sech(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CCosech = (u,vars,values)=> "cosech(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CCoth = (u,vars,values)=> "coth(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CAsin = (u,vars,values)=> "asin(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CAcos = (u,vars,values)=> "acos(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CAtan = (u,vars,values)=> "atan(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CAsec = (u,vars,values)=> "asec(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CAcosec = (u,vars,values)=> "acosec(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CAcot = (u,vars,values)=> "acot(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CAsinh = (u,vars,values)=> "asinh(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CAcosh = (u,vars,values)=> "acosh(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CAtanh = (u,vars,values)=> "atanh(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CAsech = (u,vars,values)=> "asech(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CAcosech = (u,vars,values)=> "acosech(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CAcoth = (u,vars,values)=> "acoth(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CSqrt = (u,vars,values)=> "sqrt(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CLog10 = (u,vars,values)=> "log10(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CLog = (u,vars,values)=> "log(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CLn = (u,vars,values)=> CLog(u, vars, values);
        public static readonly Func<string,string[],double[], string> CSign = (u,vars,values)=> "sign(" + C(u,vars,values)+ ")";
        public static readonly Func<string,string[],double[], string> CAbs = (u,vars,values)=> "abs(" + C(u,vars,values)+ ")";
        //public delegate string MathFunction(string x);

        //public MathFunction Atof = x => (((x) == "e") ? Math.E : ((x) == "pi" ? Math.PI : Atof(x)));

        public static readonly Func<string, string> Sign = x => double.Parse(x) >= 0 ? 1.ToString() : (-1).ToString();
    }

    enum ErrorArgument
    {
        InvalidArgument = -1,
        DivideByZero = -2,
        NotNumeric = -3
    }
}