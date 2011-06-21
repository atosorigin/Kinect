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
using AForge.Math.Random;

namespace Accord.Math
{
    /// <summary>
    /// Static class Matrix. Defines a set of extension methods
    /// that operates mainly on multidimensional arrays and vectors.
    /// </summary>
    public static partial class Matrix
    {
        #region Generic matrices

        /// <summary>
        ///   Returns a matrix with all elements set to a given value.
        /// </summary>
        public static T[,] Create<T>(int rows, int cols, T value)
        {
            if (rows < 0) throw new ArgumentException("rows");
            if (cols < 0) throw new ArgumentException("cols");

            var matrix = new T[rows,cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = value;

            return matrix;
        }

        /// <summary>
        ///   Returns a matrix with all elements set to a given value.
        /// </summary>
        public static T[,] Create<T>(int size, T value)
        {
            if (size < 0) throw new ArgumentException("size");

            return Create(size, size, value);
        }

        #endregion

        #region Diagonal matrices

        /// <summary>
        ///   Returns a square diagonal matrix of the given size.
        /// </summary>
        public static T[,] Diagonal<T>(int size, T value)
        {
            if (size < 0) throw new ArgumentException("size");

            var matrix = new T[size,size];

            for (int i = 0; i < size; i++)
                matrix[i, i] = value;

            return matrix;
        }

        /// <summary>
        ///   Returns a matrix of the given size with value on its diagonal.
        /// </summary>
        public static T[,] Diagonal<T>(int rows, int cols, T value)
        {
            if (rows < 0) throw new ArgumentException("rows");
            if (cols < 0) throw new ArgumentException("cols");

            var matrix = new T[rows,cols];

            int min = System.Math.Min(rows, cols);

            for (int i = 0; i < min; i++)
                matrix[i, i] = value;

            return matrix;
        }

        /// <summary>
        ///   Return a square matrix with a vector of values on its diagonal.
        /// </summary>
        public static T[,] Diagonal<T>(T[] values)
        {
            if (values == null) throw new ArgumentNullException("values");

            var matrix = new T[values.Length,values.Length];

            for (int i = 0; i < values.Length; i++)
                matrix[i, i] = values[i];

            return matrix;
        }

        /// <summary>
        ///   Return a square matrix with a vector of values on its diagonal.
        /// </summary>
        public static T[,] Diagonal<T>(int size, T[] values)
        {
            if (size < 0) throw new ArgumentException("size");

            return Diagonal(size, size, values);
        }

        /// <summary>
        ///   Returns a matrix with a vector of values on its diagonal.
        /// </summary>
        public static T[,] Diagonal<T>(int rows, int cols, T[] values)
        {
            if (values == null) throw new ArgumentNullException("values");
            if (rows < 0) throw new ArgumentException("rows");
            if (cols < 0) throw new ArgumentException("cols");

            var matrix = new T[rows,cols];

            for (int i = 0; i < values.Length; i++)
                matrix[i, i] = values[i];

            return matrix;
        }

        #endregion

        #region Special matrices

        /// <summary>
        ///   Returns the Identity matrix of the given size.
        /// </summary>
        public static double[,] Identity(int size)
        {
            return Diagonal(size, 1.0);
        }

        /// <summary>
        ///   Creates a magic square matrix.
        /// </summary>
        public static double[,] Magic(int size)
        {
            if (size < 3)
                throw new ArgumentException("The square size must be greater or equal to 3.", "size");

            var matrix = new double[size,size];


            // First algorithm: Odd order
            if ((size%2) == 1)
            {
                int a = (size + 1)/2;
                int b = (size + 1);

                for (int j = 0; j < size; j++)
                    for (int i = 0; i < size; i++)
                        matrix[i, j] = size*((i + j + a)%size) + ((i + 2*j + b)%size) + 1;
            }

                // Second algorithm: Even order (double)
            else if ((size%4) == 0)
            {
                for (int j = 0; j < size; j++)
                    for (int i = 0; i < size; i++)
                        if (((i + 1)/2)%2 == ((j + 1)/2)%2)
                            matrix[i, j] = size*size - size*i - j;
                        else
                            matrix[i, j] = size*i + j + 1;
            }

                // Third algorithm: Even order (single)
            else
            {
                int n = size/2;
                int p = (size - 2)/4;
                double t;

                double[,] block = Magic(n);

                for (int j = 0; j < n; j++)
                {
                    for (int i = 0; i < n; i++)
                    {
                        double e = block[i, j];
                        matrix[i, j] = e;
                        matrix[i, j + n] = e + 2*n*n;
                        matrix[i + n, j] = e + 3*n*n;
                        matrix[i + n, j + n] = e + n*n;
                    }
                }

                for (int i = 0; i < n; i++)
                {
                    // Swap M[i,j] and M[i+n,j]
                    for (int j = 0; j < p; j++)
                    {
                        t = matrix[i, j];
                        matrix[i, j] = matrix[i + n, j];
                        matrix[i + n, j] = t;
                    }
                    for (int j = size - p + 1; j < size; j++)
                    {
                        t = matrix[i, j];
                        matrix[i, j] = matrix[i + n, j];
                        matrix[i + n, j] = t;
                    }
                }

                // Continue swaping in the boundary
                t = matrix[p, 0];
                matrix[p, 0] = matrix[p + n, 0];
                matrix[p + n, 0] = t;

                t = matrix[p, p];
                matrix[p, p] = matrix[p + n, p];
                matrix[p + n, p] = t;
            }

            return matrix; // return the magic square.
        }

        /// <summary>
        ///   Creates a centering matrix of size <c>N x N</c> in the
        ///   form <c>(I - 1N)</c> where <c>1N</c> is a matrix with 
        ///   all elements equal to <c>1 / N</c>.
        /// </summary>
        public static double[,] Centering(int size)
        {
            if (size < 0) throw new ArgumentException("size");

            double[,] C = Create(size, -1.0/size);

            for (int i = 0; i < size; i++)
                C[i, i] = 1.0 - 1.0/size;

            return C;
        }

        #endregion

        #region Random matrices

        /// <summary>
        ///   Creates a rows-by-cols matrix with uniformly distributed random data.
        /// </summary>
        public static double[,] Random(int rows, int cols)
        {
            if (rows < 0) throw new ArgumentException("rows");
            if (cols < 0) throw new ArgumentException("cols");

            return Random(rows, cols, 0, 1);
        }

        /// <summary>
        ///   Creates a rows-by-cols matrix with uniformly distributed random data.
        /// </summary>
        public static double[,] Random(int size, bool symmetric, double minValue, double maxValue)
        {
            if (size < 0) throw new ArgumentException("size");

            var matrix = new double[size,size];

            if (!symmetric)
            {
                for (int i = 0; i < size; i++)
                    for (int j = 0; j < size; j++)
                        matrix[i, j] = Tools.Random.NextDouble()*(maxValue - minValue) + minValue;
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = i; j < size; j++)
                    {
                        matrix[i, j] = Tools.Random.NextDouble()*(maxValue - minValue) + minValue;
                        matrix[j, i] = matrix[i, j];
                    }
                }
            }

            return matrix;
        }

        /// <summary>
        ///   Creates a rows-by-cols matrix with uniformly distributed random data.
        /// </summary>
        public static double[,] Random(int rows, int cols, double minValue, double maxValue)
        {
            if (rows < 0) throw new ArgumentException("rows");
            if (cols < 0) throw new ArgumentException("cols");

            var matrix = new double[rows,cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = Tools.Random.NextDouble()*(maxValue - minValue) + minValue;

            return matrix;
        }

        /// <summary>
        ///   Creates a rows-by-cols matrix random data drawn from a given distribution.
        /// </summary>
        public static double[,] Random(int rows, int cols, IRandomNumberGenerator generator)
        {
            if (generator == null) throw new ArgumentNullException("generator");
            if (rows < 0) throw new ArgumentException("rows");
            if (cols < 0) throw new ArgumentException("cols");

            var matrix = new double[rows,cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = generator.Next();

            return matrix;
        }

        /// <summary>
        ///   Creates a vector with uniformly distributed random data.
        /// </summary>
        public static double[] Random(int size, double minValue, double maxValue)
        {
            if (size < 0) throw new ArgumentException("size");

            var vector = new double[size];
            for (int i = 0; i < size; i++)
                vector[i] = Tools.Random.NextDouble()*(maxValue - minValue) + minValue;

            return vector;
        }

        /// <summary>
        ///   Creates a vector with random data drawn from a given distribution.
        /// </summary>
        public static double[] Random(int size, IRandomNumberGenerator generator)
        {
            if (generator == null) throw new ArgumentNullException("generator");
            if (size < 0) throw new ArgumentException("size");

            var vector = new double[size];
            for (int i = 0; i < size; i++)
                vector[i] = generator.Next();

            return vector;
        }

        #endregion

        #region Vector creation

        /// <summary>
        ///   Creates a matrix with a single row vector.
        /// </summary>
        public static T[,] RowVector<T>(params T[] values)
        {
            if (values == null) throw new ArgumentNullException("values");

            var matrix = new T[1,values.Length];

            for (int i = 0; i < values.Length; i++)
                matrix[0, i] = values[i];

            return matrix;
        }

        /// <summary>
        ///   Creates a matrix with a single column vector.
        /// </summary>
        public static T[,] ColumnVector<T>(params T[] values)
        {
            if (values == null) throw new ArgumentNullException("values");

            var matrix = new T[values.Length,1];

            for (int i = 0; i < values.Length; i++)
                matrix[i, 0] = values[i];

            return matrix;
        }

        /// <summary>
        ///   Creates a vector with the given dimension and starting values.
        /// </summary>
        public static T[] Vector<T>(int n, T[] values)
        {
            var vector = new T[n];

            if (values != null)
            {
                for (int i = 0; i < values.Length; i++)
                    vector[i] = values[i];
            }

            return vector;
        }

        /// <summary>
        ///   Creates a vector with the given dimension and starting values.
        /// </summary>
        public static T[] Vector<T>(int n, T value)
        {
            var vector = new T[n];

            for (int i = 0; i < n; i++)
                vector[i] = value;

            return vector;
        }

        #endregion

        #region Special vectors

        /// <summary>
        ///   Creates a index vector.
        /// </summary>
        public static int[] Indexes(int from, int to)
        {
            var vector = new int[to - from];
            for (int i = 0; i < vector.Length; i++)
                vector[i] = from++;
            return vector;
        }

        /// <summary>
        ///   Creates an interval vector.
        /// </summary>
        public static int[] Interval(int from, int to)
        {
            var vector = new int[to - from + 1];
            for (int i = 0; i < vector.Length; i++)
                vector[i] = from++;
            return vector;
        }

        /// <summary>
        ///   Creates an interval vector.
        /// </summary>
        public static double[] Interval(double from, double to, double stepSize)
        {
            double range = to - from;
            int steps = (int) System.Math.Ceiling(range/stepSize) + 1;

            var r = new double[steps];
            for (int i = 0; i < r.Length; i++)
                r[i] = from + i*stepSize;

            return r;
        }

        /// <summary>
        ///   Creates an interval vector.
        /// </summary>
        public static double[] Interval(double from, double to, int steps)
        {
            double range = to - from;
            double stepSize = range/steps;

            if (steps == Int32.MaxValue)
                throw new ArgumentOutOfRangeException("steps",
                                                      "input must be lesser than Int32.MaxValue");


            var r = new double[steps + 1];
            for (int i = 0; i < r.Length; i++)
                r[i] = i*stepSize;

            return r;
        }

        #endregion

        #region Combine

        /// <summary>
        ///   Combines two vectors horizontally.
        /// </summary>
        public static T[] Combine<T>(this T[] a, T[] b)
        {
            var r = new T[a.Length + b.Length];
            for (int i = 0; i < a.Length; i++)
                r[i] = a[i];
            for (int i = 0; i < b.Length; i++)
                r[i + a.Length] = b[i];

            return r;
        }

        /// <summary>
        ///   Combines a vector and a element horizontally.
        /// </summary>
        public static T[] Combine<T>(this T[] vector, T element)
        {
            var r = new T[vector.Length + 1];
            for (int i = 0; i < vector.Length; i++)
                r[i] = vector[i];

            r[vector.Length] = element;

            return r;
        }

        /// <summary>
        ///   Combine vectors horizontally.
        /// </summary>
        public static T[] Combine<T>(params T[][] vectors)
        {
            int size = 0;
            for (int i = 0; i < vectors.Length; i++)
                size += vectors[i].Length;

            var r = new T[size];

            int c = 0;
            for (int i = 0; i < vectors.Length; i++)
                for (int j = 0; j < vectors[i].Length; j++)
                    r[c++] = vectors[i][j];

            return r;
        }

        /// <summary>
        ///   Combines matrices vertically.
        /// </summary>
        public static T[,] Combine<T>(params T[][,] matrices)
        {
            // TODO: Rename to "Stack<T>" or similar

            int rows = 0;
            int cols = 0;

            for (int i = 0; i < matrices.Length; i++)
            {
                rows += matrices[i].GetLength(0);
                if (matrices[i].GetLength(1) > cols)
                    cols = matrices[i].GetLength(1);
            }

            var r = new T[rows,cols];

            int c = 0;
            for (int i = 0; i < matrices.Length; i++)
            {
                for (int j = 0; j < matrices[i].GetLength(0); j++)
                {
                    for (int k = 0; k < matrices[i].GetLength(1); k++)
                        r[c, k] = matrices[i][j, k];
                    c++;
                }
            }

            return r;
        }

        /// <summary>
        ///   Combines matrices vertically.
        /// </summary>
        public static T[][] Combine<T>(params T[][][] matrices)
        {
            // TODO: Rename to "Stack<T>" or similar

            int rows = 0;
            int cols = 0;

            for (int i = 0; i < matrices.Length; i++)
            {
                rows += matrices[i].Length;
                if (matrices[i][0].Length > cols)
                    cols = matrices[i][0].Length;
            }

            var r = new T[rows][];
            for (int i = 0; i < rows; i++)
                r[i] = new T[cols];

            int c = 0;
            for (int i = 0; i < matrices.Length; i++)
            {
                for (int j = 0; j < matrices[i].Length; j++)
                {
                    for (int k = 0; k < matrices[i][0].Length; k++)
                        r[c][k] = matrices[i][j][k];
                    c++;
                }
            }

            return r;
        }

        #endregion

        #region Expand

        /// <summary>
        ///   Expands a data vector given in summary form.
        /// </summary>
        /// <param name="vector">A base vector.</param>
        /// <param name="count">An array containing by how much each line should be replicated.</param>
        /// <returns></returns>
        public static T[] Expand<T>(T[] vector, int[] count)
        {
            var expansion = new List<T>();
            for (int i = 0; i < count.Length; i++)
                for (int j = 0; j < count[i]; j++)
                    expansion.Add(vector[i]);

            return expansion.ToArray();
        }

        /// <summary>
        ///   Expands a data matrix given in summary form.
        /// </summary>
        /// <param name="matrix">A base matrix.</param>
        /// <param name="count">An array containing by how much each line should be replicated.</param>
        /// <returns></returns>
        public static T[,] Expand<T>(T[,] matrix, int[] count)
        {
            var expansion = new List<T[]>();
            for (int i = 0; i < count.Length; i++)
                for (int j = 0; j < count[i]; j++)
                    expansion.Add(matrix.GetRow(i));

            return expansion.ToArray().ToMatrix();
        }

        #endregion

        #region Split

        /// <summary>
        ///   Splits a given vector into a smaller vectors of the given size.
        /// </summary>
        /// <param name="vector">The vector to be splitted.</param>
        /// <param name="size">The size of the resulting vectors.</param>
        /// <returns>An array of vectors containing the subdivisions of the given vector.</returns>
        public static T[][] Split<T>(this T[] vector, int size)
        {
            int n = vector.Length/size;
            var r = new T[n][];
            for (int i = 0; i < n; i++)
            {
                T[] ri = r[i] = new T[size];
                for (int j = 0; j < size; j++)
                    ri[j] = vector[j*n + i];
            }
            return r;
        }

        #endregion
    }
}