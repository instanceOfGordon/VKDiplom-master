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
        private static readonly double[] _values = { 2d, 4d };
        [TestMethod]
        public void CalculateXYTest()
        {
            double output = 0;
            var res = Calculator.Calculate("x^2+y^2", _values[0], _values[1], ref output);


            res = Calculator.Calculate("x^y", _values[0],_values[1], ref output);
        }

        [TestMethod]
        public void CalculateMexicanHatTest()
        {
            double output = 0;
            var res = Calculator.Calculate("sin(sqrt(x^2+y^2))", _values[0], _values[1], ref output);


        }
    }
}
