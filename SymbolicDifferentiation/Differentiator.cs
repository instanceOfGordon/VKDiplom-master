using System;
using System.Collections.Generic;

namespace SymbolicDifferentiation
{
    public class Differentiator
    {
        //public string DiffVar { get; }

        //public Differentiator(string diffVar)
        //{
        //    DiffVar = diffVar;
        //}

        //public Differentiator()
        //{
        //    DiffVar = "x";
        //}
        public static string Differentiate(string input, string diffVar, bool bOptimize = false)
        {
            if (!input.Contains(diffVar)) return "0";
            var strInput = input;
            // remove spaces
            strInput = strInput.Replace(" ","");
            // make all characters lower case
            strInput = strInput.ToLower();
            // Optimize "--"
            strInput = strInput.OptimizeSign();

            var vStack = new List<ExpressionItem>();
            // parse input equation and fill stack with operators and operands
            if (MathStack.FillStack(strInput, vStack) < 0)
                return "Invalid input";

            int nExpression = 0;
            // apply operators to operands
            string strOutput = DifferentiateStack(vStack, diffVar,ref nExpression);

            // loop to fill the stack string from the stack vector
            //foreach (var expressionItem in vStack)
            //{
            //    if (expressionItem.MCOperator!=null)
            //        strStack += expressionItem.MCOperator + "\r\n";
            //    else
            //        strStack += "d(" + expressionItem.MStrInput + ")/d"+diffVar+ "= " + expressionItem.MStrOutput + "\r\n";

            //    // insert tabs in front of the item stack
            //    expressionItem.MStrStack.InsertTabs();
            //    strStack += expressionItem.MStrStack;

            //}

            if (bOptimize)
                // optimize the equation from unneeded elements
                strOutput = strOutput.Optimize();
            // return output differentiation
            return strOutput;
        }

        private static string DifferentiateStack(List<ExpressionItem> vStack, string diffVar,ref int nExpression)
        {
            var pQI = vStack[nExpression++];
            if (pQI.MCOperator != null)
            {
                // get left operand
                var u = vStack[nExpression].LeftOperand;//GetInput();
                // get left operand differentiation
                var du = DifferentiateStack(vStack, diffVar,ref nExpression);
                // get right operand
                var v = vStack[nExpression].RightOperand;//GetInput();
                // get right operand differentiation
                var dv = DifferentiateStack(vStack, diffVar,ref nExpression);

                if (du == "0")  // u is constant
                    switch (pQI.MCOperator)
                    {
                        case '-':   // d(u-v) = -dv
                            pQI.MStrOutput = "(-" + dv + ')';
                            break;
                        case '+':   // d(u+v) = dv
                            pQI.MStrOutput = dv;
                            break;
                        case '*':   // d(u*v) = u*dv
                            pQI.MStrOutput = u + '*' + dv;
                            break;
                        case '/':   // d(u/v) = (-u*dv)/v^2
                            pQI.MStrOutput = "(-" + u + '*' + dv + ")/(" + v + ")^2";
                            break;
                        case '^':   // d(u^v) = dv*u^v*ln(u)
                            pQI.MStrOutput = dv + "*" + u + "^" + v + (u == "e" ? "" : "*ln(" + u + ")");
                            break;
                    }
                else if (dv == "0") // v is constant
                    switch (pQI.MCOperator)
                    {
                        case '-':   // d(u-v) = du
                        case '+':   // d(u+v) = du
                            pQI.MStrOutput = du;
                            break;
                        case '*':   // d(u*v) = du*v
                            pQI.MStrOutput = du + '*' + v;
                            break;
                        case '/':   // d(u/v) = du/v
                            pQI.MStrOutput = '(' + du + ")/" + v;
                            break;
                        case '^':   // d(u^v) = v*u^(v-1)*du
                                    // pQI.MStrOutput.Format("%s*%s^%s*%s", v, u, TrimFloat(atof(v) - 1), du);
                            pQI.MStrOutput = v + "*" + u + "^" + MathStack.TrimFloat(double.Parse(v) - 1) + "*" + du;
                            break;
                    }
                else
                    switch (pQI.MCOperator)
                    {
                        case '-':   // d(u-v) = du-dv
                        case '+':   // d(u+v) = du+dv
                            pQI.MStrOutput = '(' + du + pQI.MCOperator + dv + ')';
                            break;
                        case '*':   // d(u*v) = u*dv+du*v
                            pQI.MStrOutput = '(' + u + '*' + dv + '+' + du + '*' + v + ')';
                            break;
                        case '/':   // d(u/v) = (du*v-u*dv)/v^2
                            pQI.MStrOutput = '(' + du + '*' + v + '-' + u + '*' + dv + ")/(" + v + ")^2";
                            break;
                        case '^':   // d(u^v) = v*u^(v-1)*du+u^v*ln(u)*dv
                            pQI.MStrOutput = '(' + v + '*' + u + "^(" + v + "-1)*" + du + '+' + u + '^' + v + "*ln(" + u + ")*" + dv + ')';
                            break;
                    }
            }
            else
                // get Expression differentiation
                pQI.GetDifferentiation(diffVar);
            // return resultant differentiation
            return pQI.MStrOutput;
        }
    }

    static class SupportedDifferentiationFunctions
    {
        //public static readonly Func<string, string> Acos = x => "log(" + x + "+sqrt(" + x + "*" + x + "-1))";

        //public static readonly Func<string, string> Acosech =
        //    x => "log((sign(" + x + ")*sqrt(" + x + "*" + x + "+1)+1)/" + x + ")";

        //public static readonly Func<string, double> Atof = x =>
        //{
        //    switch (x)
        //    {
        //        case "E":
        //            return Math.E;
        //        case "PI":
        //            return Math.PI;
        //    }
        //    return Atof(x);
        //};
        //public static readonly Func<string, string> Acoth = x => "log((" + x + "+1)/(" + x + "-1))/2";
        //public static readonly Func<string, string> Asech = x => "log((sqrt(-" + x + "*" + x + "+1)+1)/" + x + ")";
        //public static readonly Func<string, string> Asinh = x => "log(" + x + "+sqrt(" + x + "*" + x + "+1))";
        //public static readonly Func<string, string> Acosh = x => "log(" + x + "+sqrt(" + x + "*" + x + "-1))";
        //public static readonly Func<string, string> Atanh = x => "log((1+" + x + ")/(1-" + x + "))/2";
        // supported functions Dalculation
        public static readonly Func<string, string, string> D = (u, diffVar) => Differentiator.Differentiate(u, diffVar);
        public static readonly Func<string, string, string> DSin = (u, diffVar) => D(u, diffVar) + "*cos(" + u + ")";
        public static readonly Func<string, string, string> DCos = (u, diffVar) => D(u, diffVar) + "*-sin(" + u + ")";
        public static readonly Func<string, string, string> DTan = (u, diffVar) => D(u, diffVar) + "*sec(" + u + ")^2";
        public static readonly Func<string, string, string> DSec = (u, diffVar) => D(u, diffVar) + "*sec(" + u + ")*tan(" + u + ")";
        public static readonly Func<string, string, string> DCosec = (u, diffVar) => "(-" + D(u, diffVar) + ")*cosec(" + u + ")*cot(" + u + ")";
        public static readonly Func<string, string, string> DCot = (u, diffVar) => D(u, diffVar) + "*-cosec(" + u + ")^2";
        public static readonly Func<string, string, string> DSinh = (u, diffVar) => D(u, diffVar) + "*cosh(" + u + ")";
        public static readonly Func<string, string, string> DCosh = (u, diffVar) => D(u, diffVar) + "*sinh(" + u + ")";
        public static readonly Func<string, string, string> DTanh = (u, diffVar) => D(u, diffVar) + "*sech(" + u + ")^2";
        public static readonly Func<string, string, string> DSech = (u, diffVar) => D(u, diffVar) + "*sech(" + u + ")*tanh(" + u + ")";
        public static readonly Func<string, string, string> DCosech = (u, diffVar) => "(-" + D(u, diffVar) + ")*cosech(" + u + ")*coth(" + u + ")";
        public static readonly Func<string, string, string> DCoth = (u, diffVar) => D(u, diffVar) + "*-cosech(" + u + ")^2";
        public static readonly Func<string, string, string> DAsin = (u, diffVar) => D(u, diffVar) + "/sqrt(1-(" + u + ")^2)";

        public static readonly Func<string, string, string> DAcos =
            (u, diffVar) => "(-" + D(u, diffVar) + ")/sqrt(1-(" + u + ")^2)";
        public static readonly Func<string, string, string> DAtan = (u, diffVar) => D(u, diffVar) + "/(1+(" + u + ")^2)";
        public static readonly Func<string, string, string> DAsec = (u, diffVar) => D(u, diffVar) + "/(abs(" + u + ")*sqrt((" + u + ")^2-1))";
        public static readonly Func<string, string, string> DAcosec = (u, diffVar) => "(-" + D(u, diffVar) + ")/(abs(" + u + ")*sqrt((" + u + ")^2-1))";
        public static readonly Func<string, string, string> DAcot = (u, diffVar) => "(-" + D(u, diffVar) + ")/(1+(" + u + ")^2)";
        public static readonly Func<string, string, string> DAsinh = (u, diffVar) => D(u, diffVar) + "/sqrt((" + u + ")^2+1)";
        public static readonly Func<string, string, string> DAcosh = (u, diffVar) => D(u, diffVar) + ")/sqrt((" + u + ")^2-1)";
        public static readonly Func<string, string, string> DAtanh = (u, diffVar) => D(u, diffVar) + "/(1-(" + u + ")^2)";
        public static readonly Func<string, string, string> DAsech = (u, diffVar) => "(-" + D(u, diffVar) + ")/((" + u + ")*sqrt(1-(" + u + ")^2))";
        public static readonly Func<string, string, string> DAcosech = (u, diffVar) => "(-" + D(u, diffVar) + ")/((" + u + ")*sqrt(1+(" + u + ")^2))";
        public static readonly Func<string, string, string> DAcoth = (u, diffVar) => D(u, diffVar) + "/(1-(" + u + ")^2)";
        public static readonly Func<string, string, string> DSqrt = (u, diffVar) => D(u, diffVar) + "/(2*sqrt(" + u + "))";
        public static readonly Func<string, string, string> DLog10 = (u, diffVar) => D(u, diffVar) + "/((" + u + ")*log(10))";
        public static readonly Func<string, string, string> DLog = (u, diffVar) => D(u, diffVar) + "/(" + u + ")";
        public static readonly Func<string, string, string> DLn = (u, diffVar) => DLog(u, diffVar);
        public static readonly Func<string, string, string> DSign = (u, diffVar) => "0";
        public static readonly Func<string, string, string> DAbs = (u, diffVar) => D(u, diffVar) + "*(" + u + ")/abs(" + u + ")";
        //public delegate string MathFunction(string x);

        //public MathFunction Atof = x => (((x) == "e") ? Math.E : ((x) == "pi" ? Math.PI : Atof(x)));

        //public static readonly Func<string, string> Sign = x => double.Parse(x) >= 0 ? 1.ToString() : (-1).ToString();
    }
}
