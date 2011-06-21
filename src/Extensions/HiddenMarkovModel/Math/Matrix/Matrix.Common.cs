// Accord Math Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//

using System;
using Accord.Math.Decompositions;

namespace Accord.Math
{
    /// <summary>
    /// Static class Matrix. Defines a set of extension methods
    /// that operates mainly on multidimensional arrays and vectors.
    /// </summary>
    public static partial class Matrix
    {
        #region Comparison

        /// <summary>
        ///   Compares two matrices for equality, considering an acceptance threshold.
        /// </summary>
        public static bool IsEqual(this double[,] objA, double[,] objB, double threshold)
        {
            if (objA == null && objB == null) return true;
            if (objA == null) throw new ArgumentNullException("objA");
            if (objB == null) throw new ArgumentNullException("objB");

            for (int i = 0; i < objA.GetLength(0); i++)
            {
                for (int j = 0; j < objB.GetLength(1); j++)
                {
                    double x = objA[i, j], y = objB[i, j];

                    if (System.Math.Abs(x - y) > threshold || (Double.IsNaN(x) ^ Double.IsNaN(y)))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        ///   Compares two matrices for equality, considering an acceptance threshold.
        /// </summary>
        public static bool IsEqual(this float[,] objA, float[,] objB, float threshold)
        {
            if (objA == null && objB == null) return true;
            if (objA == null) throw new ArgumentNullException("objA");
            if (objB == null) throw new ArgumentNullException("objB");

            for (int i = 0; i < objA.GetLength(0); i++)
            {
                for (int j = 0; j < objB.GetLength(1); j++)
                {
                    float x = objA[i, j], y = objB[i, j];

                    if (System.Math.Abs(x - y) > threshold || (Single.IsNaN(x) ^ Single.IsNaN(y)))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        ///   Compares two matrices for equality, considering an acceptance threshold.
        /// </summary>
        public static bool IsEqual(this double[][] objA, double[][] objB, double threshold)
        {
            if (objA == null && objB == null) return true;
            if (objA == null) throw new ArgumentNullException("objA");
            if (objB == null) throw new ArgumentNullException("objB");

            for (int i = 0; i < objA.Length; i++)
            {
                for (int j = 0; j < objA[i].Length; j++)
                {
                    double x = objA[i][j], y = objB[i][j];

                    if (System.Math.Abs(x - y) > threshold || (Double.IsNaN(x) ^ Double.IsNaN(y)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        ///   Compares two matrices for equality, considering an acceptance threshold.
        /// </summary>
        public static bool IsEqual(this float[][] objA, float[][] objB, float threshold)
        {
            if (objA == null && objB == null) return true;
            if (objA == null) throw new ArgumentNullException("objA");
            if (objB == null) throw new ArgumentNullException("objB");

            for (int i = 0; i < objA.Length; i++)
            {
                for (int j = 0; j < objA[i].Length; j++)
                {
                    float x = objA[i][j], y = objB[i][j];

                    if (System.Math.Abs(x - y) > threshold || (Single.IsNaN(x) ^ Single.IsNaN(y)))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        ///   Compares two vectors for equality, considering an acceptance threshold.
        /// </summary>
        public static bool IsEqual(this double[] objA, double[] objB, double threshold)
        {
            if (objA == null && objB == null) return true;
            if (objA == null) throw new ArgumentNullException("objA");
            if (objB == null) throw new ArgumentNullException("objB");

            for (int i = 0; i < objA.Length; i++)
            {
                if (System.Math.Abs(objA[i] - objB[i]) > threshold)
                    return false;
            }
            return true;
        }

        /// <summary>
        ///   Compares two vectors for equality, considering an acceptance threshold.
        /// </summary>
        public static bool IsEqual(this float[] objA, float[] objB, float threshold)
        {
            if (objA == null && objB == null) return true;
            if (objA == null) throw new ArgumentNullException("objA");
            if (objB == null) throw new ArgumentNullException("objB");

            for (int i = 0; i < objA.Length; i++)
            {
                if (System.Math.Abs(objA[i] - objB[i]) > threshold)
                    return false;
            }
            return true;
        }


        /// <summary>
        ///   Compares each member of a vector for equality with a scalar value x.
        /// </summary>
        public static bool IsEqual(this double[] vector, double scalar)
        {
            if (vector == null) throw new ArgumentNullException("vector");
            if (vector.Length == 0) return false;

            for (int i = 0; i < vector.Length; i++)
            {
                if (vector[i] != scalar)
                    return false;
            }
            return true;
        }

        /// <summary>
        ///   Compares each member of a matrix for equality with a scalar value x.
        /// </summary>
        public static bool IsEqual(this double[,] matrix, double scalar)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");
            if (matrix.Length == 0) return false;

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j] != scalar)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        ///   Compares two matrices for equality.
        /// </summary>
        public static bool IsEqual<T>(this T[][] objA, T[][] objB)
        {
            if (objA == null && objB == null) return true;
            if (objA == null) throw new ArgumentNullException("objA");
            if (objB == null) throw new ArgumentNullException("objB");

            if (objA.Length != objB.Length)
                return false;

            for (int i = 0; i < objA.Length; i++)
            {
                if (objA[i] == objB[i])
                    continue;

                if (objA[i] == null || objB[i] == null)
                    return false;

                if (objA[i].Length != objB[i].Length)
                    return false;

                for (int j = 0; j < objA[i].Length; j++)
                {
                    if (!objA[i][j].Equals(objB[i][j]))
                        return false;
                }
            }
            return true;
        }

        /// <summary>Compares two matrices for equality.</summary>
        public static bool IsEqual<T>(this T[,] objA, T[,] objB)
        {
            if (objA == null && objB == null) return true;
            if (objA == null) throw new ArgumentNullException("objA");
            if (objB == null) throw new ArgumentNullException("objB");

            if (objA.GetLength(0) != objB.GetLength(0) ||
                objA.GetLength(1) != objB.GetLength(1))
                return false;

            int rows = objA.GetLength(0);
            int cols = objA.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (!objA[i, j].Equals(objB[i, j]))
                        return false;
                }
            }
            return true;
        }

        /// <summary>Compares two vectors for equality.</summary>
        public static bool IsEqual<T>(this T[] objA, params T[] objB)
        {
            if (objA == null && objB == null) return true;
            if (objA == null) throw new ArgumentNullException("objA");
            if (objB == null) throw new ArgumentNullException("objB");

            if (objA.Length != objB.Length)
                return false;

            for (int i = 0; i < objA.Length; i++)
            {
                if (!objA[i].Equals(objB[i]))
                    return false;
            }
            return true;
        }

        /// <summary>
        ///   This method should not be called. Use Matrix.IsEqual instead.
        /// </summary>
        public new static bool Equals(object value)
        {
            throw new NotSupportedException("Use Matrix.IsEqual instead.");
        }

        #endregion

        #region Transpose

        /// <summary>
        ///   Gets the transpose of a matrix.
        /// </summary>
        /// <param name="matrix">A matrix.</param>
        /// <returns>The transpose of the given matrix.</returns>
        public static T[,] Transpose<T>(this T[,] matrix)
        {
            return Transpose(matrix, false);
        }

        /// <summary>
        ///   Gets the transpose of a matrix.
        /// </summary>
        /// <param name="matrix">A matrix.</param>
        /// <returns>The transpose of the given matrix.</returns>
        public static T[][] Transpose<T>(this T[][] matrix)
        {
            return Transpose(matrix, false);
        }

        /// <summary>
        ///   Gets the transpose of a matrix.
        /// </summary>
        /// <param name="matrix">A matrix.</param>
        /// <param name="inPlace">True to store the transpose over the same input
        ///   <paramref name="matrix"/>, false otherwise. Default is false.</param>
        /// <returns>The transpose of the given matrix.</returns>
        public static T[,] Transpose<T>(this T[,] matrix, bool inPlace)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (inPlace)
            {
                if (rows != cols)
                    throw new ArgumentException("Only square matrices can be transposed in place.");

                for (int i = 0; i < rows; i++)
                {
                    for (int j = i; j < cols; j++)
                    {
                        T element = matrix[j, i];
                        matrix[j, i] = matrix[i, j];
                        matrix[i, j] = element;
                    }
                }

                return matrix;
            }
            else
            {
                var result = new T[cols,rows];

                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        result[j, i] = matrix[i, j];

                return result;
            }
        }

        /// <summary>
        ///   Gets the transpose of a matrix.
        /// </summary>
        /// <param name="matrix">A matrix.</param>
        /// <param name="inPlace">True to store the transpose over the same input
        ///   <paramref name="matrix"/>, false otherwise. Default is false.</param>
        /// <returns>The transpose of the given matrix.</returns>
        public static T[][] Transpose<T>(this T[][] matrix, bool inPlace)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.Length;
            if (rows == 0) return new T[rows][];
            int cols = matrix[0].Length;

            if (inPlace)
            {
                if (rows != cols)
                    throw new ArgumentException("Only square matrices can be transposed in place.");

                for (int i = 0; i < rows; i++)
                {
                    for (int j = i; j < cols; j++)
                    {
                        T element = matrix[j][i];
                        matrix[j][i] = matrix[i][j];
                        matrix[i][j] = element;
                    }
                }

                return matrix;
            }
            else
            {
                var result = new T[cols][];

                for (int j = 0; j < cols; j++)
                {
                    result[j] = new T[rows];

                    for (int i = 0; i < rows; i++)
                        result[j][i] = matrix[i][j];
                }

                return result;
            }
        }

        /// <summary>
        ///   Gets the transpose of a row vector.
        /// </summary>
        /// <param name="rowVector">A row vector.</param>
        /// <returns>The transpose of the given vector.</returns>
        public static T[,] Transpose<T>(this T[] rowVector)
        {
            if (rowVector == null) throw new ArgumentNullException("matrix");

            var trans = new T[rowVector.Length,1];
            for (int i = 0; i < rowVector.Length; i++)
                trans[i, 0] = rowVector[i];

            return trans;
        }

        #endregion

        #region Matrix Characteristics

        /// <summary>
        ///   Returns true if a matrix is square.
        /// </summary>
        public static bool IsSquare<T>(this T[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            return matrix.GetLength(0) == matrix.GetLength(1);
        }

        /// <summary>
        ///   Returns true if a matrix is symmetric.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsSymmetric(this double[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            if (matrix.GetLength(0) == matrix.GetLength(1))
            {
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        if (matrix[i, j] != matrix[j, i])
                            return false;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        ///   Gets the trace of a matrix.
        /// </summary>
        /// <remarks>
        ///   The trace of an n-by-n square matrix A is defined to be the sum of the
        ///   elements on the main diagonal (the diagonal from the upper left to the
        ///   lower right) of A.
        /// </remarks>
        public static double Trace(this double[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            double trace = 0.0;
            for (int i = 0; i < matrix.GetLength(0); i++)
                trace += matrix[i, i];
            return trace;
        }

        /// <summary>
        ///   Gets the diagonal vector from a matrix.
        /// </summary>
        /// <param name="matrix">A matrix.</param>
        /// <returns>The diagonal vector from the given matrix.</returns>
        public static T[] Diagonal<T>(this T[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            var r = new T[matrix.GetLength(0)];
            for (int i = 0; i < r.Length; i++)
                r[i] = matrix[i, i];

            return r;
        }

        /// <summary>
        ///   Gets the determinant of a matrix.
        /// </summary>
        public static double Determinant(this double[,] matrix)
        {
            // Assume the most general case
            return Determinant(matrix, false);
        }

        /// <summary>
        ///   Gets the determinant of a matrix.
        /// </summary>
        public static double Determinant(this double[,] matrix, bool symmetric)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            if (symmetric) // Use faster robust cholesky decomposition
                return new CholeskyDecomposition(matrix, true, true).Determinant;

            return new LuDecomposition(matrix).Determinant;
        }

        /// <summary>
        ///    Gets whether a matrix is positive definite.
        /// </summary>
        public static bool IsPositiveDefinite(this double[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            return new CholeskyDecomposition(matrix).PositiveDefinite;
        }

        #endregion

        #region Summation

        /// <summary>Calculates the matrix Sum vector.</summary>
        /// <param name="matrix">A matrix whose sums will be calculated.</param>
        /// <returns>Returns a vector containing the sums of each variable in the given matrix.</returns>
        public static double[] Sum(this double[,] matrix)
        {
            return Sum(matrix, 0);
        }

        /// <summary>Calculates the matrix Sum vector.</summary>
        /// <param name="matrix">A matrix whose sums will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be calculated.</param>
        /// <returns>Returns a vector containing the sums of each variable in the given matrix.</returns>
        public static double[] Sum(this double[,] matrix, int dimension)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[] sum;

            if (dimension == 0)
            {
                sum = new double[cols];

                for (int j = 0; j < cols; j++)
                {
                    double s = 0.0;
                    for (int i = 0; i < rows; i++)
                        s += matrix[i, j];
                    sum[j] = s;
                }
            }
            else if (dimension == 1)
            {
                sum = new double[rows];

                for (int j = 0; j < rows; j++)
                {
                    double s = 0.0;
                    for (int i = 0; i < cols; i++)
                        s += matrix[j, i];
                    sum[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return sum;
        }

        /// <summary>Calculates the matrix Sum vector.</summary>
        /// <param name="matrix">A matrix whose sums will be calculated.</param>
        /// <returns>Returns a vector containing the sums of each variable in the given matrix.</returns>
        public static int[] Sum(int[,] matrix)
        {
            return Sum(matrix, 0);
        }

        /// <summary>Calculates the matrix Sum vector.</summary>
        /// <param name="matrix">A matrix whose sums will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be calculated.</param>
        /// <returns>Returns a vector containing the sums of each variable in the given matrix.</returns>
        public static int[] Sum(this int[,] matrix, int dimension)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            int[] sum;

            if (dimension == 0)
            {
                sum = new int[cols];
                for (int j = 0; j < cols; j++)
                {
                    int s = 0;
                    for (int i = 0; i < rows; i++)
                        s += matrix[i, j];
                    sum[j] = s;
                }
            }
            else if (dimension == 1)
            {
                sum = new int[rows];
                for (int j = 0; j < rows; j++)
                {
                    int s = 0;
                    for (int i = 0; i < cols; i++)
                        s += matrix[j, i];
                    sum[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return sum;
        }

        /// <summary>
        ///   Gets the sum of all elements in a vector.
        /// </summary>
        public static double Sum(this double[] vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            double sum = 0.0;
            for (int i = 0; i < vector.Length; i++)
                sum += vector[i];
            return sum;
        }

        /// <summary>
        ///   Gets the sum of all elements in a vector.
        /// </summary>
        public static int Sum(this int[] vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            int sum = 0;
            for (int i = 0; i < vector.Length; i++)
                sum += vector[i];
            return sum;
        }

        /// <summary>Calculates a vector cumulative sum.</summary>
        public static double[] CumulativeSum(this double[] vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            if (vector.Length == 0)
                return new double[0];

            var sum = new double[vector.Length];

            sum[0] = vector[0];
            for (int i = 1; i < vector.Length; i++)
                sum[i] += sum[i - 1] + vector[i];
            return sum;
        }

        /// <summary>Calculates the matrix Sum vector.</summary>
        /// <param name="matrix">A matrix whose sums will be calculated.</param>
        /// <param name="dimension">The dimension in which the cumulative sum will be calculated.</param>
        /// <returns>Returns a vector containing the sums of each variable in the given matrix.</returns>
        public static double[][] CumulativeSum(this double[,] matrix, int dimension)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            double[][] sum;

            if (dimension == 1)
            {
                sum = new double[matrix.GetLength(0)][];
                sum[0] = matrix.GetRow(0);

                // for each row
                for (int i = 1; i < matrix.GetLength(0); i++)
                {
                    sum[i] = new double[matrix.GetLength(1)];

                    // for each column
                    for (int j = 0; j < matrix.GetLength(1); j++)
                        sum[i][j] += sum[i - 1][j] + matrix[i, j];
                }
            }
            else if (dimension == 0)
            {
                sum = new double[matrix.GetLength(1)][];
                sum[0] = matrix.GetColumn(0);

                // for each column
                for (int i = 1; i < matrix.GetLength(1); i++)
                {
                    sum[i] = new double[matrix.GetLength(0)];

                    // for each row
                    for (int j = 0; j < matrix.GetLength(0); j++)
                        sum[i][j] += sum[i - 1][j] + matrix[j, i];
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return sum;
        }

        #endregion

        #region Product

        /// <summary>
        ///   Gets the product of all elements in a vector.
        /// </summary>
        public static double Product(this double[] vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            double product = 1.0;
            for (int i = 0; i < vector.Length; i++)
                product *= vector[i];
            return product;
        }

        /// <summary>
        ///   Gets the product of all elements in a vector.
        /// </summary>
        public static int Product(this int[] vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            int product = 1;
            for (int i = 0; i < vector.Length; i++)
                product *= vector[i];
            return product;
        }

        #endregion

        #region Operation Mapping (Apply)

        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        public static void ApplyInPlace<T>(this T[] vector, Func<T, T> func)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            for (int i = 0; i < vector.Length; i++)
                vector[i] = func(vector[i]);
        }

        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        public static void ApplyInPlace<T>(this T[] vector, Func<T, int, T> func)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            for (int i = 0; i < vector.Length; i++)
                vector[i] = func(vector[i], i);
        }

        /// <summary>
        ///   Applies a function to every element of a matrix.
        /// </summary>
        public static void ApplyInPlace<T>(this T[,] matrix, Func<T, T> func)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = func(matrix[i, j]);
        }

        /// <summary>
        ///   Applies a function to every element of a matrix.
        /// </summary>
        public static void ApplyInPlace<T>(this T[,] matrix, Func<T, int, int, T> func)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = func(matrix[i, j], i, j);
        }

        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        public static TResult[] Apply<TData, TResult>(this TData[] vector, Func<TData, TResult> func)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            var r = new TResult[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                r[i] = func(vector[i]);

            return r;
        }

        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        public static TResult[] Apply<TData, TResult>(this TData[] vector, Func<TData, int, TResult> func)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            var r = new TResult[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                r[i] = func(vector[i], i);

            return r;
        }

        /// <summary>
        ///   Applies a function to every element of a matrix.
        /// </summary>
        public static TResult[,] Apply<TData, TResult>(this TData[,] matrix, Func<TData, TResult> func)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            var r = new TResult[rows,cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = func(matrix[i, j]);

            return r;
        }

        /// <summary>
        ///   Applies a function to every element of a matrix.
        /// </summary>
        public static TResult[,] Apply<TData, TResult>(this TData[,] matrix, Func<TData, int, int, TResult> func)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            var r = new TResult[rows,cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = func(matrix[i, j], i, j);

            return r;
        }

        #endregion

        #region Rounding and discretization

        /// <summary>
        ///   Rounds a double-precision floating-point matrix to a specified number of fractional digits.
        /// </summary>
        public static double[,] Round(this double[,] matrix, int decimals)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            var r = new double[matrix.GetLength(0),matrix.GetLength(1)];

            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                    r[i, j] = System.Math.Round(matrix[i, j], decimals);

            return r;
        }

        /// <summary>
        ///   Returns the largest integer less than or equal than to the specified 
        ///   double-precision floating-point number for each element of the matrix.
        /// </summary>
        public static double[,] Floor(this double[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            var r = new double[matrix.GetLength(0),matrix.GetLength(1)];

            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                    r[i, j] = System.Math.Floor(matrix[i, j]);

            return r;
        }

        /// <summary>
        ///   Returns the largest integer greater than or equal than to the specified 
        ///   double-precision floating-point number for each element of the matrix.
        /// </summary>
        public static double[,] Ceiling(this double[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            var r = new double[matrix.GetLength(0),matrix.GetLength(1)];

            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                    r[i, j] = System.Math.Ceiling(matrix[i, j]);

            return r;
        }

        /// <summary>
        ///   Rounds a double-precision floating-point number array to a specified number of fractional digits.
        /// </summary>
        public static double[] Round(double[] vector, int decimals)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            var r = new double[vector.Length];
            for (int i = 0; i < r.Length; i++)
                r[i] = System.Math.Round(vector[i], decimals);
            return r;
        }

        /// <summary>
        ///   Returns the largest integer less than or equal than to the specified 
        ///   double-precision floating-point number for each element of the array.
        /// </summary>
        public static double[] Floor(double[] vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            var r = new double[vector.Length];
            for (int i = 0; i < r.Length; i++)
                r[i] = System.Math.Floor(vector[i]);
            return r;
        }

        /// <summary>
        ///   Returns the largest integer greater than or equal than to the specified 
        ///   double-precision floating-point number for each element of the array.
        /// </summary>
        public static double[] Ceiling(double[] vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            var r = new double[vector.Length];
            for (int i = 0; i < r.Length; i++)
                r[i] = System.Math.Ceiling(vector[i]);
            return r;
        }

        #endregion

        #region Morphological operations

        /// <summary>
        ///   Transforms a vector into a matrix of given dimensions.
        /// </summary>
        public static T[,] Reshape<T>(T[] array, int rows, int cols)
        {
            if (array == null) throw new ArgumentNullException("array");
            if (rows < 0) throw new ArgumentException("rows");
            if (cols < 0) throw new ArgumentException("cols");

            var r = new T[rows,cols];

            for (int j = 0, k = 0; j < cols; j++)
                for (int i = 0; i < rows; i++)
                    r[i, j] = array[k++];

            return r;
        }

        /// <summary>
        ///   Transforms a vector into a single vector.
        /// </summary>
        public static T[] Reshape<T>(T[,] array, int dimension)
        {
            if (array == null) throw new ArgumentNullException("array");
            if (dimension < 0) throw new ArgumentException("dimension");

            int rows = array.GetLength(0);
            int cols = array.GetLength(1);

            var r = new T[rows*cols];

            if (dimension == 1)
            {
                for (int j = 0, k = 0; j < rows; j++)
                    for (int i = 0; i < cols; i++)
                        r[k++] = array[j, i];
            }
            else
            {
                for (int i = 0, k = 0; i < cols; i++)
                    for (int j = 0; j < rows; j++)
                        r[k++] = array[j, i];
            }

            return r;
        }

        #endregion
    }
}