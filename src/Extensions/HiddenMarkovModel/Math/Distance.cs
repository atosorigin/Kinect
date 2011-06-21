// Accord Math Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//

namespace Accord.Math
{
    /// <summary>
    ///   Static class Distance. Defines a set of extension methods defining distance measures.
    /// </summary>
    /// 
    public static class Distance
    {
        /// <summary>
        ///   Gets the Square Mahalanobis distance between two points.
        /// </summary>
        /// <param name="x">A point in space.</param>
        /// <param name="y">A point in space.</param>
        /// <param name="precision">
        ///   The inverse of the covariance matrix of the distribution for the two points x and y.
        /// </param>
        /// <returns>The Square Mahalanobis distance between x and y.</returns>
        public static double SquareMahalanobis(this double[] x, double[] y, double[,] precision)
        {
            var d = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
                d[i] = x[i] - y[i];

            return d.InnerProduct(precision.Multiply(d));
        }

        /// <summary>
        ///   Gets the Mahalanobis distance between two points.
        /// </summary>
        /// <param name="x">A point in space.</param>
        /// <param name="y">A point in space.</param>
        /// <param name="precision">
        ///   The inverse of the covariance matrix of the distribution for the two points x and y.
        /// </param>
        /// <returns>The Mahalanobis distance between x and y.</returns>
        public static double Mahalanobis(this double[] x, double[] y, double[,] precision)
        {
            return System.Math.Sqrt(SquareMahalanobis(x, y, precision));
        }

        /// <summary>
        ///   Gets the Manhattan distance between two points.
        /// </summary>
        /// <param name="x">A point in space.</param>
        /// <param name="y">A point in space.</param>
        /// <returns>The manhattan distance between x and y.</returns>
        public static double Manhattan(this double[] x, double[] y)
        {
            double sum = 0.0;
            for (int i = 0; i < x.Length; i++)
                sum += System.Math.Abs(x[i] - y[i]);
            return sum;
        }

        /// <summary>
        ///   Gets the Square Euclidean distance between two points.
        /// </summary>
        /// <param name="x">A point in space.</param>
        /// <param name="y">A point in space.</param>
        /// <returns>The Square Euclidean distance between x and y.</returns>
        public static double SquareEuclidean(this double[] x, double[] y)
        {
            double d = 0.0, u;

            for (int i = 0; i < x.Length; i++)
            {
                u = x[i] - y[i];
                d += u*u;
            }

            return d;
        }

        /// <summary>
        ///   Gets the Euclidean distance between two points.
        /// </summary>
        /// <param name="x">A point in space.</param>
        /// <param name="y">A point in space.</param>
        /// <returns>The Euclidean distance between x and y.</returns>
        public static double Euclidean(this double[] x, double[] y)
        {
            return System.Math.Sqrt(SquareEuclidean(x, y));
        }

        /// <summary>
        ///   Gets the Modulo-m distance between two integers a and b.
        /// </summary>
        public static int Modular(int a, int b, int modulo)
        {
            return System.Math.Min(Tools.Mod(a - b, modulo), Tools.Mod(b - a, modulo));
        }

        /// <summary>
        ///   Bhattacharyya distance between two normalized histograms.
        /// </summary>
        /// <param name="histogram1">A normalized histogram.</param>
        /// <param name="histogram2">A normalized histogram.</param>
        /// <returns>The Bhattacharya distance between the two histograms.</returns>
        public static double Bhattacharyya(double[] histogram1, double[] histogram2)
        {
            int bins = histogram1.Length; // histogram bins
            double b = 0; // Bhattacharyya's coefficient

            for (int i = 0; i < bins; i++)
                b += System.Math.Sqrt(histogram1[i])*System.Math.Sqrt(histogram2[i]);

            // bhattacharyya distance between the two distributions
            return System.Math.Sqrt(1.0 - b);
        }

        /// <summary>
        ///   Bhattacharyya distance between two matrices.
        /// </summary>
        /// <returns>The Bhattacharia distance between the two matrices.</returns>
        public static double Bhattacharyya(double[,] x, double[,] y)
        {
            double[] meanX = mean(x);
            double[] meanY = mean(y);
            double[,] covX = cov(x, meanX);
            double[,] covY = cov(y, meanY);

            return Bhattacharyya(meanX, covX, meanY, covY);
        }


        /// <summary>
        ///   Bhattacharyya distance between two gaussian distributions.
        /// </summary>
        /// <param name="meanX">Mean for the first distribution.</param>
        /// <param name="covX">Covariance matrix for the first distribution.</param>
        /// <param name="meanY">Mean for the second distribution.</param>
        /// <param name="covY">Covariance matrix for the second distribution.</param>
        /// <returns>The Bhattacharia distance between the two distributions.</returns>
        public static double Bhattacharyya(double[] meanX, double[,] covX, double[] meanY, double[,] covY)
        {
            int n = covX.GetLength(0);

            // P = (covX + covY) / 2
            var P = new double[n,n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n - i; j++)
                    P[j, i] = P[i, j] = (covX[i, j] + covY[i, j])/2.0;

            double detP = P.Determinant(true);
            double detP1 = covX.Determinant(true);
            double detP2 = covY.Determinant(true);

            return (1.0/8.0)*SquareMahalanobis(meanY, meanX, P.Inverse())
                   + (0.5)*System.Math.Log(detP/System.Math.Sqrt(detP1*detP2));
        }

        #region Private methods

        private static double[] mean(double[,] matrix)
        {
            var mean = new double[matrix.GetLength(1)];
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            double N = matrix.GetLength(0);

            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < rows; i++)
                    mean[j] += matrix[i, j];
                mean[j] /= N;
            }

            return mean;
        }

        private static double[,] cov(double[,] matrix, double[] means)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            double divisor = rows - 1;

            var cov = new double[cols,cols];
            for (int i = 0; i < cols; i++)
            {
                for (int j = i; j < cols; j++)
                {
                    double s = 0.0;
                    for (int k = 0; k < rows; k++)
                        s += (matrix[k, j] - means[j])*(matrix[k, i] - means[i]);
                    s /= divisor;
                    cov[i, j] = s;
                    cov[j, i] = s;
                }
            }

            return cov;
        }

        #endregion
    }
}