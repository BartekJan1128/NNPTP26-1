using Microsoft.VisualStudio.TestTools.UnitTesting;
using NNPTPZ1.Mathematics;

namespace NNPTPZ1Tests
{
    [TestClass]
    public class ComplexNumberTests
    {
        [TestMethod]
        public void AdditionPreservesOperandsAndSupportsZero()
        {
            var left = new ComplexNumber { Real = 10, Imaginary = 20 };
            var right = new ComplexNumber { Real = 1, Imaginary = 2 };

            Assert.AreEqual(new ComplexNumber { Real = 11, Imaginary = 22 }, left.Add(right));
            Assert.AreEqual(new ComplexNumber { Real = 10, Imaginary = 20 }, left);
            Assert.AreEqual(new ComplexNumber { Real = 1, Imaginary = 2 }, right);

            var value = new ComplexNumber { Real = 1, Imaginary = -1 };
            Assert.AreEqual(value, value.Add(ComplexNumber.Zero));
            Assert.AreEqual("(1 + -1i)", value.ToString());
        }

        [TestMethod]
        public void PolynomialEvaluatesRealArgumentsAndFormatsTerms()
        {
            var polynomial = new Polynomial();
            polynomial.Coefficients.Add(new ComplexNumber { Real = 1 });
            polynomial.Coefficients.Add(ComplexNumber.Zero);
            polynomial.Coefficients.Add(new ComplexNumber { Real = 1 });

            Assert.AreEqual(new ComplexNumber { Real = 1 }, polynomial.Eval(ComplexNumber.Zero));
            Assert.AreEqual(new ComplexNumber { Real = 2 }, polynomial.Eval(new ComplexNumber { Real = 1 }));
            Assert.AreEqual(new ComplexNumber { Real = 5 }, polynomial.Eval(new ComplexNumber { Real = 2 }));
            Assert.AreEqual("(1 + 0i) + (0 + 0i)x + (1 + 0i)xx", polynomial.ToString());
        }
    }
}
