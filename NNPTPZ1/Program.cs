using System;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using NNPTPZ1.Mathematics;

namespace NNPTPZ1
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                var options = FractalOptions.Parse(args);
                var polynomial = new Polynomial();
                polynomial.Add(new ComplexNumber { Real = 1 });
                polynomial.Add(ComplexNumber.Zero);
                polynomial.Add(ComplexNumber.Zero);
                polynomial.Add(new ComplexNumber { Real = 1 });

                Console.WriteLine(polynomial);
                Console.WriteLine(polynomial.Derive());
                using (var bitmap = new NewtonFractalRenderer().Render(options, polynomial))
                {
                    bitmap.Save(options.OutputPath, ImageFormat.Png);
                }

                Console.WriteLine("Saved: " + Path.GetFullPath(options.OutputPath));

                return 0;
            }
            catch (Exception exception) when (
                exception is ArgumentException || exception is IOException ||
                exception is UnauthorizedAccessException || exception is ExternalException)
            {
                Console.Error.WriteLine(exception.Message);
                return 1;
            }
        }
    }
}
