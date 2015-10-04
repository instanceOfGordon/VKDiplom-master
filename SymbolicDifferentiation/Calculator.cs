using System;
using System.Collections.Generic;
using System.Globalization;

namespace SymbolicDifferentiation
{
    public class Calculator
    {
        

        public static int Calculate(string input, double xVal, double yVal, ref double output)
        {

            var xValStr = xVal.ToString();//.Replace("e","");//CultureInfo.InvariantCulture);//.Replace(",",".");
            var yValStr = yVal.ToString();//.Replace("e", "");//CultureInfo.InvariantCulture);//.Replace(",", ".");
            input = xVal<0? input.Replace("x", "(" + xValStr + ")") : input.Replace("x", xValStr);
            input = yVal<0? input.Replace("y", "("+ yValStr + ")") : input.Replace("y", yValStr);
            return Calculate(input, ref output);
        }

        public static int Calculate(string input, ref double output)
        {
         
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

            //foreach (var expressionItem in vStack)
            //{
            //    for (int i = 0; i < values.Length; i++)
            //    {
            //        if (expressionItem.MStrInput == variables[i])
            //            expressionItem.MStrInput = values.ToString();
            //        else if (expressionItem.MStrInput.Contains("("+variables[i]+")"))
                    
            //    }
            //}

            int nExpression = 0;
            // apply operators to operands
            nError = CalculateStack(vStack, ref nExpression, ref output);

            return nError;
        }

        private static int CalculateStack(List<ExpressionItem> vStack, ref int nExpression, ref double output)
        {
            // output = -1;
            var pQI = vStack[nExpression++];
            int nError;
            if (pQI.MCOperator != null)
            {
                double cu = 0, cv = 0;
                // get left operand calculation
                if ((nError = CalculateStack(vStack, ref nExpression, ref cu)) < 0)
                    return nError;
                // get right operand calculation
                if ((nError = CalculateStack(vStack,  ref nExpression, ref cv)) < 0)
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
                if ((nError = pQI.GetCalculation(true)) < 0)
                return nError;
            // return resultant calculation
            output = pQI.MNOutput;

            return 0;
        }
        private static int CalculateStack(List<ExpressionItem> vStack, ref int nExpression, ref string output)
        {
            var pQI = vStack[nExpression++];
            int nError;
            //output = "";
            if (pQI.MCOperator != null)
            {
                string cu = "", cv = "";
                // get left operand calculation
                if ((nError = CalculateStack(vStack, ref nExpression, ref cu)) < 0)
                    return nError;
                // get right operand calculation
                if ((nError = CalculateStack(vStack, ref nExpression, ref cv)) < 0)
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
                if ((nError = pQI.GetCalculation()) < 0)
            {
                output = "Error evaluating expression";
                return nError;
            }
            // return resultant calculation
            output = pQI.MStrOutput;

            return 0;
        }


        internal static string Calculate(string input, bool bOptimize = false)
        {
            // remove spaces
            input = input.Replace(" ", "");
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
            if (CalculateStack(vStack, ref nExpression, ref nOutput) < 0)
            {
                nExpression = 0;
                // apply operators to operands
                CalculateStack(vStack, ref nExpression, ref output);
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
        public static readonly Func<string, string> C = (u) => Calculator.Calculate(u);
        public static readonly Func<string, string> CSin = (u)=> "sin(" + C(u)+ ")";
        public static readonly Func<string, string> CCos = (u)=> "cos(" + C(u)+ ")";
        public static readonly Func<string, string> CTan = (u)=> "tan(" + C(u)+ ")";
        public static readonly Func<string, string> CSec = (u)=> "sec(" + C(u)+ ")";
        public static readonly Func<string, string> CCosec = (u)=> "cosec(" + C(u)+ ")";
        public static readonly Func<string, string> CCot = (u)=> "cot(" + C(u)+ ")";
        public static readonly Func<string, string> CSinh = (u)=> "sinh(" + C(u)+ ")";
        public static readonly Func<string, string> CCosh = (u)=> "cosh(" + C(u)+ ")";
        public static readonly Func<string, string> CTanh = (u)=> "tanh(" + C(u)+ ")";
        public static readonly Func<string, string> CSech = (u)=> "sech(" + C(u)+ ")";
        public static readonly Func<string, string> CCosech = (u)=> "cosech(" + C(u)+ ")";
        public static readonly Func<string, string> CCoth = (u)=> "coth(" + C(u)+ ")";
        public static readonly Func<string, string> CAsin = (u)=> "asin(" + C(u)+ ")";
        public static readonly Func<string, string> CAcos = (u)=> "acos(" + C(u)+ ")";
        public static readonly Func<string, string> CAtan = (u)=> "atan(" + C(u)+ ")";
        public static readonly Func<string, string> CAsec = (u)=> "asec(" + C(u)+ ")";
        public static readonly Func<string, string> CAcosec = (u)=> "acosec(" + C(u)+ ")";
        public static readonly Func<string, string> CAcot = (u)=> "acot(" + C(u)+ ")";
        public static readonly Func<string, string> CAsinh = (u)=> "asinh(" + C(u)+ ")";
        public static readonly Func<string, string> CAcosh = (u)=> "acosh(" + C(u)+ ")";
        public static readonly Func<string, string> CAtanh = (u)=> "atanh(" + C(u)+ ")";
        public static readonly Func<string, string> CAsech = (u)=> "asech(" + C(u)+ ")";
        public static readonly Func<string, string> CAcosech = (u)=> "acosech(" + C(u)+ ")";
        public static readonly Func<string, string> CAcoth = (u)=> "acoth(" + C(u)+ ")";
        public static readonly Func<string, string> CSqrt = (u)=> "sqrt(" + C(u)+ ")";
        public static readonly Func<string, string> CLog10 = (u)=> "log10(" + C(u)+ ")";
        public static readonly Func<string, string> CLog = (u)=> "log(" + C(u)+ ")";
        public static readonly Func<string, string> CLn = (u)=> CLog(u);
        public static readonly Func<string, string> CSign = (u)=> "sign(" + C(u)+ ")";
        public static readonly Func<string, string> CAbs = (u)=> "abs(" + C(u)+ ")";
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