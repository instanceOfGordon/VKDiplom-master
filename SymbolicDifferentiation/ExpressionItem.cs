using System;

namespace SymbolicDifferentiation
{
    public class ExpressionItem
    {
        public static string[] Operators { get; } = { "+-", "*/", "^%" };
        public static string[] AdditionSubtractionOperator { get; } = { "+-" };
        public ExpressionItem(string lpcsInput)
        {
            MStrInput = lpcsInput;
            MCOperator = null;
            MNFunction = -1;
            MNOutput = 0;
            MNSign = 1;
        }

        public string MStrInput { get; set; }
        public char? MCOperator { get; set; }
        public int MNFunction { get; set; }
        public double MNOutput { get; set; }
        public int MNSign { get; set; }
        public string MStrOutput { get; set; }


        public string GetInput()
        {
            if (!MStrInput.IsNumeric() &&
                (MStrInput.Contains("+") || MStrInput.Contains("-") || MStrInput.Contains("/") ||
                 MStrInput.Contains("*") || MStrInput.Contains("^")))
                return '(' + MStrInput + ')';
            return MStrInput;
        }

        //public string LeftOperand
        //{
        //    get
        //    {
        //        if (MStrInput.IsNumeric())
        //            return MStrInput;
        //        var operIdx = MathStack.GetOperator(MStrInput, Operators);
        //        if (operIdx == -1) return MStrInput;
        //        return MStrInput.Substring(0, operIdx);
        //    }
        //}

        //public string RightOperand
        //{
        //    get
        //    {
        //        if (MStrInput.IsNumeric())
        //            return MStrInput;
        //        var operIdx = MathStack.GetOperator(MStrInput, Operators);
        //        if (operIdx == -1) return MStrInput;
        //        return MStrInput.Substring(operIdx + 1);
        //    }
        //}

        public void GetDifferentiation(string diffVar)
        {
            //if (!MStrInput.Contains(diffVar))
            //{
            //    MStrOutput = "0";
            //    return;
            //}
            int nIndex;
            if (MNFunction != -1)
            {
                nIndex = MStrInput.IndexOf('(');
                var str = MStrInput.Substring(nIndex + 1);
                // get the string between function parentheses
                str = str.Substring(0, str.LastIndexOf(')'));
                switch (MNFunction)
                {
                    case 0: MStrOutput = SupportedDifferentiationFunctions.D(str, diffVar); break;
                    case 1: MStrOutput = SupportedDifferentiationFunctions.DSin(str, diffVar); break;
                    case 2: MStrOutput = SupportedDifferentiationFunctions.DCos(str, diffVar); break;
                    case 3: MStrOutput = SupportedDifferentiationFunctions.DTan(str, diffVar); break;
                    case 4: MStrOutput = SupportedDifferentiationFunctions.DSec(str, diffVar); break;
                    case 5: MStrOutput = SupportedDifferentiationFunctions.DCosec(str, diffVar); break;
                    case 6: MStrOutput = SupportedDifferentiationFunctions.DCot(str, diffVar); break;
                    case 7: MStrOutput = SupportedDifferentiationFunctions.DSinh(str, diffVar); break;
                    case 8: MStrOutput = SupportedDifferentiationFunctions.DCosh(str, diffVar); break;
                    case 9: MStrOutput = SupportedDifferentiationFunctions.DTanh(str, diffVar); break;
                    case 10: MStrOutput = SupportedDifferentiationFunctions.DSech(str, diffVar); break;
                    case 11: MStrOutput = SupportedDifferentiationFunctions.DCosech(str, diffVar); break;
                    case 12: MStrOutput = SupportedDifferentiationFunctions.DCoth(str, diffVar); break;
                    case 13: MStrOutput = SupportedDifferentiationFunctions.DAsin(str, diffVar); break;
                    case 14: MStrOutput = SupportedDifferentiationFunctions.DAcos(str, diffVar); break;
                    case 15: MStrOutput = SupportedDifferentiationFunctions.DAtan(str, diffVar); break;
                    case 16: MStrOutput = SupportedDifferentiationFunctions.DAsec(str, diffVar); break;
                    case 17: MStrOutput = SupportedDifferentiationFunctions.DAcosec(str, diffVar); break;
                    case 18: MStrOutput = SupportedDifferentiationFunctions.DAcot(str, diffVar); break;
                    case 19: MStrOutput = SupportedDifferentiationFunctions.DAsinh(str, diffVar); break;
                    case 20: MStrOutput = SupportedDifferentiationFunctions.DAcosh(str, diffVar); break;
                    case 21: MStrOutput = SupportedDifferentiationFunctions.DAtanh(str, diffVar); break;
                    case 22: MStrOutput = SupportedDifferentiationFunctions.DAsech(str, diffVar); break;
                    case 23: MStrOutput = SupportedDifferentiationFunctions.DAcosech(str, diffVar); break;
                    case 24: MStrOutput = SupportedDifferentiationFunctions.DAcoth(str, diffVar); break;
                    case 25: MStrOutput = SupportedDifferentiationFunctions.DSqrt(str, diffVar); break;
                    case 26: MStrOutput = SupportedDifferentiationFunctions.DLog10(str, diffVar); break;
                    case 27: MStrOutput = SupportedDifferentiationFunctions.DLog(str, diffVar); break;
                    case 28: MStrOutput = SupportedDifferentiationFunctions.DLn(str, diffVar); break;
                    case 29: MStrOutput = SupportedDifferentiationFunctions.DSign(str, diffVar); break;
                    case 30: MStrOutput = SupportedDifferentiationFunctions.DAbs(str, diffVar); break;
                }
                MStrOutput = (MNSign == -1 ? "-" : "") + MStrOutput;
            }
            else
            {
                // dx/dx = 1
                if (MStrInput == diffVar || MStrInput == "+" + diffVar)
                    MStrOutput = "1";
                else if (MStrInput == "-" + diffVar)
                    MStrOutput = "-1";
                else if (MStrInput.IsNumeric()|| MStrInput!=diffVar)
                    // dc/dx = 0, where c is constant
                    MStrOutput = "0";
                else
                    // du/dx, where u is a function of x
                    MStrOutput = 'd' + MStrInput + "/d" + diffVar;
            }
        }

        // return error number, 0 on success
        public int GetCalculation(string[] variables, double[] values,bool bValue = false)
        {
            var nIndex = MStrInput.First('(');
            var str = MStrInput.Substring(nIndex + 1);
            // get the string between function parentheses
            str = str.Substring(0, str.Last(')'));

            double u = 0;
            var nError = Calculator.Calculate(str,variables,values, ref u);
            if (nError != 0 && bValue)
                return nError;
            if (nError == 0)
            {
                switch (MNFunction)
                {
                    case 13: // asin
                    case 14: // acos
                    case 16: // asec
                    case 17: // acosec
                        if (u < -1 || u > 1)
                            return (int)ErrorArgument.InvalidArgument;
                        break;
                    case 25: // sqrt
                    case 26: // log10
                    case 27: // log
                    case 28: // ln
                        if (u <= 0)
                            return (int)ErrorArgument.InvalidArgument;
                        break;
                }
                switch (MNFunction)
                {
                    case 0:
                        MNOutput = u;
                        break;
                    case 1:
                        MNOutput = Math.Sin(u);
                        break;
                    case 2:
                        MNOutput = Math.Cos(u);
                        break;
                    case 3:
                        MNOutput = Math.Tan(u);
                        break;
                    case 4:
                        MNOutput = Math.Cos(u);
                        break; // sec
                    case 5:
                        MNOutput = Math.Sin(u);
                        break; // cosec
                    case 6:
                        MNOutput = Math.Tan(u);
                        break; // cot
                    case 7:
                        MNOutput = Math.Sinh(u);
                        break;
                    case 8:
                        MNOutput = Math.Cosh(u);
                        break;
                    case 9:
                        MNOutput = Math.Tanh(u);
                        break;
                    case 10:
                        MNOutput = Math.Cosh(u);
                        break; // sech
                    case 11:
                        MNOutput = Math.Sinh(u);
                        break; // cosech
                    case 12:
                        MNOutput = Math.Tanh(u);
                        break; // coth
                    case 13:
                        MNOutput = Math.Asin(u);
                        break;
                    case 14:
                        MNOutput = Math.Acos(u);
                        break;
                    case 15:
                        MNOutput = Math.Atan(u);
                        break;
                    case 16:
                        MNOutput = Math.Acos(u);
                        break; // asec
                    case 17:
                        MNOutput = Math.Asin(u);
                        break; // acosec
                    case 18:
                        MNOutput = Math.Atan(u);
                        break; // acot
                    case 19:
                        str = SupportedCalculationFunctions.Asinh(str);
                        break;
                    case 20:
                        str = SupportedCalculationFunctions.Acosh(str);
                        break;
                    case 21:
                        str = SupportedCalculationFunctions.Atanh(str);
                        break;
                    case 22:
                        str = SupportedCalculationFunctions.Asech(str);
                        break;
                    case 23:
                        str = SupportedCalculationFunctions.Acosech(str);
                        break;
                    case 24:
                        str = SupportedCalculationFunctions.Acoth(str);
                        break;
                    case 25:
                        MNOutput = Math.Sqrt(u);
                        break;
                    case 26:
                        MNOutput = Math.Log10(u);
                        break;
                    case 27:
                        MNOutput = Math.Log(u);
                        break;
                    case 28:
                        MNOutput = Math.Log(u);
                        break;
                    case 29:
                        MNOutput = Math.Sign(u);
                        break;
                    case 30:
                        MNOutput = Math.Abs(u);
                        break;
                }
                switch (MNFunction)
                {
                    case 4: // sec
                    case 5: // cosec
                    case 6: // cot
                    case 10: // sech
                    case 11: // cosech
                    case 12: // coth
                    case 16: // asec
                    case 17: // acosec
                    case 18: // acot
                        if (Math.Abs(MNOutput) < 0.001)
                            return (int)ErrorArgument.InvalidArgument;
                        MNOutput = 1 / MNOutput;
                        break;
                    case 19: // asinh
                    case 20: // acosh
                    case 21: // atanh
                    case 22: // asech
                    case 23: // acosech
                    case 24: // acoth
                        double result = 0;
                        var err = Calculator.Calculate(str, variables, values, ref result);
                        MNOutput = result;
                        if ((nError = err) < 0)
                            return nError;

                        break;
                }
                MNOutput *= MNSign;
                var mnout = MNOutput;

                MStrOutput = MathStack.TrimFloat(mnout);
            }
            else
            {
                if (MNFunction == -1 && bValue)
                    return (int)ErrorArgument.NotNumeric;
                switch (MNFunction)
                {
                    case 0:
                        MStrOutput = SupportedCalculationFunctions.C(str,variables, values);
                        break;
                    case 1:
                        MStrOutput = SupportedCalculationFunctions.CSin(str,variables, values);
                        break;
                    case 2:
                        MStrOutput = SupportedCalculationFunctions.CCos(str,variables, values);
                        break;
                    case 3:
                        MStrOutput = SupportedCalculationFunctions.CTan(str,variables, values);
                        break;
                    case 4:
                        MStrOutput = SupportedCalculationFunctions.CSec(str,variables, values);
                        break;
                    case 5:
                        MStrOutput = SupportedCalculationFunctions.CCosec(str,variables, values);
                        break;
                    case 6:
                        MStrOutput = SupportedCalculationFunctions.CCot(str,variables, values);
                        break;
                    case 7:
                        MStrOutput = SupportedCalculationFunctions.CSinh(str,variables, values);
                        break;
                    case 8:
                        MStrOutput = SupportedCalculationFunctions.CCosh(str,variables, values);
                        break;
                    case 9:
                        MStrOutput = SupportedCalculationFunctions.CTanh(str,variables, values);
                        break;
                    case 10:
                        MStrOutput = SupportedCalculationFunctions.CSech(str,variables, values);
                        break;
                    case 11:
                        MStrOutput = SupportedCalculationFunctions.CCosech(str,variables, values);
                        break;
                    case 12:
                        MStrOutput = SupportedCalculationFunctions.CCoth(str,variables, values);
                        break;
                    case 13:
                        MStrOutput = SupportedCalculationFunctions.CAsin(str,variables, values);
                        break;
                    case 14:
                        MStrOutput = SupportedCalculationFunctions.CAcos(str,variables, values);
                        break;
                    case 15:
                        MStrOutput = SupportedCalculationFunctions.CAtan(str,variables, values);
                        break;
                    case 16:
                        MStrOutput = SupportedCalculationFunctions.CAsec(str,variables, values);
                        break;
                    case 17:
                        MStrOutput = SupportedCalculationFunctions.CAcosec(str,variables, values);
                        break;
                    case 18:
                        MStrOutput = SupportedCalculationFunctions.CAcot(str,variables, values);
                        break;
                    case 19:
                        MStrOutput = SupportedCalculationFunctions.CAsinh(str,variables, values);
                        break;
                    case 20:
                        MStrOutput = SupportedCalculationFunctions.CAcosh(str,variables, values);
                        break;
                    case 21:
                        MStrOutput = SupportedCalculationFunctions.CAtanh(str,variables, values);
                        break;
                    case 22:
                        MStrOutput = SupportedCalculationFunctions.CAsech(str,variables, values);
                        break;
                    case 23:
                        MStrOutput = SupportedCalculationFunctions.CAcosech(str,variables, values);
                        break;
                    case 24:
                        MStrOutput = SupportedCalculationFunctions.CAcoth(str,variables, values);
                        break;
                    case 25:
                        MStrOutput = SupportedCalculationFunctions.CSqrt(str,variables, values);
                        break;
                    case 26:
                        MStrOutput = SupportedCalculationFunctions.CLog10(str,variables, values);
                        break;
                    case 27:
                        MStrOutput = SupportedCalculationFunctions.CLog(str,variables, values);
                        break;
                    case 28:
                        MStrOutput = SupportedCalculationFunctions.CLn(str,variables, values);
                        break;
                    case 29:
                        MStrOutput = SupportedCalculationFunctions.CSign(str,variables, values);
                        break;
                    case 30:
                        MStrOutput = SupportedCalculationFunctions.CAbs(str,variables, values);
                        break;
                }
                MNOutput = MNSign * SupportedCalculationFunctions.Atof(MStrOutput);
                MStrOutput = (MNSign == -1 ? "-" : "") + MStrOutput;
                if (bValue && !StringExtensions.IsNumeric(MStrOutput))
                {
                    //MStrStack = "";
                    return (int)ErrorArgument.NotNumeric;
                }


                MStrOutput = MStrInput;
                if (bValue && !StringExtensions.IsNumeric(MStrOutput))
                {
                    //MStrStack = "";
                    return (int)ErrorArgument.NotNumeric;
                }
                MNOutput = SupportedCalculationFunctions.Atof(MStrOutput);
            }
            return 0;
        }
    }
}