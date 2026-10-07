using System;

namespace NNPTPZ1.Mathematics
{
    public class ComplexNumber
    {
        public double Real { get; set; }
        public float Imaginary { get; set; }

        // A fresh instance prevents callers from changing a shared mutable zero.
        public static ComplexNumber Zero => new ComplexNumber();

        public bool Equals(ComplexNumber other) =>
            other != null && Real.Equals(other.Real) && Imaginary.Equals(other.Imaginary);

        public override bool Equals(object obj) => Equals(obj as ComplexNumber);

        public override int GetHashCode()
        {
            unchecked
            {
                return (Real.GetHashCode() * 397) ^ Imaginary.GetHashCode();
            }
        }

        public ComplexNumber Add(ComplexNumber other)
        {
            ValidateOperand(other);
            return new ComplexNumber { Real = Real + other.Real, Imaginary = Imaginary + other.Imaginary };
        }

        public ComplexNumber Subtract(ComplexNumber other)
        {
            ValidateOperand(other);
            return new ComplexNumber { Real = Real - other.Real, Imaginary = Imaginary - other.Imaginary };
        }

        public ComplexNumber Multiply(ComplexNumber other)
        {
            ValidateOperand(other);
            return new ComplexNumber
            {
                Real = Real * other.Real - (double)Imaginary * other.Imaginary,
                Imaginary = (float)(Real * other.Imaginary + Imaginary * other.Real)
            };
        }

        internal ComplexNumber Divide(ComplexNumber other)
        {
            ValidateOperand(other);
            double denominator = other.Real * other.Real + (double)other.Imaginary * other.Imaginary;
            if (denominator == 0)
            {
                throw new DivideByZeroException("Cannot divide by a zero complex number.");
            }

            return new ComplexNumber
            {
                Real = (Real * other.Real + (double)Imaginary * other.Imaginary) / denominator,
                Imaginary = (float)((Imaginary * other.Real - Real * other.Imaginary) / denominator)
            };
        }

        public double GetAbS() => Math.Sqrt(Real * Real + (double)Imaginary * Imaginary);
        public override string ToString() => $"({Real} + {Imaginary}i)";

        private static void ValidateOperand(ComplexNumber other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
        }
    }
}
