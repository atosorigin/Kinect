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
        /// <summary>
        ///   Returns the LHS solution matrix if the matrix is square or the least squares solution otherwise.
        /// </summary>
        /// <remarks>
        ///   Please note that this does not check if the matrix is non-singular before attempting to solve.
        /// </remarks>
        public static double[,] Solve(this double[,] matrix, double[,] rightSide)
        {
            if (matrix.GetLength(0) == matrix.GetLength(1))
            {
                // Solve by LU Decomposition if matrix is square.
                return new LuDecomposition(matrix).Solve(rightSide);
            }
            else
            {
                // Solve by QR Decomposition if not.
                return new QrDecomposition(matrix).Solve(rightSide);
            }
        }

        /// <summary>
        ///   Returns the LHS solution vector if the matrix is square or the least squares solution otherwise.
        /// </summary>
        /// <remarks>
        ///   Please note that this does not check if the matrix is non-singular before attempting to solve.
        /// </remarks>
        public static double[] Solve(this double[,] matrix, double[] rightSide)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            if (rightSide == null)
                throw new ArgumentNullException("rightSide");


            if (matrix.GetLength(0) == matrix.GetLength(1))
            {
                // Solve by LU Decomposition if matrix is square.
                return new LuDecomposition(matrix).Solve(rightSide);
            }
            else
            {
                // Solve by QR Decomposition if not.
                return new QrDecomposition(matrix).Solve(rightSide);
            }
        }

        /// <summary>
        ///   Computes the inverse of a matrix.
        /// </summary>
        public static double[,] Inverse(this double[,] matrix)
        {
            if (matrix.GetLength(0) != matrix.GetLength(1))
                throw new ArgumentException("Matrix must be square", "matrix");

            return new LuDecomposition(matrix).Inverse();
        }

        /// <summary>
        ///   Computes the pseudo-inverse of a matrix.
        /// </summary>
        public static double[,] PseudoInverse(this double[,] matrix)
        {
            return new SingularValueDecomposition(matrix, true, true, true).Inverse();
        }
    }
}