using System;
using System.Globalization;
using System.IO;

namespace NNPTPZ1
{
    internal sealed class FractalOptions
    {
        private const string Usage = "Usage: NNPTPZ1 <width> <height> <xmin> <xmax> <ymin> <ymax> <output.png>. Use a decimal point. Run without arguments to use defaults.";

        public int Width { get; private set; }
        public int Height { get; private set; }
        public double XMin { get; private set; }
        public double XMax { get; private set; }
        public double YMin { get; private set; }
        public double YMax { get; private set; }
        public string OutputPath { get; private set; }

        private FractalOptions() { }

        public static FractalOptions Parse(string[] args)
        {
            if (args != null && args.Length == 0)
            {
                return new FractalOptions
                {
                    Width = 800,
                    Height = 600,
                    XMin = -2,
                    XMax = 2,
                    YMin = -1.5,
                    YMax = 1.5,
                    OutputPath = GetDefaultOutputPath()
                };
            }

            if (args == null || args.Length != 7) throw new ArgumentException(Usage);

            var options = new FractalOptions
            {
                Width = ParseDimension(args[0]),
                Height = ParseDimension(args[1]),
                XMin = ParseCoordinate(args[2]),
                XMax = ParseCoordinate(args[3]),
                YMin = ParseCoordinate(args[4]),
                YMax = ParseCoordinate(args[5]),
                OutputPath = args[6]
            };

            if (options.XMin >= options.XMax || options.YMin >= options.YMax ||
                string.IsNullOrWhiteSpace(options.OutputPath))
            {
                throw new ArgumentException("Coordinate ranges must be increasing; an output path is required. " + Usage);
            }

            return options;
        }

        private static string GetDefaultOutputPath()
        {
            // Find the solution root independently of Visual Studio's working directory.
            var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (directory != null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "NNPTPZ1.sln")))
                    return Path.Combine(directory.FullName, "output.png");

                directory = directory.Parent;
            }

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.png");
        }

        private static int ParseDimension(string value)
        {
            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int dimension) || dimension <= 0)
            {
                throw new ArgumentException("Image dimensions must be positive integers. " + Usage);
            }

            return dimension;
        }

        private static double ParseCoordinate(string value)
        {
            if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double coordinate) ||
                double.IsNaN(coordinate) || double.IsInfinity(coordinate) || Math.Abs(coordinate) > float.MaxValue)
            {
                throw new ArgumentException("Coordinates must be finite and within the supported numeric range. " + Usage);
            }

            return coordinate;
        }
    }
}
