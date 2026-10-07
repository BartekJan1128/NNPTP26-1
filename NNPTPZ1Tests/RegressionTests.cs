using System;
using System.Drawing;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NNPTPZ1;
using NNPTPZ1.Mathematics;

namespace NNPTPZ1Tests
{
    [TestClass]
    public class RegressionTests
    {
        [TestMethod]
        public void ComplexArithmeticDoesNotModifyOperands()
        {
            var left = new ComplexNumber { Real = 3, Imaginary = 4 };
            var right = new ComplexNumber { Real = 1, Imaginary = -2 };

            Assert.AreEqual(new ComplexNumber { Real = 2, Imaginary = 6 }, left.Subtract(right));
            Assert.AreEqual(new ComplexNumber { Real = 11, Imaginary = -2 }, left.Multiply(right));
            Assert.AreEqual(new ComplexNumber { Real = -1, Imaginary = 2 }, left.Divide(right));
            Assert.AreEqual(5, left.GetAbS(), 1e-10);
            Assert.AreEqual(new ComplexNumber { Real = 3, Imaginary = 4 }, left);
            Assert.AreEqual(new ComplexNumber { Real = 1, Imaginary = -2 }, right);
            Assert.ThrowsException<DivideByZeroException>(() => left.Divide(ComplexNumber.Zero));
            Assert.ThrowsException<ArgumentNullException>(() => left.Add(null));
        }

        [TestMethod]
        public void EqualComplexNumbersHaveEqualHashCodes()
        {
            var first = new ComplexNumber { Real = 3, Imaginary = -4 };
            var second = new ComplexNumber { Real = 3, Imaginary = -4 };
            Assert.AreEqual(first, second);
            Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
            Assert.IsFalse(first.Equals(null));
        }

        [TestMethod]
        public void ZeroCannotBeChangedThroughAnotherInstance()
        {
            var zero = ComplexNumber.Zero;
            zero.Real = 42;
            Assert.AreEqual(0, ComplexNumber.Zero.Real);
        }

        [TestMethod]
        public void PolynomialEvaluationAndDerivativeSupportComplexArguments()
        {
            var polynomial = Cubic();
            var point = new ComplexNumber { Imaginary = 1 };
            Assert.AreEqual(new ComplexNumber { Real = 1, Imaginary = -1 }, polynomial.Eval(point));
            Assert.AreEqual(new ComplexNumber { Real = -3 }, polynomial.Derive().Eval(point));
            Assert.AreEqual(ComplexNumber.Zero, new Polynomial().Eval(point));
            var constant = new Polynomial();
            constant.Add(new ComplexNumber { Real = 5 });
            Assert.AreEqual(ComplexNumber.Zero, constant.Derive().Eval(point));
        }

        [TestMethod]
        public void ArgumentsUseDecimalPointRegardlessOfCurrentCulture()
        {
            CultureInfo original = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("cs-CZ");
                var options = FractalOptions.Parse(new[] { "8", "5", "-1.5", "2", "-2", "2", "out.png" });
                Assert.AreEqual(-1.5, options.XMin);
                Assert.AreEqual(8, options.Width);
                Assert.AreEqual(5, options.Height);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = original;
            }
        }

        [DataTestMethod]
        [DataRow(0, "0")]
        [DataRow(1, "-1")]
        [DataRow(0, "abc")]
        [DataRow(2, "NaN")]
        [DataRow(3, "Infinity")]
        [DataRow(2, "3")]
        [DataRow(4, "2")]
        [DataRow(2, "1,5")]
        [DataRow(6, " ")]
        public void InvalidArgumentsAreRejected(int index, string value)
        {
            var args = new[] { "8", "5", "-2", "2", "-2", "2", "out.png" };
            args[index] = value;
            Assert.ThrowsException<ArgumentException>(() => FractalOptions.Parse(args));
        }

        [TestMethod]
        public void MissingArgumentsAreRejected()
        {
            Assert.ThrowsException<ArgumentException>(() => FractalOptions.Parse(new[] { "800" }));
            Assert.ThrowsException<ArgumentException>(() => FractalOptions.Parse(null));
        }

        [DataTestMethod]
        [DataRow(8, 5)]
        [DataRow(5, 8)]
        [DataRow(1, 1)]
        public void RectangularImagesUseCorrectDimensionsAndStableRootColors(int width, int height)
        {
            var options = FractalOptions.Parse(new[] { width.ToString(), height.ToString(), "1", "2", "1", "2", "out.png" });
            var linear = new Polynomial();
            linear.Add(ComplexNumber.Zero);
            linear.Add(new ComplexNumber { Real = 1 });
            using (var image = new NewtonFractalRenderer().Render(options, linear))
            {
                Assert.AreEqual(width, image.Width);
                Assert.AreEqual(height, image.Height);
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Assert.AreEqual(Color.FromArgb(253, 0, 0).ToArgb(), image.GetPixel(x, y).ToArgb());
                    }
                }
            }
        }

        [TestMethod]
        public void SingularPointIsRenderedAtCorrectPixel()
        {
            var options = FractalOptions.Parse(new[] { "4", "2", "-1", "1", "0", "1", "out.png" });
            using (var image = new NewtonFractalRenderer().Render(options, Cubic()))
            {
                Assert.AreEqual(Color.Black.ToArgb(), image.GetPixel(2, 0).ToArgb());
                Assert.AreEqual(Color.Red.ToArgb(), image.GetPixel(0, 0).ToArgb());
            }
        }

        [TestMethod]
        public void NewtonIterationConvergesToRoot()
        {
            var polynomial = Cubic();
            Assert.IsTrue(NewtonFractalRenderer.TryFindRoot(polynomial, polynomial.Derive(),
                new ComplexNumber { Real = -2 }, out ComplexNumber root, out int iterations));
            Assert.AreEqual(-1, root.Real, 1e-6);
            Assert.IsTrue(iterations > 0 && iterations <= 100);
        }

        [TestMethod]
        public void NewtonIterationStopsAtZeroDerivative()
        {
            var polynomial = Cubic();
            Assert.IsFalse(NewtonFractalRenderer.TryFindRoot(polynomial, polynomial.Derive(),
                ComplexNumber.Zero, out _, out int iterations));
            Assert.AreEqual(0, iterations);
        }

        [TestMethod]
        [Timeout(2000)]
        public void NewtonIterationStopsForCycle()
        {
            // x^3 - 2x + 2 cycles between 0 and 1 starting at zero.
            var polynomial = new Polynomial();
            polynomial.Add(new ComplexNumber { Real = 2 });
            polynomial.Add(new ComplexNumber { Real = -2 });
            polynomial.Add(ComplexNumber.Zero);
            polynomial.Add(new ComplexNumber { Real = 1 });
            Assert.IsFalse(NewtonFractalRenderer.TryFindRoot(polynomial, polynomial.Derive(),
                ComplexNumber.Zero, out _, out int iterations));
            Assert.AreEqual(100, iterations);
        }

        private static Polynomial Cubic()
        {
            var polynomial = new Polynomial();
            polynomial.Add(new ComplexNumber { Real = 1 });
            polynomial.Add(ComplexNumber.Zero);
            polynomial.Add(ComplexNumber.Zero);
            polynomial.Add(new ComplexNumber { Real = 1 });
            return polynomial;
        }
    }
}
