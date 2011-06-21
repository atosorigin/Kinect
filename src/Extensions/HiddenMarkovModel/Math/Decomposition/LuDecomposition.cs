// Accord Math Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Modifications copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//
// Original work copyright © Lutz Roeder, 2000
//  Adapted from Mapack for COM and Jama routines
//

using System;

namespace Accord.Math.Decompositions
{
    /// <summary>
    ///   LU decomposition of a rectangular matrix.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     For an m-by-n matrix <c>A</c> with <c>m >= n</c>, the LU decomposition is an m-by-n
    ///     unit lower triangular matrix <c>L</c>, an n-by-n upper triangular matrix <c>U</c>,
    ///     and a permutation vector <c>piv</c> of length m so that <c>A(piv) = L*U</c>.
    ///     If m &lt; n, then <c>L</c> is m-by-m and <c>U</c> is m-by-n.</para>
    ///   <para>
    ///     The LU decomposition with pivoting always exists, even if the matrix is
    ///     singular, so the constructor will never fail.  The primary use of the
    ///     LU decomposition is in the solution of square systems of simultaneous
    ///     linear equations. This will fail if <see cref="Nonsingular"/> returns
    ///     <see langword="false"/>.
    ///   </para>
    /// </remarks>
    /// 
    public sealed class LuDecomposition : ISolverDecomposition
    {
        private readonly double[,] lu;
        private readonly int pivotSign;
        private readonly int[] pivotVector;

        /// <summary>Construct a new LU decomposition.</summary>	
        /// <param name="value">The matrix A to be decomposed.</param>
        public LuDecomposition(double[,] value)
            : this(value, false)
        {
        }

        /// <summary>Construct a LU decomposition.</summary>	
        /// <param name="value">The matrix A to be decomposed.</param>
        /// <param name="transpose">True if the decomposition should be performed on
        /// the transpose of A rather than A itself, false otherwise. Default is false.</param>
        public LuDecomposition(double[,] value, bool transpose)
            : this(value, transpose, false)
        {
        }

        /// <summary>Construct a LU decomposition.</summary>	
        /// <param name="value">The matrix A to be decomposed.</param>
        /// <param name="transpose">True if the decomposition should be performed on
        /// the transpose of A rather than A itself, false otherwise. Default is false.</param>
        /// <param name="inPlace">True if the decomposition should be performed over the
        /// <paramref name="value"/> matrix rather than on a copy of it. If true, the
        /// matrix will be destroyed during the decomposition. Default is false.</param>
        public LuDecomposition(double[,] value, bool transpose, bool inPlace)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "Matrix cannot be null.");
            }

            if ((!transpose && value.GetLength(0) < value.GetLength(1)) ||
                (transpose && value.GetLength(1) < value.GetLength(0)))
            {
                throw new ArgumentException("Matrix has more columns than rows.", "value");
            }

            if (transpose)
            {
                lu = value.Transpose(inPlace);
            }
            else
            {
                lu = inPlace ? value : (double[,]) value.Clone();
            }

            int rows = lu.GetLength(0);
            int cols = lu.GetLength(1);

            pivotVector = new int[rows];

            for (int i = 0; i < rows; i++)
                pivotVector[i] = i;

            pivotSign = 1;
            var LUcolj = new double[rows];


            unsafe
            {
                // Outer loop.
                for (int j = 0; j < cols; j++)
                {
                    // Make a copy of the j-th column to localize references.
                    for (int i = 0; i < rows; i++)
                        LUcolj[i] = lu[i, j];

                    // Apply previous transformations.
                    for (int i = 0; i < rows; i++)
                    {
                        double s = 0.0;

                        // Most of the time is spent in the following dot product.
                        int kmax = System.Math.Min(i, j);

                        fixed (double* LUrowi = &lu[i, 0])
                        {
                            for (int k = 0; k < kmax; k++)
                                s += LUrowi[k]*LUcolj[k];

                            LUrowi[j] = LUcolj[i] -= s;
                        }
                    }

                    // Find pivot and exchange if necessary.
                    int p = j;
                    for (int i = j + 1; i < rows; i++)
                    {
                        if (System.Math.Abs(LUcolj[i]) > System.Math.Abs(LUcolj[p]))
                            p = i;
                    }

                    if (p != j)
                    {
                        for (int k = 0; k < cols; k++)
                        {
                            double t = lu[p, k];
                            lu[p, k] = lu[j, k];
                            lu[j, k] = t;
                        }

                        int v = pivotVector[p];
                        pivotVector[p] = pivotVector[j];
                        pivotVector[j] = v;

                        pivotSign = -pivotSign;
                    }

                    // Compute multipliers.
                    double lujj = lu[j, j];
                    if (j < rows & lujj != 0.0)
                    {
                        for (int i = j + 1; i < rows; i++)
                            lu[i, j] /= lujj;
                    }
                }
            }
        }

        /// <summary>Returns if the matrix is non-singular.</summary>
        public bool Nonsingular
        {
            get
            {
                for (int j = 0; j < lu.GetLength(1); j++)
                    if (lu[j, j] == 0)
                        return false;
                return true;
            }
        }

        /// <summary>Returns the determinant of the matrix.</summary>
        public double Determinant
        {
            get
            {
                if (lu.GetLength(0) != lu.GetLength(1))
                    throw new InvalidOperationException("Matrix must be square.");

                double determinant = pivotSign;
                for (int j = 0; j < lu.GetLength(1); j++)
                    determinant *= lu[j, j];

                return determinant;
            }
        }

        /// <summary>Returns the lower triangular factor <c>L</c> with <c>A=LU</c>.</summary>
        public double[,] LowerTriangularFactor
        {
            get
            {
                int rows = lu.GetLength(0);
                int columns = lu.GetLength(1);
                var X = new double[rows,columns];

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (i > j)
                            X[i, j] = lu[i, j];
                        else if (i == j)
                            X[i, j] = 1.0;
                        else
                            X[i, j] = 0.0;
                    }
                }

                return X;
            }
        }

        /// <summary>Returns the lower triangular factor <c>L</c> with <c>A=LU</c>.</summary>
        public double[,] UpperTriangularFactor
        {
            get
            {
                int rows = lu.GetLength(0);
                int columns = lu.GetLength(1);
                var X = new double[rows,columns];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (i <= j)
                            X[i, j] = lu[i, j];
                        else
                            X[i, j] = 0.0;
                    }
                }
                return X;
            }
        }

        /// <summary>Returns the pivot permuation vector.</summary>
        public double[] PivotPermutationVector
        {
            get
            {
                int rows = lu.GetLength(0);
                var p = new double[rows];

                for (int i = 0; i < rows; i++)
                    p[i] = pivotVector[i];

                return p;
            }
        }

        #region ISolverDecomposition Members

        /// <summary>Solves a set of equation systems of type <c>A * X = B</c>.</summary>
        /// <param name="value">Right hand side matrix with as many rows as <c>A</c> and any number of columns.</param>
        /// <returns>Matrix <c>X</c> so that <c>L * U * X = B</c>.</returns>
        public double[,] Solve(double[,] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value.GetLength(0) != lu.GetLength(0))
            {
                throw new ArgumentException("Invalid matrix dimensions.", "value");
            }

            if (!Nonsingular)
            {
                throw new InvalidOperationException("Matrix is singular");
            }

            // Copy right hand side with pivoting
            int count = value.GetLength(1);
            double[,] X = value.Submatrix(pivotVector, null);

            int columns = lu.GetLength(1);

            // Solve L*Y = B(piv,:)
            for (int k = 0; k < columns; k++)
                for (int i = k + 1; i < columns; i++)
                    for (int j = 0; j < count; j++)
                        X[i, j] -= X[k, j]*lu[i, k];

            // Solve U*X = Y;
            for (int k = columns - 1; k >= 0; k--)
            {
                for (int j = 0; j < count; j++)
                    X[k, j] /= lu[k, k];

                for (int i = 0; i < k; i++)
                    for (int j = 0; j < count; j++)
                        X[i, j] -= X[k, j]*lu[i, k];
            }

            return X;
        }

        /// <summary>Solves a set of equation systems of type <c>A * X = B</c>.</summary>
        /// <param name="value">Right hand side matrix with as many rows as <c>A</c> and any number of columns.</param>
        /// <returns>Matrix <c>X</c> so that <c>L * U * X = B</c>.</returns>
        public double[] Solve(double[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value.Length != lu.GetLength(0))
            {
                throw new ArgumentException("Invalid matrix dimensions.", "value");
            }

            if (!Nonsingular)
            {
                throw new InvalidOperationException("Matrix is singular");
            }

            // Copy right hand side with pivoting
            int count = value.Length;
            var b = new double[count];
            for (int i = 0; i < b.Length; i++)
                b[i] = value[pivotVector[i]];

            int rows = lu.GetLength(1);
            int columns = lu.GetLength(1);


            // Solve L*Y = B
            var X = new double[count];
            for (int i = 0; i < rows; i++)
            {
                X[i] = b[i];
                for (int j = 0; j < i; j++)
                    X[i] -= lu[i, j]*X[j];
            }

            // Solve U*X = Y;
            for (int i = rows - 1; i >= 0; i--)
            {
                //double sum = 0.0;
                for (int j = columns - 1; j > i; j--)
                    X[i] -= lu[i, j]*X[j];
                X[i] /= lu[i, i];
            }

            return X;
        }

        #endregion

        /// <summary>Solves a set of equation systems of type <c>A * X = I</c>.</summary>
        public double[,] Inverse()
        {
            if (!Nonsingular)
            {
                throw new InvalidOperationException("Matrix is singular");
            }

            int rows = lu.GetLength(1);
            int columns = lu.GetLength(1);
            int count = rows;

            // Copy right hand side with pivoting
            var X = new double[rows,columns];
            for (int i = 0; i < rows; i++)
            {
                int k = pivotVector[i];
                X[i, k] = 1.0;
            }

            // Solve L*Y = B(piv,:)
            for (int k = 0; k < columns; k++)
                for (int i = k + 1; i < columns; i++)
                    for (int j = 0; j < count; j++)
                        X[i, j] -= X[k, j]*lu[i, k];

            // Solve U*X = I;
            for (int k = columns - 1; k >= 0; k--)
            {
                for (int j = 0; j < count; j++)
                    X[k, j] /= lu[k, k];

                for (int i = 0; i < k; i++)
                    for (int j = 0; j < count; j++)
                        X[i, j] -= X[k, j]*lu[i, k];
            }

            return X;
        }

        /// <summary>Solves a set of equation systems of type <c>X * A = B</c>.</summary>
        /// <param name="value">Right hand side matrix with as many columns as <c>A</c> and any number of rows.</param>
        /// <returns>Matrix <c>X</c> so that <c>X * L * U = A</c>.</returns>
        public double[,] SolveTranspose(double[,] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value.GetLength(0) != lu.GetLength(0))
            {
                throw new ArgumentException("Invalid matrix dimensions.", "value");
            }

            if (!Nonsingular)
            {
                throw new InvalidOperationException("Matrix is singular");
            }

            // Copy right hand side with pivoting
            double[,] X = value.Submatrix(null, pivotVector);

            int count = X.GetLength(1);
            int columns = lu.GetLength(1);

            // Solve L*Y = B(piv,:)
            for (int k = 0; k < columns; k++)
                for (int i = k + 1; i < columns; i++)
                    for (int j = 0; j < count; j++)
                        X[j, i] -= X[j, k]*lu[i, k];

            // Solve U*X = Y;
            for (int k = columns - 1; k >= 0; k--)
            {
                for (int j = 0; j < count; j++)
                    X[j, k] /= lu[k, k];

                for (int i = 0; i < k; i++)
                    for (int j = 0; j < count; j++)
                        X[j, i] -= X[j, k]*lu[i, k];
            }

            return X;
        }
    }
}