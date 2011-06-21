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
using System.Linq;
using Accord.Math.Decompositions;

namespace Accord.Math
{
    /// <summary>
    /// Static class Matrix. Defines a set of extension methods
    /// that operates mainly on multidimensional arrays and vectors.
    /// </summary>
    public static partial class Matrix
    {
        #region Matrix-Matrix Multiplication

        /// <summary>
        ///   Multiplies two matrices.
        /// </summary>
        /// <param name="a">The left matrix.</param>
        /// <param name="b">The right matrix.</param>
        /// <returns>The product of the multiplication of the given matrices.</returns>
        public static double[,] Multiply(this double[,] a, double[,] b)
        {
            var r = new double[a.GetLength(0),b.GetLength(1)];
            Multiply(a, b, r);
            return r;
        }

        /// <summary>
        ///   Multiplies two matrices.
        /// </summary>
        /// <param name="a">The left matrix.</param>
        /// <param name="b">The right matrix.</param>
        /// <returns>The product of the multiplication of the given matrices.</returns>
        public static double[][] Multiply(this double[][] a, double[][] b)
        {
            int rows = a.Length;
            int cols = b[0].Length;

            var r = new double[rows][];
            for (int i = 0; i < rows; i++)
                r[i] = new double[cols];

            Multiply(a, b, r);
            return r;
        }

        /// <summary>
        ///   Multiplies two matrices.
        /// </summary>
        /// <param name="a">The left matrix.</param>
        /// <param name="b">The right matrix.</param>
        /// <returns>The product of the multiplication of the given matrices.</returns>
        public static float[][] Multiply(this float[][] a, float[][] b)
        {
            int rows = a.Length;
            int cols = b[0].Length;

            var r = new float[rows][];
            for (int i = 0; i < rows; i++)
                r[i] = new float[cols];

            Multiply(a, b, r);
            return r;
        }


        /// <summary>
        ///   Multiplies two matrices.
        /// </summary>
        /// <param name="a">The left matrix.</param>
        /// <param name="b">The right matrix.</param>
        /// <returns>The product of the multiplication of the given matrices.</returns>
        public static float[,] Multiply(this float[,] a, float[,] b)
        {
            var r = new float[a.GetLength(0),b.GetLength(1)];
            Multiply(a, b, r);
            return r;
        }

        /// <summary>
        ///   Multiplies two matrices.
        /// </summary>
        /// <param name="a">The left matrix.</param>
        /// <param name="b">The right matrix.</param>
        /// <param name="result">The matrix to store results.</param>
        public static unsafe void Multiply(this double[,] a, double[,] b, double[,] result)
        {
            // TODO: enable argument checking
            // if (a.GetLength(1) != b.GetLength(0))
            //     throw new ArgumentException("Matrix dimensions must match");


            int n = a.GetLength(1);
            int m = a.GetLength(0);
            int p = b.GetLength(1);

            fixed (double* ptrA = a)
            {
                var Bcolj = new double[n];
                for (int j = 0; j < p; j++)
                {
                    for (int k = 0; k < n; k++)
                        Bcolj[k] = b[k, j];

                    double* Arowi = ptrA;
                    for (int i = 0; i < m; i++)
                    {
                        double s = 0;
                        for (int k = 0; k < n; k++)
                            s += *(Arowi++)*Bcolj[k];
                        result[i, j] = s;
                    }
                }
            }
        }

        /// <summary>
        ///   Multiplies two matrices.
        /// </summary>
        /// <param name="a">The left matrix.</param>
        /// <param name="b">The right matrix.</param>
        /// <param name="result">The matrix to store results.</param>
        public static void Multiply(this double[][] a, double[][] b, double[][] result)
        {
            // TODO: enable argument checking
            // if (a.GetLength(1) != b.GetLength(0))
            //     throw new ArgumentException("Matrix dimensions must match");


            int n = a[0].Length;
            int m = a.Length;
            int p = b[0].Length;

            var Bcolj = new double[n];
            for (int j = 0; j < p; j++)
            {
                for (int k = 0; k < n; k++)
                    Bcolj[k] = b[k][j];

                for (int i = 0; i < m; i++)
                {
                    double[] Arowi = a[i];

                    double s = 0;
                    for (int k = 0; k < n; k++)
                        s += Arowi[k]*Bcolj[k];

                    result[i][j] = s;
                }
            }
        }

        /// <summary>
        ///   Multiplies two matrices.
        /// </summary>
        /// <param name="a">The left matrix.</param>
        /// <param name="b">The right matrix.</param>
        /// <param name="result">The matrix to store results.</param>
        public static void Multiply(this float[][] a, float[][] b, float[][] result)
        {
            // TODO: enable argument checking
            // if (a.GetLength(1) != b.GetLength(0))
            //     throw new ArgumentException("Matrix dimensions must match");


            int n = a[0].Length;
            int m = a.Length;
            int p = b[0].Length;

            var Bcolj = new float[n];
            for (int j = 0; j < p; j++)
            {
                for (int k = 0; k < n; k++)
                    Bcolj[k] = b[k][j];

                for (int i = 0; i < m; i++)
                {
                    float[] Arowi = a[i];

                    float s = 0;
                    for (int k = 0; k < n; k++)
                        s += Arowi[k]*Bcolj[k];

                    result[i][j] = s;
                }
            }
        }

        /// <summary>
        ///   Multiplies two matrices.
        /// </summary>
        /// <param name="a">The left matrix.</param>
        /// <param name="b">The right matrix.</param>
        /// <param name="result">The matrix to store results.</param>
        public static unsafe void Multiply(this float[,] a, float[,] b, float[,] result)
        {
            int acols = a.GetLength(1);
            int arows = a.GetLength(0);
            int bcols = b.GetLength(1);

            fixed (float* ptrA = a)
            {
                var Bcolj = new float[acols];
                for (int j = 0; j < bcols; j++)
                {
                    for (int k = 0; k < acols; k++)
                        Bcolj[k] = b[k, j];

                    float* Arowi = ptrA;
                    for (int i = 0; i < arows; i++)
                    {
                        float s = 0;
                        for (int k = 0; k < acols; k++)
                            s += *(Arowi++)*Bcolj[k];
                        result[i, j] = s;
                    }
                }
            }
        }


        /// <summary>
        ///   Computes A*B', where B' denotes the transpose of B.
        /// </summary>
        public static double[,] MultiplyByTranspose(this double[,] a, double[,] b)
        {
            var r = new double[a.GetLength(0),b.GetLength(0)];
            MultiplyByTranspose(a, b, r);
            return r;
        }

        /// <summary>
        ///   Computes A*B', where B' denotes the transpose of B.
        /// </summary>
        public static unsafe void MultiplyByTranspose(this double[,] a, double[,] b, double[,] result)
        {
            int n = a.GetLength(1);
            int m = a.GetLength(0);
            int p = b.GetLength(0);

            fixed (double* ptrA = a)
            fixed (double* ptrB = b)
            fixed (double* ptrR = result)
            {
                double* rc = ptrR;

                for (int i = 0; i < m; i++)
                {
                    double* bColj = ptrB;
                    for (int j = 0; j < p; j++)
                    {
                        double* aColi = ptrA + n*i;

                        double s = 0;
                        for (int k = 0; k < n; k++)
                            s += *(aColi++)**(bColj++);
                        *(rc++) = s;
                    }
                }
            }
        }


        /// <summary>
        ///   Computes A'*B, where A' denotes the transpose of A.
        /// </summary>
        public static double[,] TransposeAndMultiply(this double[,] a, double[,] b)
        {
            var r = new double[a.GetLength(1),b.GetLength(1)];
            MultiplyByTranspose(a, b, r);
            return r;
        }

        /// <summary>
        ///   Computes A'*B, where A' denotes the transpose of A.
        /// </summary>
        public static void TransposeAndMultiply(this double[,] a, double[,] b, double[,] result)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");
            if (result == null) throw new ArgumentNullException("r");

            // TODO: Check dimensions

            int n = a.GetLength(0);
            int m = a.GetLength(1);
            int p = b.GetLength(1);

            var Bcolj = new double[n];
            for (int i = 0; i < p; i++)
            {
                for (int k = 0; k < n; k++)
                    Bcolj[k] = b[k, i];

                for (int j = 0; j < m; j++)
                {
                    double s = 0;
                    for (int k = 0; k < n; k++)
                        s += a[k, j]*Bcolj[k];

                    result[j, i] = s;
                }
            }
        }


        /// <summary>
        ///   Computes A*B, where B is a diagonal matrix.
        /// </summary>
        public static double[,] MultiplyByDiagonal(this double[,] a, double[] b)
        {
            var r = new double[a.GetLength(0),b.Length];
            MultiplyByDiagonal(a, b, r);
            return r;
        }

        /// <summary>
        ///   Computes A*B, where B is a diagonal matrix.
        /// </summary>
        public static unsafe void MultiplyByDiagonal(this double[,] a, double[] b, double[,] result)
        {
            if (a.GetLength(1) != b.Length)
                throw new ArgumentException("Matrix dimensions must match.");


            int m = a.GetLength(0);

            fixed (double* ptrA = a, ptrR = result)
            {
                double* A = ptrA;
                double* R = ptrR;
                for (int i = 0; i < m; i++)
                    for (int j = 0; j < b.Length; j++)
                        *R++ = *A++*b[j];
            }
        }

        #endregion

        #region Matrix-Vector multiplication

        /// <summary>
        ///   Multiplies a row vector and a matrix.
        /// </summary>
        /// <param name="rowVector">A row vector.</param>
        /// <param name="matrix">A matrix.</param>
        /// <returns>The product of the multiplication of the given row vector and matrix.</returns>
        public static double[] Multiply(this double[] rowVector, double[,] matrix)
        {
            if (matrix.GetLength(0) != rowVector.Length)
                throw new ArgumentException("Matrix dimensions must match", "b");

            var r = new double[matrix.GetLength(1)];

            for (int j = 0; j < matrix.GetLength(1); j++)
                for (int k = 0; k < rowVector.Length; k++)
                    r[j] += rowVector[k]*matrix[k, j];

            return r;
        }

        /// <summary>
        ///   Multiplies a matrix and a vector (a*bT).
        /// </summary>
        /// <param name="matrix">A matrix.</param>
        /// <param name="columnVector">A column vector.</param>
        /// <returns>The product of the multiplication of matrix a and column vector b.</returns>
        public static double[] Multiply(this double[,] matrix, double[] columnVector)
        {
            if (matrix.GetLength(1) != columnVector.Length)
                throw new ArgumentException("Matrix dimensions must match", "b");

            var r = new double[matrix.GetLength(0)];

            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < columnVector.Length; j++)
                    r[i] += matrix[i, j]*columnVector[j];

            return r;
        }

        /// <summary>
        ///   Multiplies a matrix by a scalar.
        /// </summary>
        /// <param name="matrix">A matrix.</param>
        /// <param name="scalar">A scalar.</param>
        /// <returns>The product of the multiplication of the given matrix and scalar.</returns>
        public static double[,] Multiply(this double[,] matrix, double scalar)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            var r = new double[rows,cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = matrix[i, j]*scalar;

            return r;
        }

        /// <summary>
        ///   Multiplies a vector by a scalar.
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <param name="scalar">A scalar.</param>
        /// <returns>The product of the multiplication of the given vector and scalar.</returns>
        public static double[] Multiply(this double[] vector, double scalar)
        {
            var r = new double[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                r[i] = vector[i]*scalar;

            return r;
        }

        /// <summary>
        ///   Multiplies a matrix by a scalar.
        /// </summary>
        /// <param name="scalar">A scalar.</param>
        /// <param name="matrix">A matrix.</param>
        /// <returns>The product of the multiplication of the given vector and scalar.</returns>
        public static double[,] Multiply(this double scalar, double[,] matrix)
        {
            return matrix.Multiply(scalar);
        }

        /// <summary>
        ///   Multiplies a vector by a scalar.
        /// </summary>
        /// <param name="scalar">A scalar.</param>
        /// <param name="vector">A vector.</param>
        /// <returns>The product of the multiplication of the given scalar and vector.</returns>
        public static double[] Multiply(this double scalar, double[] vector)
        {
            return vector.Multiply(scalar);
        }

        #endregion

        #region Division

        /// <summary>
        ///   Divides a vector by a scalar.
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <param name="scalar">A scalar.</param>
        /// <returns>The division quotient of the given vector and scalar.</returns>
        public static double[] Divide(this double[] vector, double scalar)
        {
            var r = new double[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                r[i] = vector[i]/scalar;

            return r;
        }

        /// <summary>
        ///   Divides a vector by a scalar.
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <param name="scalar">A scalar.</param>
        /// <returns>The division quotient of the given vector and scalar.</returns>
        public static float[] Divide(this float[] vector, float scalar)
        {
            var r = new float[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                r[i] = vector[i]/scalar;

            return r;
        }

        /// <summary>
        ///   Elementwise divides a scalar by a vector.
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <param name="scalar">A scalar.</param>
        /// <returns>The division quotient of the given scalar and vector.</returns>
        public static double[] Divide(this double scalar, double[] vector)
        {
            var r = new double[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                r[i] = scalar/vector[i];

            return r;
        }


        /// <summary>
        ///   Divides two matrices by multiplying A by the inverse of B.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix (which will be inverted).</param>
        /// <returns>The result from the division of the given matrices.</returns>
        public static double[,] Divide(this double[,] a, double[,] b)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");

            if (b.GetLength(0) == b.GetLength(1) &&
                a.GetLength(0) == a.GetLength(1))
            {
                // Solve by LU Decomposition if matrix is square.
                return new LuDecomposition(b, true).SolveTranspose(a);
            }
            else
            {
                // Solve by QR Decomposition if not.
                return new QrDecomposition(b, true).SolveTranspose(a);
            }
        }

        /// <summary>
        ///   Divides a matrix by a scalar.
        /// </summary>
        /// <param name="matrix">A matrix.</param>
        /// <param name="scalar">A scalar.</param>
        /// <returns>The division quotient of the given matrix and scalar.</returns>
        public static double[,] Divide(this double[,] matrix, double scalar)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            var r = new double[rows,cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = matrix[i, j]/scalar;

            return r;
        }

        /// <summary>
        ///   Elementwise divides a scalar by a matrix.
        /// </summary>
        /// <param name="scalar">A scalar.</param>
        /// <param name="matrix">A matrix.</param>
        /// <returns>The elementwise division of the given scalar and matrix.</returns>
        public static double[,] Divide(this double scalar, double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            var r = new double[rows,cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = scalar/matrix[i, j];

            return r;
        }

        #endregion

        #region Products

        /// <summary>
        ///   Gets the inner product (scalar product) between two vectors (aT*b).
        /// </summary>
        /// <param name="a">A vector.</param>
        /// <param name="b">A vector.</param>
        /// <returns>The inner product of the multiplication of the vectors.</returns>
        /// <remarks>
        ///    In mathematics, the dot product is an algebraic operation that takes two
        ///    equal-length sequences of numbers (usually coordinate vectors) and returns
        ///    a single number obtained by multiplying corresponding entries and adding up
        ///    those products. The name is derived from the dot that is often used to designate
        ///    this operation; the alternative name scalar product emphasizes the scalar
        ///    (rather than vector) nature of the result.
        ///    
        ///    The principal use of this product is the inner product in a Euclidean vector space:
        ///    when two vectors are expressed on an orthonormal basis, the dot product of their 
        ///    coordinate vectors gives their inner product.
        /// </remarks>
        public static double InnerProduct(this double[] a, double[] b)
        {
            double r = 0.0;

            if (a.Length != b.Length)
                throw new ArgumentException("Vector dimensions must match", "b");

            for (int i = 0; i < a.Length; i++)
                r += a[i]*b[i];

            return r;
        }

        /// <summary>
        ///   Gets the outer product (matrix product) between two vectors (a*bT).
        /// </summary>
        /// <remarks>
        ///   In linear algebra, the outer product typically refers to the tensor
        ///   product of two vectors. The result of applying the outer product to
        ///   a pair of vectors is a matrix. The name contrasts with the inner product,
        ///   which takes as input a pair of vectors and produces a scalar.
        /// </remarks>
        public static double[,] OuterProduct(this double[] a, double[] b)
        {
            var r = new double[a.Length,b.Length];

            for (int i = 0; i < a.Length; i++)
                for (int j = 0; j < b.Length; j++)
                    r[i, j] += a[i]*b[j];

            return r;
        }

        /// <summary>
        ///   Vectorial product.
        /// </summary>
        /// <remarks>
        ///   The cross product, vector product or Gibbs vector product is a binary operation
        ///   on two vectors in three-dimensional space. It has a vector result, a vector which
        ///   is always perpendicular to both of the vectors being multiplied and the plane
        ///   containing them. It has many applications in mathematics, engineering and physics.
        /// </remarks>
        public static double[] VectorProduct(double[] a, double[] b)
        {
            return new[]
                       {
                           a[1]*b[2] - a[2]*b[1],
                           a[2]*b[0] - a[0]*b[2],
                           a[0]*b[1] - a[1]*b[0]
                       };
        }

        /// <summary>
        ///   Vectorial product.
        /// </summary>
        public static float[] VectorProduct(float[] a, float[] b)
        {
            return new[]
                       {
                           a[1]*b[2] - a[2]*b[1],
                           a[2]*b[0] - a[0]*b[2],
                           a[0]*b[1] - a[1]*b[0]
                       };
        }

        /// <summary>
        ///   Computes the cartesian product of many sets.
        /// </summary>
        /// <remarks>
        ///   References:
        ///   - http://blogs.msdn.com/b/ericlippert/archive/2010/06/28/computing-a-cartesian-product-with-linq.aspx 
        /// </remarks>
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> empty = new[] {Enumerable.Empty<T>()};

            return sequences.Aggregate(empty, (accumulator, sequence) =>
                                              from accumulatorSequence in accumulator
                                              from item in sequence
                                              select accumulatorSequence.Concat(new[] {item}));
        }

        /// <summary>
        ///   Computes the cartesian product of many sets.
        /// </summary>
        public static T[][] CartesianProduct<T>(params T[][] sequences)
        {
            IEnumerable<IEnumerable<T>> result = CartesianProduct(sequences as IEnumerable<IEnumerable<T>>);

            var list = new List<T[]>();
            foreach (var point in result)
                list.Add(point.ToArray());

            return list.ToArray();
        }

        /// <summary>
        ///   Computes the cartesian product of two sets.
        /// </summary>
        public static T[][] CartesianProduct<T>(this T[] sequence1, T[] sequence2)
        {
            return CartesianProduct(new[] {sequence1, sequence2});
        }

        #endregion

        #region Addition and Subraction

        /// <summary>
        ///   Adds two matrices.
        /// </summary>
        /// <param name="a">A matrix.</param>
        /// <param name="b">A matrix.</param>
        /// <returns>The sum of the given matrices.</returns>
        public static double[,] Add(this double[,] a, double[,] b)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");

            if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1))
                throw new ArgumentException("Matrix dimensions must match", "b");

            int rows = a.GetLength(0);
            int cols = b.GetLength(1);

            var r = new double[rows,cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = a[i, j] + b[i, j];

            return r;
        }

        /// <summary>
        ///   Adds a vector to a column or row of a matrix.
        /// </summary>
        /// <param name="matrix">A matrix.</param>
        /// <param name="vector">A vector.</param>
        /// <param name="dimension">
        ///   Pass 0 if the vector should be added row-wise, 
        ///   or 1 if the vector should be added column-wise.
        /// </param>
        public static double[,] Add(this double[,] matrix, double[] vector, int dimension)
        {
            if (matrix == null) throw new ArgumentNullException("a");
            if (vector == null) throw new ArgumentNullException("b");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            var r = new double[rows,cols];

            if (dimension == 1)
            {
                if (rows != vector.Length)
                    throw new ArgumentException(
                        "Length of B should equal the number of rows in A", "b");

                for (int j = 0; j < cols; j++)
                    for (int i = 0; i < rows; i++)
                        r[i, j] = matrix[i, j] + vector[i];
            }
            else
            {
                if (cols != vector.Length)
                    throw new ArgumentException(
                        "Length of B should equal the number of cols in A", "b");

                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        r[i, j] = matrix[i, j] + vector[j];
            }

            return r;
        }

        /// <summary>
        ///   Adds a vector to a column or row of a matrix.
        /// </summary>
        /// <param name="matrix">A matrix.</param>
        /// <param name="vector">A vector.</param>
        /// <param name="dimension">The dimension to add the vector to.</param>
        public static double[,] Subtract(this double[,] matrix, double[] vector, int dimension)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            var r = new double[rows,cols];

            if (dimension == 1)
            {
                if (rows != vector.Length)
                    throw new ArgumentException(
                        "Length of B should equal the number of rows in A", "b");

                for (int j = 0; j < cols; j++)
                    for (int i = 0; i < rows; i++)
                        r[i, j] = matrix[i, j] - vector[i];
            }
            else
            {
                if (cols != vector.Length)
                    throw new ArgumentException(
                        "Length of B should equal the number of cols in A", "b");

                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        r[i, j] = matrix[i, j] - vector[j];
            }

            return r;
        }

        /// <summary>
        ///   Adds two vectors.
        /// </summary>
        /// <param name="a">A vector.</param>
        /// <param name="b">A vector.</param>
        /// <returns>The addition of the given vectors.</returns>
        public static double[] Add(this double[] a, double[] b)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");

            if (a.Length != b.Length)
                throw new ArgumentException("Vector lengths must match", "b");

            var r = new double[a.Length];

            for (int i = 0; i < a.Length; i++)
                r[i] = a[i] + b[i];

            return r;
        }

        /// <summary>
        ///   Subtracts two matrices.
        /// </summary>
        /// <param name="a">A matrix.</param>
        /// <param name="b">A matrix.</param>
        /// <returns>The subtraction of the given matrices.</returns>
        public static double[,] Subtract(this double[,] a, double[,] b)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");

            if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1))
                throw new ArgumentException("Matrix dimensions must match", "b");

            int rows = a.GetLength(0);
            int cols = b.GetLength(1);

            var r = new double[rows,cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = a[i, j] - b[i, j];

            return r;
        }

        /// <summary>
        ///   Subtracts a scalar from each element of a matrix.
        /// </summary>
        public static double[,] Subtract(this double[,] matrix, double scalar)
        {
            if (matrix == null) throw new ArgumentNullException("a");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            var r = new double[rows,cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = matrix[i, j] - scalar;

            return r;
        }

        /// <summary>
        ///   Elementwise subtracts an element of a matrix from a scalar.
        /// </summary>
        /// <param name="scalar">A scalar.</param>
        /// <param name="matrix">A matrix.</param>
        /// <returns>The elementwise subtraction of scalar a and matrix b.</returns>
        public static double[,] Subtract(this double scalar, double[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            var r = new double[rows,cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = scalar - matrix[i, j];

            return r;
        }

        /// <summary>
        ///   Subtracts two vectors.
        /// </summary>
        /// <param name="a">A vector.</param>
        /// <param name="b">A vector.</param>
        /// <returns>The subtraction of vector b from vector a.</returns>
        public static double[] Subtract(this double[] a, double[] b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vector length must match", "b");

            var r = new double[a.Length];

            for (int i = 0; i < a.Length; i++)
                r[i] = a[i] - b[i];

            return r;
        }

        /// <summary>
        ///   Subtracts a scalar from a vector.
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <param name="scalar">A scalar.</param>
        /// <returns>The subtraction of given scalar from all elements in the given vector.</returns>
        public static double[] Subtract(this double[] vector, double scalar)
        {
            var r = new double[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                r[i] = vector[i] - scalar;

            return r;
        }

        /// <summary>
        ///   Subtracts a scalar from a vector.
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <param name="scalar">A scalar.</param>
        /// <returns>The subtraction of the given vector elements from the given scalar.</returns>
        public static double[] Subtract(this double scalar, double[] vector)
        {
            var r = new double[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                r[i] = vector[i] - scalar;

            return r;
        }

        #endregion

        /// <summary>
        ///   Multiplies a matrix by itself <c>n</c> times.
        /// </summary>
        public static double[,] Power(double[,] matrix, int n)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            if (!matrix.IsSquare())
                throw new ArgumentException("Matrix must be square", "matrix");

            // TODO: This is a very naive implementation and should be optimized.
            // http://en.wikipedia.org/wiki/Cayley-Hamilton_theorem

            double[,] r = matrix;
            for (int i = 0; i < n; i++)
                r = r.Multiply(matrix);

            return r;
        }
    }
}