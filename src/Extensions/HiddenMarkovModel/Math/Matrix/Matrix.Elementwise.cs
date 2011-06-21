// Accord Math Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//

using System;

namespace Accord.Math
{
    /// <summary>
    /// Static class Matrix. Defines a set of extension methods
    /// that operates mainly on multidimensional arrays and vectors.
    /// </summary>
    public static partial class Matrix
    {
        /// <summary>
        ///   Elementwise absolute value.
        /// </summary>
        public static int[] Abs(this int[] value)
        {
            var r = new int[value.Length];
            for (int i = 0; i < value.Length; i++)
                r[i] = System.Math.Abs(value[i]);
            return r;
        }

        /// <summary>
        ///   Elementwise absolute value.
        /// </summary>
        public static double[] Abs(this double[] value)
        {
            var r = new double[value.Length];
            for (int i = 0; i < value.Length; i++)
                r[i] = System.Math.Abs(value[i]);
            return r;
        }

        /// <summary>
        ///   Elementwise absolute value.
        /// </summary>
        public static double[,] Abs(this double[,] value)
        {
            int rows = value.GetLength(0);
            int cols = value.GetLength(1);

            var r = new double[rows,cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = System.Math.Abs(value[i, j]);
            return r;
        }

        /// <summary>
        ///   Elementwise absolute value.
        /// </summary>
        public static int[,] Abs(this int[,] value)
        {
            int rows = value.GetLength(0);
            int cols = value.GetLength(1);

            var r = new int[rows,cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = System.Math.Abs(value[i, j]);
            return r;
        }


        /// <summary>
        ///   Elementwise Square root.
        /// </summary>
        public static double[] Sqrt(this double[] value)
        {
            var r = new double[value.Length];
            for (int i = 0; i < value.Length; i++)
                r[i] = System.Math.Sqrt(value[i]);
            return r;
        }

        /// <summary>
        ///   Elementwise Square root.
        /// </summary>
        public static double[,] Sqrt(this double[,] value)
        {
            int rows = value.GetLength(0);
            int cols = value.GetLength(1);

            var r = new double[rows,cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = System.Math.Sqrt(value[i, j]);
            return r;
        }


        /// <summary>
        ///   Elementwise power operation.
        /// </summary>
        /// <param name="x">A matrix.</param>
        /// <param name="y">A power.</param>
        /// <returns>Returns x elevated to the power of y.</returns>
        public static double[,] ElementwisePower(this double[,] x, double y)
        {
            var r = new double[x.GetLength(0),x.GetLength(1)];

            for (int i = 0; i < x.GetLength(0); i++)
                for (int j = 0; j < x.GetLength(1); j++)
                    r[i, j] = System.Math.Pow(x[i, j], y);

            return r;
        }

        /// <summary>
        ///   Elementwise power operation.
        /// </summary>
        /// <param name="x">A matrix.</param>
        /// <param name="y">A power.</param>
        /// <returns>Returns x elevated to the power of y.</returns>
        public static double[] ElementwisePower(this double[] x, double y)
        {
            var r = new double[x.Length];

            for (int i = 0; i < r.Length; i++)
                r[i] = System.Math.Pow(x[i], y);

            return r;
        }


        /// <summary>
        ///   Elementwise divide operation.
        /// </summary>
        public static double[] ElementwiseDivide(this double[] a, double[] b)
        {
            var r = new double[a.Length];

            for (int i = 0; i < a.Length; i++)
                r[i] = a[i]/b[i];

            return r;
        }

        /// <summary>
        ///   Elementwise divide operation.
        /// </summary>
        public static double[,] ElementwiseDivide(this double[,] a, double[,] b)
        {
            int rows = a.GetLength(0);
            int cols = b.GetLength(1);

            var r = new double[rows,cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = a[i, j]/b[i, j];

            return r;
        }

        /// <summary>
        ///   Elementwise division.
        /// </summary>
        public static double[,] ElementwiseDivide(this double[,] a, double[] b)
        {
            return ElementwiseDivide(a, b, 0);
        }

        /// <summary>
        ///   Elementwise division.
        /// </summary>
        public static double[,] ElementwiseDivide(this double[,] a, double[] b, int dimension)
        {
            int rows = a.GetLength(0);
            int cols = a.GetLength(1);

            var r = new double[rows,cols];

            if (dimension == 1)
            {
                if (cols != b.Length)
                    throw new ArgumentException(
                        "Length of B should equal the number of columns in A", "b");

                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        r[i, j] = a[i, j]/b[j];
            }
            else
            {
                if (rows != b.Length)
                    throw new ArgumentException(
                        "Length of B should equal the number of rows in A", "b");

                for (int j = 0; j < cols; j++)
                    for (int i = 0; i < rows; i++)
                        r[i, j] = a[i, j]/b[i];
            }
            return r;
        }


        /// <summary>
        ///   Elementwise multiply operation.
        /// </summary>
        public static double[] ElementwiseMultiply(this double[] a, double[] b)
        {
            var r = new double[a.Length];

            for (int i = 0; i < a.Length; i++)
                r[i] = a[i]*b[i];

            return r;
        }

        /// <summary>
        ///   Elementwise multiply operation.
        /// </summary>
        public static double[,] ElementwiseMultiply(this double[,] a, double[,] b)
        {
            if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1))
                throw new ArgumentException("Matrix dimensions must agree.", "b");

            int rows = a.GetLength(0);
            int cols = a.GetLength(1);

            var r = new double[rows,cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = a[i, j]*b[i, j];

            return r;
        }

        /// <summary>
        ///   Elementwise multiply operation.
        /// </summary>
        public static int[] ElementwiseMultiply(this int[] a, int[] b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vector dimensions must agree.", "b");

            var r = new int[a.Length];

            for (int i = 0; i < a.Length; i++)
                r[i] = a[i]*b[i];

            return r;
        }

        /// <summary>
        ///   Elementwise multiplication.
        /// </summary>
        public static int[,] ElementwiseMultiply(this int[,] a, int[,] b)
        {
            if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1))
                throw new ArgumentException("Matrix dimensions must agree.", "b");

            int rows = a.GetLength(0);
            int cols = a.GetLength(1);

            var r = new int[rows,cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = a[i, j]*b[i, j];

            return r;
        }

        /// <summary>
        ///   Elementwise multiplication.
        /// </summary>
        public static double[,] ElementwiseMultiply(this double[,] a, double[] b, int dimension)
        {
            int rows = a.GetLength(0);
            int cols = a.GetLength(1);

            var r = new double[rows,cols];

            if (dimension == 1)
            {
                if (cols != b.Length)
                    throw new ArgumentException(
                        "Length of B should equal the number of columns in A", "b");

                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        r[i, j] = a[i, j]*b[j];
            }
            else
            {
                if (rows != b.Length)
                    throw new ArgumentException(
                        "Length of B should equal the number of rows in A", "b");

                for (int j = 0; j < cols; j++)
                    for (int i = 0; i < rows; i++)
                        r[i, j] = a[i, j]*b[i];
            }

            return r;
        }
    }
}