// Accord Math Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//
// Copyright © Antonino Porcino, 2010
// iz8bly at yahoo.it
//

using System;

namespace Accord.Math.Decompositions
{
    /// <summary>
    ///   Nonnegative Matrix Factorization algorithms.
    /// </summary>
    public enum NonnegativeFactorizationAlgorithm
    {
        /// <summary>
        ///   Multiplicative update rule, also known as Lee and Seung's updates.
        ///   May lead to slower convergence and is very sensitive to initial values.
        /// </summary>
        MultiplicativeUpdate,

        /// <summary>
        ///   Alternate Least Squares update. May converge faster and more
        ///   consistently than the multiplicative update rule.
        /// </summary>
        AlternateLeastSquares,
    }

    /// <summary>
    ///   Nonnegative Matrix Factorization.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   For a nonnegative n-by-m matrix <c>A</c>, the Nonnegative Matrix Factorization
    ///   is given by a n-by-k matrix of nonnegative factors <c>W</c> and a k-by-m matrix
    ///   of nonnegative factor coefficients <c>H</c>. The factorization is not exact, as
    ///   <c>W * H</c> is a lower-rank approximation to <c>A</c>. The factors <c>W</c> and
    ///   <c>H</c> are chosen to minimize the root-mean-squared residual <c>U</c> between
    ///   <c>A</c> and <c>W * H</c>.</para>
    ///   
    ///   <para>
    ///     References:
    ///     <list type="bullet">
    ///       <item><description>
    ///         <a href="http://en.wikipedia.org/wiki/Non-negative_matrix_factorization">
    ///         http://en.wikipedia.org/wiki/Non-negative_matrix_factorization</a>
    ///       </description></item>
    ///       <item><description>
    ///         <a href="http://www.mathworks.com/help/toolbox/stats/nnmf.html">
    ///         http://www.mathworks.com/help/toolbox/stats/nnmf.html</a>
    ///       </description></item>
    ///     </list>
    ///   </para>
    /// </remarks>
    /// 
    public sealed class NonnegativeFactorization
    {
        private static readonly double sqrteps = System.Math.Sqrt(Special.DoubleEpsilon);
        private double[,] H; // H is r x n (transformed data transposed)
        private double[,] W; // W is m x r (weights)

        private int k; // dimension of output vector (rank approximation)
        private int m; // dimension of input vector
        private int n; // number of input data vectors

        // cached value for further calculations


        /// <summary>
        ///   Constructs a new non-negative matrix factorization.
        /// </summary>
        /// <param name="value">The matrix to be factorized.</param>
        public NonnegativeFactorization(double[,] value)
            : this(value, value.GetLength(1))
        {
        }

        /// <summary>
        ///   Constructs a new non-negative matrix factorization.
        /// </summary>
        /// <param name="value">The matrix to be factorized.</param>
        /// <param name="k">The desired approximation rank.</param>
        public NonnegativeFactorization(double[,] value, int k)
            : this(value, k, NonnegativeFactorizationAlgorithm.AlternateLeastSquares)
        {
        }

        /// <summary>
        ///   Constructs a new non-negative matrix factorization.
        /// </summary>
        /// <param name="value">The matrix to be factorized.</param>
        /// <param name="k">The desired approximation rank.</param>
        /// <param name="attempts">How many repetitions of the method should be
        /// performed to avoid arriving at a poor local solution minima. Default
        /// value is <c>1</c>.</param>
        public NonnegativeFactorization(double[,] value, int k, int attempts)
        {
            init(value, k, NonnegativeFactorizationAlgorithm.AlternateLeastSquares,
                 null, null, attempts, 100, 1e-4, 1e-4);
        }

        /// <summary>
        ///   Constructs a new non-negative matrix factorization.
        /// </summary>
        /// <param name="value">The matrix to be factorized.</param>
        /// <param name="k">The desired approximation rank.</param>
        /// <param name="algorithm">The algorithm to be used in the factorization.
        /// Please see <see cref="NonnegativeFactorizationAlgorithm"/> for details.
        /// Default is <see cref="NonnegativeFactorizationAlgorithm.AlternateLeastSquares"/>.</param>
        public NonnegativeFactorization(double[,] value, int k, NonnegativeFactorizationAlgorithm algorithm)
        {
            init(value, k, algorithm, null, null, 1, 100, 1e-4, 1e-4);
        }

        /// <summary>
        ///   Constructs a new non-negative matrix factorization.
        /// </summary>
        /// <param name="value">The matrix to be factorized.</param>
        /// <param name="h0">Initial approximation to the coefficient matrix H.
        /// Default is <see langword="null"/>.</param>
        /// <param name="w0">Initial approximation to the weight matrix W.
        /// Default is <see langword="null"/>.</param>
        public NonnegativeFactorization(double[,] value, double[,] h0, double[,] w0)
        {
            init(value, w0.GetLength(1), NonnegativeFactorizationAlgorithm.AlternateLeastSquares, h0, w0, 1, 100, 1e-4,
                 1e-4);
        }

        /// <summary>
        ///   Constructs a new non-negative matrix factorization.
        /// </summary>
        /// <param name="value">The matrix to be factorized.</param>
        /// <param name="h0">Initial approximation to the coefficient matrix H.</param>
        /// <param name="w0">Initial approximation to the weight matrix W.</param>
        /// <param name="algorithm">The algorithm to be used in the factorization.
        /// Please see <see cref="NonnegativeFactorizationAlgorithm"/> for details.
        /// Default is <see cref="NonnegativeFactorizationAlgorithm.AlternateLeastSquares"/>.</param>
        public NonnegativeFactorization(double[,] value, double[,] h0, double[,] w0,
                                        NonnegativeFactorizationAlgorithm algorithm)
        {
            init(value, w0.GetLength(1), algorithm, h0, w0, 1, 100, 1e-4, 1e-4);
        }

        /// <summary>
        ///   Constructs a new non-negative matrix factorization.
        /// </summary>
        /// <param name="value">The matrix to be factorized.</param>
        /// <param name="k">The desired approximation rank.</param>
        /// <param name="algorithm">The algorithm to be used in the factorization.
        /// Please see <see cref="NonnegativeFactorizationAlgorithm"/> for details.
        /// Default is <see cref="NonnegativeFactorizationAlgorithm.AlternateLeastSquares"/>.</param>
        /// <param name="attempts">How many repetitions of the method should be
        /// performed to avoid arriving at a poor local solution minima. Default
        /// value is <c>1</c>.</param>
        /// <param name="maxIterations">The maximum number of iterations to perform.</param>
        /// <param name="errorTolerance">The minimum change in error to use as convergence criteria.</param>
        /// <param name="changeTolerance">The maximum absolute factor change to use as convergence criteria.</param>
        public NonnegativeFactorization(double[,] value, int k, NonnegativeFactorizationAlgorithm algorithm,
                                        int attempts, int maxIterations, double errorTolerance, double changeTolerance)
        {
            init(value, k, algorithm, null, null, attempts, maxIterations, errorTolerance, changeTolerance);
        }

        /// <summary>
        ///   Constructs a new non-negative matrix factorization.
        /// </summary>
        /// <param name="value">The matrix to be factorized.</param>
        /// <param name="h0">Initial approximation to the coefficient matrix H.</param>
        /// <param name="w0">Initial approximation to the weight matrix W.</param>
        /// <param name="algorithm">The algorithm to be used in the factorization.
        /// Please see <see cref="NonnegativeFactorizationAlgorithm"/> for details.
        /// Default is <see cref="NonnegativeFactorizationAlgorithm.AlternateLeastSquares"/>.</param>
        /// <param name="attempts">How many repetitions of the method should be
        /// performed to avoid arriving at a poor local solution minima. Default
        /// value is <c>1</c>.</param>
        /// <param name="maxIterations">The maximum number of iterations to perform.</param>
        /// <param name="errorTolerance">The minimum change in error to use as convergence criteria.</param>
        /// <param name="changeTolerance">The maximum absolute factor change to use as convergence criteria.</param>
        public NonnegativeFactorization(double[,] value, double[,] h0, double[,] w0,
                                        NonnegativeFactorizationAlgorithm algorithm,
                                        int attempts, int maxIterations, double errorTolerance, double changeTolerance)
        {
            init(value, k, algorithm, h0, w0, attempts, maxIterations, errorTolerance, changeTolerance);
        }

        /// <summary>
        ///   Gets the nonnegative factor matrix W.
        /// </summary>
        public double[,] LeftNonnegativeFactors
        {
            get { return W; }
        }

        /// <summary>
        ///   Gets the nonnegative factor matrix H.
        /// </summary>
        public double[,] RightNonnegativeFactors
        {
            get { return H; }
        }


        /// <summary>
        ///   Constructs a new non-negative matrix factorization.
        /// </summary>
        private void init(double[,] value, int k, NonnegativeFactorizationAlgorithm algorithm,
                          double[,] h0, double[,] w0, int attempts, int maxIterations, double errorTolerance,
                          double changeTolerance)
        {
            #region Initial argument checking

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (k < 0 || k > value.GetLength(1))
            {
                throw new ArgumentException(
                    "The approximation rank k should be a positive integer equal or less than the number of columns of the matrix to be decomposed.",
                    "k");
            }

            if (h0 != null)
            {
                if (h0.GetLength(0) != k || h0.GetLength(1) != value.GetLength(1))
                    throw new ArgumentException(
                        "The initial coefficient matrix should have the same number of columns as the value matrix and the same number of rows as the desired rank approximation.",
                        "h0");
            }

            if (w0 != null)
            {
                if (w0.GetLength(0) != value.GetLength(0) || w0.GetLength(1) != k)
                    throw new ArgumentException(
                        "The initial weight matrix should have the same number of rows as the value matrix and the same number of columns as the desired rank approximation.",
                        "w0");
            }

            if (attempts <= 0)
            {
                throw new ArgumentException("The number of parallel attempts should be greater than zero.", "attempts");
            }

            if (maxIterations <= 0)
            {
                throw new ArgumentException("The maximum number of iterations should be greater than zero.",
                                            "maxIterations");
            }

            if (errorTolerance <= 0)
            {
                throw new ArgumentException("The error tolerance convergence threshold should be greater than zero.",
                                            "errorTolerance");
            }

            if (changeTolerance <= 0)
            {
                throw new ArgumentException(
                    "The maximum absolute factor change convergence threshold should be greater than zero.",
                    "changeTolerance");
            }

            #endregion

            // Initialization
            double[,] X = value;
            n = X.GetLength(0);
            m = X.GetLength(1);
            this.k = k;
            W = w0;
            H = h0;


            // First, check for special case
            if (W == null && H == null)
            {
                // In the case both W and H are to
                // be found, the answer is direct.

                if (k == m)
                {
                    W = X;
                    H = Matrix.Identity(k);
                }
                else if (k == n)
                {
                    W = Matrix.Identity(k);
                    H = X;
                }
            }


            // Next we will attempt several factorizations
            //  and return the one which produces the best
            //  approximation norm.

            // TODO: This should be parallelized.

            double bestNorm = Double.PositiveInfinity;
            double[,] bestW = null;
            double[,] bestH = null;

            // Perform several parallel attempts
            for (int t = 0; t < attempts; t++)
            {
                // Perform round initialization to random values
                if (W == null || t > 0) W = Matrix.Random(n, k);
                if (H == null || t > 0) H = Matrix.Random(k, m);
                double norm = Double.PositiveInfinity;


                // Perform a factorization
                norm = nnmf(X, ref W, ref H, algorithm,
                            maxIterations, errorTolerance, changeTolerance);


                // Check if this has been the
                //  best factorization so far
                if (norm < bestNorm)
                {
                    bestNorm = norm;
                    bestW = W;
                    bestH = H;
                }
            }


            // Select the best factorization
            //  and normalize the results

            H = bestH;
            W = bestW;

            for (int i = 0; i < k; i++)
            {
                double norm = 0.0, v;
                for (int j = 0; j < m; j++)
                {
                    v = H[i, j];
                    norm += v*v;
                }

                if (norm == 0) // If any of the norms equals 0
                {
                    // Then the algorithm has converged to a solution
                    // with a different rank than the rank specified.
                    norm = 1;
                }
                else
                {
                    norm = System.Math.Sqrt(norm);
                }

                // Normalize H and re-scale W
                for (int j = 0; j < m; j++)
                    H[i, j] /= norm;

                for (int j = 0; j < n; j++)
                    W[j, i] *= norm;
            }

            // Then order by the norms of W
            var wnorm = new double[k];
            var index = new int[k];
            for (int j = 0; j < k; j++)
            {
                double norm = 0.0, d;
                for (int i = 0; i < n; i++)
                {
                    d = W[i, j];
                    norm += d*d;
                }

                // Set as minus to order
                //  in descending order
                wnorm[j] = -norm;
                index[j] = j;
            }

            Array.Sort(wnorm, index);

            W = W.Submatrix(null, index);
            H = H.Submatrix(index, null);
        }


        /// <summary>
        ///   Single non-negative matrix factorization.
        /// </summary>
        private double nnmf(double[,] value,
                            ref double[,] w0, ref double[,] h0, NonnegativeFactorizationAlgorithm alg,
                            int maxIterations, double normChangeThreshold, double maxFactorChangeThreshold)
        {
            double[,] v = value;
            double[,] h = h0;
            double[,] w = w0;
            double[,] z = null;

            double dnorm0 = 0.0; // previous iteration norm

            // Main loop
            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                // Check which algorithm should be used
                if (alg == NonnegativeFactorizationAlgorithm.MultiplicativeUpdate)
                {
                    // Multiplicative update formula
                    h = new double[k,m];
                    w = new double[n,k];

                    if (z == null) z = new double[k,k];

                    // Update H
                    for (int i = 0; i < k; i++)
                    {
                        for (int j = i; j < k; j++)
                        {
                            double s = 0.0;
                            for (int l = 0; l < n; l++)
                                s += w0[l, i]*w0[l, j];
                            z[i, j] = z[j, i] = s;
                        }

                        for (int j = 0; j < m; j++)
                        {
                            double d = 0.0;
                            for (int l = 0; l < k; l++)
                                d += z[i, l]*h0[l, j];

                            double s = 0.0;
                            for (int l = 0; l < n; l++)
                                s += w0[l, i]*v[l, j];

                            h[i, j] = h0[i, j]*s/(d + Special.Epslon(s));
                        }
                    }

                    // Update W
                    for (int j = 0; j < k; j++)
                    {
                        for (int i = j; i < k; i++)
                        {
                            double s = 0.0;
                            for (int l = 0; l < m; l++)
                                s += h[i, l]*h[j, l];
                            z[i, j] = z[j, i] = s;
                        }

                        for (int i = 0; i < n; i++)
                        {
                            double d = 0.0;
                            for (int l = 0; l < k; l++)
                                d += w0[i, l]*z[j, l];

                            double s = 0.0;
                            for (int l = 0; l < m; l++)
                                s += v[i, l]*h[j, l];

                            w[i, j] = w0[i, j]*s/(d + Special.Epslon(s));
                        }
                    }
                }
                else
                {
                    // Alternating least squares update
                    h = w0.Solve(v);
                    makepositive(h);
                    w = v.Divide(h);
                    makepositive(w);
                }


                // Reconstruct matrix A using W and H
                double[,] r = w.Multiply(h);

                // Get norm of difference
                double dnorm = normdiff(v, r);

                // Get maximum change in factors
                double dw = maxdiff(w0, w)/(sqrteps + maxabs(w0));
                double dh = maxdiff(h0, h)/(sqrteps + maxabs(h0));
                double delta = System.Math.Max(dw, dh);

                if (iteration > 0) // Check for convergence
                {
                    if (delta <= maxFactorChangeThreshold ||
                        dnorm <= normChangeThreshold*dnorm0)
                        break;
                }

                // Remember previous iteration results
                dnorm0 = dnorm;
                w0 = w;
                h0 = h;
            }

            return dnorm0;
        }

        /// <summary>
        ///   Norm of differences
        /// </summary>
        private static unsafe double normdiff(double[,] matrixA, double[,] matrixB)
        {
            int length = matrixA.Length;
            double dnorm = 0.0, d;

            fixed (double* ptrA = matrixA, ptrB = matrixB)
            {
                double* a = ptrA, b = ptrB;
                for (int i = 0; i < length; i++, a++, b++)
                {
                    d = *a - *b;
                    dnorm += d*d;
                }

                return System.Math.Sqrt(dnorm/length);
            }
        }

        /// <summary>
        ///   Max absolute difference
        /// </summary>
        private static unsafe double maxdiff(double[,] matrixA, double[,] matrixB)
        {
            int length = matrixA.Length;

            fixed (double* ptrA = matrixA, ptrB = matrixB)
            {
                double* a = ptrA, b = ptrB;
                double max = System.Math.Abs(*a - *b), d;
                for (int i = 1; i < length; i++, a++, b++)
                {
                    d = System.Math.Abs(*a - *b);
                    if (d > max) max = d;
                }

                return max;
            }
        }

        /// <summary>
        ///   Max absolute value
        /// </summary>
        private static unsafe double maxabs(double[,] value)
        {
            int length = value.Length;

            fixed (double* matrix = value)
            {
                double* p = matrix;

                double max = System.Math.Abs(*p), d;
                for (int i = 1; i < length; i++, p++)
                {
                    d = System.Math.Abs(*p);
                    if (d > max) max = d;
                }

                return max;
            }
        }

        /// <summary>
        ///   Enforces a matrix to contain only positive values.
        /// </summary>
        private static unsafe void makepositive(double[,] value)
        {
            int length = value.Length;

            fixed (double* ptr = value)
            {
                double* p = ptr;
                for (int i = 0; i < length; i++, p++)
                {
                    if (*p < 0) *p = 0;
                }
            }
        }
    }
}