using System;
using System.Collections.Generic;
using System.Drawing;
using NNPTPZ1.Mathematics;

namespace NNPTPZ1
{
    internal sealed class NewtonFractalRenderer
    {
        private const int MaxIterations = 100;
        private const double ConvergenceTolerance = 0.000001;
        private const double RootTolerance = 0.001;
        private const int ShadePerIteration = 2;
        private static readonly Color[] Palette =
        {
            Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Orange,
            Color.Fuchsia, Color.Gold, Color.Cyan
        };

        /// <summary>Creates an image owned by the caller. Unconverged points are black.</summary>
        public Bitmap Render(FractalOptions options, Polynomial polynomial)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (polynomial == null) throw new ArgumentNullException(nameof(polynomial));

            var derivative = polynomial.Derive();
            var roots = new List<ComplexNumber>();
            double xStep = (options.XMax - options.XMin) / options.Width;
            double yStep = (options.YMax - options.YMin) / options.Height;
            var bitmap = new Bitmap(options.Width, options.Height);
            try
            {
                for (int row = 0; row < options.Height; row++)
                {
                    for (int column = 0; column < options.Width; column++)
                    {
                        var point = new ComplexNumber
                        {
                            Real = options.XMin + column * xStep,
                            Imaginary = (float)(options.YMin + row * yStep)
                        };

                        Color color = Color.Black;
                        if (TryFindRoot(polynomial, derivative, point, out ComplexNumber root, out int iterations))
                        {
                            int rootIndex = FindOrAddRoot(roots, root);
                            color = Shade(Palette[rootIndex % Palette.Length], iterations);
                        }

                        bitmap.SetPixel(column, row, color);
                    }
                }

                return bitmap;
            }
            catch
            {
                bitmap.Dispose();
                throw;
            }
        }

        internal static bool TryFindRoot(Polynomial polynomial, Polynomial derivative, ComplexNumber start, out ComplexNumber root, out int iterations)
        {
            root = start;
            for (iterations = 0; iterations <= MaxIterations; iterations++)
            {
                if (!IsFinite(root)) return false;
                ComplexNumber value = polynomial.Eval(root);
                if (!IsFinite(value)) return false;
                if (value.GetAbsoluteValue() <= ConvergenceTolerance) return true;
                if (iterations == MaxIterations) return false;

                ComplexNumber slope = derivative.Eval(root);
                if (!IsFinite(slope) || slope.GetAbsoluteValue() == 0) return false;
                root = root.Subtract(value.Divide(slope));
            }

            return false;
        }

        private static bool IsFinite(ComplexNumber value) =>
            !double.IsNaN(value.Real) && !double.IsInfinity(value.Real) &&
            !float.IsNaN(value.Imaginary) && !float.IsInfinity(value.Imaginary);

        private static int FindOrAddRoot(List<ComplexNumber> roots, ComplexNumber root)
        {
            for (int index = 0; index < roots.Count; index++)
            {
                if (root.Subtract(roots[index]).GetAbsoluteValue() <= RootTolerance) return index;
            }

            roots.Add(root);
            return roots.Count - 1;
        }

        private static Color Shade(Color color, int iterations)
        {
            int shade = iterations * ShadePerIteration;
            return Color.FromArgb(Math.Max(0, color.R - shade), Math.Max(0, color.G - shade), Math.Max(0, color.B - shade));
        }
    }
}
