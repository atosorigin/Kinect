// Accord Math Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//

using System;
using System.Collections.Generic;
using AForge;

namespace Accord.Math
{
    /// <summary>
    ///   Set of mathematical tools.
    /// </summary>
    public static class Tools
    {
        #region Framework-wide random number generator

        private static Random random = new Random();

        /// <summary>
        ///   Gets a reference to the random number generator used
        ///   internally by the Accord.NET classes and methods.
        /// </summary>
        public static Random Random
        {
            get { return random; }
        }

        /// <summary>
        ///   Sets a random seed for the internal number generator.
        /// </summary>
        public static void SetupGenerator(int seed)
        {
            random = new Random(seed);
        }

        #endregion

        /// <summary>
        ///   Returns the next power of 2 after the input value x.
        /// </summary>
        /// <param name="x">Input value x.</param>
        /// <returns>Returns the next power of 2 after the input value x.</returns>
        public static int NextPowerOf2(int x)
        {
            --x;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return ++x;
        }

        /// <summary>
        ///   Returns the previous power of 2 after the input value x.
        /// </summary>
        /// <param name="x">Input value x.</param>
        /// <returns>Returns the previous power of 2 after the input value x.</returns>
        public static int PreviousPowerOf2(int x)
        {
            return NextPowerOf2(x + 1)/2;
        }


        /// <summary>
        ///   Hypotenuse calculus without overflow/underflow
        /// </summary>
        /// <param name="a">first value</param>
        /// <param name="b">second value</param>
        /// <returns>The hypotenuse Sqrt(a^2 + b^2)</returns>
        public static double Hypotenuse(double a, double b)
        {
            double r = 0.0;
            double absA = System.Math.Abs(a);
            double absB = System.Math.Abs(b);

            if (absA > absB)
            {
                r = b/a;
                r = absA*System.Math.Sqrt(1 + r*r);
            }
            else if (b != 0)
            {
                r = a/b;
                r = absB*System.Math.Sqrt(1 + r*r);
            }

            return r;
        }

        /// <summary>
        ///   Gets the proper modulus operation for
        ///   a integer x and modulo m.
        /// </summary>
        public static int Mod(int x, int m)
        {
            if (m < 0) m = -m;
            int r = x%m;
            return r < 0 ? r + m : r;
        }


        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        public static int Scale(this IntRange from, IntRange to, int x)
        {
            if (from.Length == 0) return 0;
            return (to.Length)*(x - from.Min)/from.Length + to.Min;
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        public static double Scale(this DoubleRange from, DoubleRange to, double x)
        {
            if (from.Length == 0) return 0;
            return (to.Length)*(x - from.Min)/from.Length + to.Min;
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        public static double Scale(double fromMin, double fromMax, double toMin, double toMax, double x)
        {
            if (fromMax - fromMin == 0) return 0;
            return (toMax - toMin)*(x - fromMin)/(fromMax - fromMin) + toMin;
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        public static double[] Scale(double fromMin, double fromMax, double toMin, double toMax, double[] x)
        {
            var result = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
                result[i] = (toMax - toMin)*(x[i] - fromMin)/(fromMax - fromMin) + toMin;

            return result;
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        public static float[] Scale(float fromMin, float fromMax, float toMin, float toMax, float[] x)
        {
            var result = new float[x.Length];
            for (int i = 0; i < x.Length; i++)
                result[i] = (toMax - toMin)*(x[i] - fromMin)/(fromMax - fromMin) + toMin;

            return result;
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        public static double[] Scale(double toMin, double toMax, double[] x)
        {
            return Scale(x.Min(), x.Max(), toMin, toMax, x);
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        public static double[][] Scale(double[] fromMin, double[] fromMax, double[] toMin, double[] toMax, double[][] x)
        {
            int rows = x.Length;
            int cols = fromMin.Length;

            var result = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                result[i] = new double[cols];
                for (int j = 0; j < cols; j++)
                {
                    result[i][j] = (toMax[j] - toMin[j])*(x[i][j] - fromMin[j])/(fromMax[j] - fromMin[j]) + toMin[j];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        public static double[][] Scale(double[] fromMin, double[] fromMax, double toMin, double toMax, double[][] x)
        {
            int rows = x.Length;
            int cols = fromMin.Length;

            var result = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                result[i] = new double[cols];
                for (int j = 0; j < cols; j++)
                {
                    result[i][j] = (toMax - toMin)*(x[i][j] - fromMin[j])/(fromMax[j] - fromMin[j]) + toMin;
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        public static double[][] Scale(double[] toMin, double[] toMax, double[][] x)
        {
            double[] min = x.Min();
            double[] max = x.Max();
            return Scale(min, max, toMin, toMax, x);
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        public static double[][] Scale(double toMin, double toMax, double[][] x)
        {
            return Scale(x.Min(), x.Max(), toMin, toMax, x);
        }

        /// <summary>
        ///   Converts the value x (which is measured in the scale
        ///   'from') to another value measured in the scale 'to'.
        /// </summary>
        public static float Scale(float fromMin, float fromMax, float toMin, float toMax, float x)
        {
            if (fromMax - fromMin == 0) return 0;
            return (toMax - toMin)*(x - fromMin)/(fromMax - fromMin) + toMin;
        }


        /// <summary>
        ///   Returns the hyperbolic arc cosine of the specified value.
        /// </summary>
        public static double Acosh(double x)
        {
            if (x < 1.0)
                throw new ArgumentOutOfRangeException("x");
            return System.Math.Log(x + System.Math.Sqrt(x*x - 1));
        }

        /// <summary>
        /// Returns the hyperbolic arc sine of the specified value.
        /// </summary>
        public static double Asinh(double d)
        {
            double x;
            int sign;

            if (d == 0.0)
                return d;

            if (d < 0.0)
            {
                sign = -1;
                x = -d;
            }
            else
            {
                sign = 1;
                x = d;
            }
            return sign*System.Math.Log(x + System.Math.Sqrt(x*x + 1));
        }

        /// <summary>
        /// Returns the hyperbolic arc tangent of the specified value.
        /// </summary>
        public static double Atanh(double d)
        {
            if (d > 1.0 || d < -1.0)
                throw new ArgumentOutOfRangeException("d");
            return 0.5*System.Math.Log((1.0 + d)/(1.0 - d));
        }


        /// <summary>
        ///   Returns the factorial falling power of the specified value.
        /// </summary>
        public static int FactorialPower(int value, int degree)
        {
            int t = value;
            for (int i = 0; i < degree; i++)
                t *= degree--;
            return t;
        }

        /// <summary>
        ///   Truncated power function.
        /// </summary>
        public static double TruncatedPower(double value, double degree)
        {
            double x = System.Math.Pow(value, degree);
            return (x > 0) ? x : 0.0;
        }

        /// <summary>
        ///   Fast inverse floating-point square root.
        /// </summary>
        public static unsafe float InvSqrt(float f)
        {
            float xhalf = 0.5f*f;
            Int32 i = *(Int32*) &f;
            i = 0x5f375a86 - (i >> 1);
            f = *(float*) &i;
            f = f*(1.5f - xhalf*f*f);
            return f;
        }
    }

    /// <summary>
    ///   Directions for the General Comparer.
    /// </summary>
    public enum ComparerDirection
    {
        /// <summary>
        ///   Sorting will be performed in ascending order.
        /// </summary>
        Ascending,
        /// <summary>
        ///   Sorting will be performed in descending order.
        /// </summary>
        Descending
    } ;

    /// <summary>
    ///   General comparer which supports multiple directions
    ///   and comparison of absolute values.
    /// </summary>
    public class GeneralComparer : IComparer<double>
    {
        private readonly bool absolute;
        private readonly int direction = 1;

        /// <summary>
        ///   Constructs a new General Comparer.
        /// </summary>
        /// <param name="direction">The direction to compare.</param>
        public GeneralComparer(ComparerDirection direction)
            : this(direction, false)
        {
        }

        /// <summary>
        ///   Constructs a new General Comparer.
        /// </summary>
        /// <param name="direction">The direction to compare.</param>
        /// <param name="useAbsoluteValues">True to compare absolute values, false otherwise. Default is false.</param>
        public GeneralComparer(ComparerDirection direction, bool useAbsoluteValues)
        {
            this.direction = (direction == ComparerDirection.Ascending) ? 1 : -1;
            absolute = useAbsoluteValues;
        }

        #region IComparer<double> Members

        /// <summary>
        ///   Compares two objects and returns a value indicating whether one is less than,
        ///    equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        public int Compare(double x, double y)
        {
            if (absolute)
                return direction*(System.Math.Abs(x).CompareTo(System.Math.Abs(y)));
            else
                return direction*(x.CompareTo(y));
        }

        #endregion
    }
}