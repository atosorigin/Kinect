// Accord Statistics Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//

using System;
using Accord.Math;
using Accord.Math.Decompositions;
using AForge.Math.Random;

namespace Accord.Statistics.Distributions.Multivariate
{
    /// <summary>
    ///   Multivariate Normal (Gaussian) distribution.
    /// </summary>
    /// <remarks>
    ///   The Gaussian is the most widely used distribution for continuous
    ///   variables. In the case of many variables, it is governed by
    ///   two parameters, the mean vector and the variance-covariance matrix.
    /// </remarks>
    /// 
    [Serializable]
    public class NormalDistribution : MultivariateContinuousDistribution
    {
        // Distribution parameters

        private CholeskyDecomposition chol;
        private double constant;
        private double[,] covariance;
        private double[] mean;
        private SingularValueDecomposition svd;
        private double[] variance;


        /// <summary>
        ///   Constructs a multivariate Gaussian distribution
        ///   with zero mean vector and unitary variance matrix.
        /// </summary>
        public NormalDistribution(int dimension)
            : this(new double[dimension], Matrix.Identity(dimension))
        {
        }

        /// <summary>
        ///   Constructs a multivariate Gaussian distribution
        ///   with given mean vector and covariance matrix.
        /// </summary>
        public NormalDistribution(double[] mean, double[,] covariance)
            : base(mean.Length)
        {
            int k = mean.Length;

            this.mean = mean;
            this.covariance = covariance;
            variance = covariance.Diagonal();

            double detSqrt = System.Math.Sqrt(System.Math.Abs(covariance.Determinant()));
            constant = 1.0/(System.Math.Pow(2.0*System.Math.PI, k/2.0)*detSqrt);

            chol = new CholeskyDecomposition(covariance, true);

            if (chol.Determinant == 0)
            {
                // The covariance matrix is singular, use pseudo-inverse
                svd = new SingularValueDecomposition(covariance);
            }
        }

        /// <summary>
        ///   Gets the Mean vector for the Gaussian distribution.
        /// </summary>
        public override double[] Mean
        {
            get { return mean; }
        }

        /// <summary>
        ///   Gets the Variance vector for the Gaussian distribution.
        /// </summary>
        public override double[] Variance
        {
            get { return variance; }
        }

        /// <summary>
        ///   Gets the variance-covariance matrix for the Gaussian distribution.
        /// </summary>
        public override double[,] Covariance
        {
            get { return covariance; }
        }

        /// <summary>
        ///   This method is not supported.
        /// </summary>
        public override double DistributionFunction(params double[] x)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">A single point in the distribution range. For a
        ///   univariate distribution, this should be a single
        ///   double value. For a multivariate distribution,
        ///   this should be a double array.</param>
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.
        /// </returns>
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        public override double ProbabilityDensityFunction(params double[] x)
        {
            double[] z = x.Subtract(mean);

            double[] a = (svd == null) ? chol.Solve(z) : svd.Solve(z);

            double b = a.InnerProduct(z);

            double r = constant*System.Math.Exp(-0.5*b);

            return r > 1.0 ? 1.0 : r;
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// <param name="observations">
        ///   The array of observations to fit the model against.
        /// </param>
        /// <param name="weights">
        ///   The weight vector containing the weight for each of the samples.
        /// </param>
        /// <returns>
        ///  Returns a new IDistribution fitted to the given observations.
        /// </returns>
        public override IDistribution Fit(double[][] observations, double[] weights)
        {
#if DEBUG
            for (int i = 0; i < weights.Length; i++)
                if (Double.IsNaN(weights[i]) || Double.IsInfinity(weights[i]))
                    throw new Exception("Invalid numbers in the weight vector.");
#endif
            // Compute weighted mean vector
            double[] means = Tools.Mean(observations, weights);

            // Compute weighted covariance matrix
            double[,] cov = Tools.Covariance(observations, means, weights);

            // return the newly fitted distribution.
            return new NormalDistribution(means, cov);
        }


        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            var clone = new NormalDistribution(Dimension);
            clone.constant = constant;
            clone.covariance = (double[,]) covariance.Clone();
            clone.mean = (double[]) mean.Clone();
            clone.variance = (double[]) variance.Clone();

            clone.chol = (CholeskyDecomposition) chol.Clone();
            clone.svd = (svd != null) ? (SingularValueDecomposition) svd.Clone() : null;

            return clone;
        }


        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// <param name="samples">The number of samples to generate.</param>
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        public double[][] Generate(int samples)
        {
            var r = new StandardGenerator();
            double[,] A = chol.LeftTriangularFactor;

            var data = new double[samples][];
            for (int i = 0; i < data.Length; i++)
            {
                var sample = new double[Dimension];
                for (int j = 0; j < sample.Length; j++)
                    sample[j] = r.Next();

                data[i] = A.Multiply(sample).Add(Mean);
            }

            return data;
        }
    }
}