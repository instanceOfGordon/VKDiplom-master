using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymbolicDifferentiation;

namespace LUTest
{
    [TestClass]
    public class SymbolicDifferentiationTest
    {

        [TestMethod]
        public void DifferentiateXYTest()
        {
            var differentiator = new Differentiator();
            var d = differentiator.Differentiate("x^2+y^2", "x", true);
            var res = differentiator.Differentiate(d, "y", true);

            d = differentiator.Differentiate("x^2+y^2", "y", true);
            res = differentiator.Differentiate(d, "x", true);

            d = differentiator.Differentiate("x^y", "x", true);
            // toto nezvladne t.j y*x^(y-1) podla y
            res = differentiator.Differentiate(d, "y", true);

            d = differentiator.Differentiate("x^y", "y", true);
            res = differentiator.Differentiate(d, "x", true);
        }

        [TestMethod]
        public void DifferentiateMexicanHatTest()
        {
            var differentiator = new Differentiator();
            var dx = differentiator.Differentiate("sin(sqrt(x^2+y^2))", "x", true);
            var res = differentiator.Differentiate(dx, "y", true);

            dx = differentiator.Differentiate("sin(sqrt(x^2+y^2))", "y", true);
            res = differentiator.Differentiate(dx, "x", true);

        }



    }
}
