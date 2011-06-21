// Accord Statistics Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//

using System;

namespace Accord.Statistics.Distributions
{
    /// <summary>
    ///   Common interface for probability distributions.
    /// </summary>
    /// 
    public interface IDistribution : ICloneable
    {
        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   the this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        double DistributionFunction(params double[] x);

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">
        ///   A single point in the distribution range. For a 
        ///   univariate distribution, this should be a single
        ///   double value. For a multivariate distribution,
        ///   this should be a double array.</param>
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.</returns>
        ///   
        double ProbabilityFunction(params double[] x);

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// <param name="observations">
        ///   The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).
        /// </param>
        /// <remarks>
        ///   Although both double[] and double[][] arrays are supported,
        ///   providing a double[] for a multivariate distribution or
        ///   a double[][] for a univariate distribution may have a negative
        ///   impact in performance.
        /// </remarks>
        /// <param name="weights">
        ///   The weight vector containing the weight for each of the samples.
        /// </param>
        /// <returns>
        ///  Returns a new IDistribution fitted to the given observations.
        /// </returns>
        IDistribution Fit(Array observations, double[] weights);

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// <param name="observations">
        ///   The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).
        /// </param>
        /// <remarks>
        ///   Although both double[] and double[][] arrays are supported,
        ///   providing a double[] for a multivariate distribution or
        ///   a double[][] for a univariate distribution may have a negative
        ///   impact in performance.
        /// </remarks>
        /// <returns>
        ///  Returns a new IDistribution fitted to the given observations.
        /// </returns>
        IDistribution Fit(Array observations);
    }

    /// <summary>
    ///   Common interface for univariate probability distributions.
    /// </summary>
    /// 
    public interface IUnivariateDistribution : IDistribution
    {
        /// <summary>
        ///   Gets the mean value for the distribution.
        /// </summary>
        /// 
        double Mean { get; }

        /// <summary>
        ///   Gets the variance value for the distribution.
        /// </summary>
        /// 
        double Variance { get; }

        /// <summary>
        ///   Gets entropy of the distribution.
        /// </summary>
        /// 
        double Entropy { get; }
    }

    /// <summary>
    ///   Common interface for multivariate probability distributions.
    /// </summary>
    /// 
    public interface IMultivariateDistribution : IDistribution
    {
        /// <summary>
        ///   Gets the number of variables for the distribution.
        /// </summary>
        /// 
        int Dimension { get; }

        /// <summary>
        ///   Gets the Mean vector for the distribution.
        /// </summary>
        /// 
        double[] Mean { get; }

        /// <summary>
        ///   Gets the Variance vector for the distribution.
        /// </summary>
        /// 
        double[] Variance { get; }

        /// <summary>
        ///   Gets the Variance-Covariance matrix for the distribution.
        /// </summary>
        /// 
        double[,] Covariance { get; }
    }
}