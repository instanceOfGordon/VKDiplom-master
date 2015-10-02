using System;
using System.CodeDom;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymbolicDifferentiation;

namespace Test
{
    [TestClass]
    public class SymbolicCalculationTest
    {
        //private static readonly string[] _variables = {"x", "y"};
        private static readonly double[] Values = { -3, -2.98023223876953 };
        [TestMethod]
        public void CalculateXYTest()
        {
            double output = 0;
            var res = Calculator.Calculate("x^2+y^2", Values[0], Values[1], ref output);


            res = Calculator.Calculate("x^y", Values[0], Values[1], ref output);
        }

        [TestMethod]
        public void CalculateMexicanHatTest()
        {
            double output = 0;
            var res = Calculator.Calculate("sin(sqrt(x^2+y^2))", Values[0], Values[1], ref output);


        }
    }
}
