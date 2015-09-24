using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymbolicDifferentiation;

namespace Test
{
    [TestClass]
    public class SymbolicDifferentiationTest
    {
        //[TestMethod]
        //public void DifferentiateXTest()
        //{
        //    var res = Differentiator.Differentiate("x^2+y^2", "x", true);
        //    res = Differentiator.Differentiate("x^y", "x", true);
        //    //res = Differentiator.Differentiate("y^x", "x", true);
        //}
        //[TestMethod]
        //public void DifferentiateYTest()
        //{
        //    var res = Differentiator.Differentiate("x^2+y^2", "y", true);
        //    res = Differentiator.Differentiate("x^y", "y", true);
        //    //res = Differentiator.Differentiate("y^x", "y", true);
        //}
        [TestMethod]
        public void DifferentiateXYTest()
        {
            var d = Differentiator.Differentiate("x^2+y^2", "x", true);
            var res = Differentiator.Differentiate(d, "y", true);

            d = Differentiator.Differentiate("x^y", "x", true);
            // toto nezvladne t.j y*x^(y-1) podla y
            res = Differentiator.Differentiate(d, "y", true);

            d = Differentiator.Differentiate("x^y", "y", true);
            res = Differentiator.Differentiate(d, "x", true);
        }

        [TestMethod]
        public void DifferentiateMexicanHatTest()
        {
            var dx = Differentiator.Differentiate("sin(sqrt(x^2+y^2))", "x", true);
            var res = Differentiator.Differentiate(dx, "y", true);

            dx = Differentiator.Differentiate("sin(sqrt(x^2+y^2))", "y", true);
            res = Differentiator.Differentiate(dx, "x", true);

        }

    }
}
