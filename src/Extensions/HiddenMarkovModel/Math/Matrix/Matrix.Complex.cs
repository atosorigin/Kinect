// Accord Math Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//

using AForge;
using AForge.Math;

namespace Accord.Math
{
    /// <summary>
    ///  Static class ComplexMatrix. Defines a set of extension methods
    ///  that operates mainly on multidimensional arrays and vectors of
    ///  AForge.NET's <seealso cref="Complex"/> data type.
    /// </summary>
    /// 
    public static class ComplexMatrix
    {
        /// <summary>
        ///   Computes the absolute value of an array of complex numbers.
        /// </summary>
        public static Complex[] Abs(this Complex[] x)
        {
            var r = new Complex[x.Length];
            for (int i = 0; i < x.Length; i++)
                r[i] = new Complex(x[i].Magnitude, 0);
            return r;
        }

        /// <summary>
        ///   Computes the sum of an array of complex numbers.
        /// </summary>
        public static Complex Sum(this Complex[] x)
        {
            Complex r = Complex.Zero;
            for (int i = 0; i < x.Length; i++)
                r += x[i];
            return r;
        }

        /// <summary>
        ///   Elementwise multiplication of two complex vectors.
        /// </summary>
        public static Complex[] Multiply(this Complex[] a, Complex[] b)
        {
            var r = new Complex[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                r[i] = Complex.Multiply(a[i], b[i]);
            }
            return r;
        }

        /// <summary>
        ///   Gets the magnitude of every complex number in an array.
        /// </summary>
        public static double[] Magnitude(this Complex[] c)
        {
            var magnitudes = new double[c.Length];
            for (int i = 0; i < c.Length; i++)
                magnitudes[i] = c[i].Magnitude;

            return magnitudes;
        }

        /// <summary>
        ///   Gets the phase of every complex number in an array.
        /// </summary>
        public static double[] Phase(this Complex[] c)
        {
            var phases = new double[c.Length];
            for (int i = 0; i < c.Length; i++)
                phases[i] = c[i].Phase;

            return phases;
        }

        /// <summary>
        ///   Returns the real vector part of the complex vector c.
        /// </summary>
        /// <param name="c">A vector of complex numbers.</param>
        /// <returns>A vector of scalars with the real part of the complex numers.</returns>
        public static double[] Re(this Complex[] c)
        {
            var re = new double[c.Length];
            for (int i = 0; i < c.Length; i++)
                re[i] = c[i].Re;

            return re;
        }

        /// <summary>
        ///   Returns the imaginary vector part of the complex vector c.
        /// </summary>
        /// <param name="c">A vector of complex numbers.</param>
        /// <returns>A vector of scalars with the imaginary part of the complex numers.</returns>
        public static double[] Im(this Complex[] c)
        {
            var im = new double[c.Length];
            for (int i = 0; i < c.Length; i++)
                im[i] = c[i].Im;

            return im;
        }

        /// <summary>
        ///   Converts a complex number to a matrix of scalar values
        ///   in which the first column contains the real values and 
        ///   the second column contains the imaginary values.
        /// </summary>
        /// <param name="c">An array of complex numbers.</param>
        public static double[,] ToArray(this Complex[] c)
        {
            var arr = new double[c.Length,2];
            for (int i = 0; i < c.GetLength(0); i++)
            {
                arr[i, 0] = c[i].Re;
                arr[i, 1] = c[i].Im;
            }

            return arr;
        }

        /// <summary>
        ///   Gets the range of the magnitude values in a complex number vector.
        /// </summary>
        /// <param name="array">A complex number vector.</param>
        /// <returns>The range of magnitude values in the complex vector.</returns>
        public static DoubleRange Range(this Complex[] array)
        {
            double min = array[0].SquaredMagnitude;
            double max = array[0].SquaredMagnitude;

            for (int i = 1; i < array.Length; i++)
            {
                double sqMagnitude = array[i].SquaredMagnitude;
                if (min > sqMagnitude)
                    min = sqMagnitude;
                if (max < sqMagnitude)
                    max = sqMagnitude;
            }

            return new DoubleRange(
                System.Math.Sqrt(min),
                System.Math.Sqrt(max));
        }
    }
}