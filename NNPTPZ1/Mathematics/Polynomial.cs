using System;
using System.Collections.Generic;
using System.Text;

namespace NNPTPZ1.Mathematics
{
    public class Polynomial
    {
        private List<ComplexNumber> coefficients = new List<ComplexNumber>();

        /// <summary>Coefficients in ascending order of exponent (constant first).</summary>
        public List<ComplexNumber> Coefficients
        {
            get => coefficients;
            set => coefficients = value ?? throw new ArgumentNullException(nameof(value));
        }

        public void Add(ComplexNumber coefficient)
        {
            if (coefficient == null) throw new ArgumentNullException(nameof(coefficient));
            coefficients.Add(coefficient);
        }

        public Polynomial Derive()
        {
            var derivative = new Polynomial();
            for (int exponent = 1; exponent < coefficients.Count; exponent++)
            {
                derivative.Add(coefficients[exponent].Multiply(new ComplexNumber { Real = exponent }));
            }

            return derivative;
        }

        public ComplexNumber Eval(ComplexNumber x)
        {
            if (x == null) throw new ArgumentNullException(nameof(x));

            // Horner's method evaluates the polynomial in linear time.
            var result = ComplexNumber.Zero;
            for (int exponent = coefficients.Count - 1; exponent >= 0; exponent--)
            {
                result = result.Multiply(x).Add(coefficients[exponent]);
            }

            return result;
        }

        public override string ToString()
        {
            var text = new StringBuilder();
            for (int exponent = 0; exponent < coefficients.Count; exponent++)
            {
                if (exponent > 0) text.Append(" + ");
                text.Append(coefficients[exponent]);
                text.Append('x', exponent);
            }

            return text.ToString();
        }
    }
}
