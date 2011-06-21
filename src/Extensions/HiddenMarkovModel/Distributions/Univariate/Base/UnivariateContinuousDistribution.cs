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

namespace Accord.Statistics.Distributions.Univariate
{
    /// <summary>
    ///   Abstract class for Probability Distributions.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   A probability distribution identifies either the probability of each value of an
    ///   unidentified random variable (when the variable is discrete), or the probability
    ///   of the value falling within a particular interval (when the variable is continuous).</para>
    /// <para>
    ///   The probability distribution describes the range of possible values that a random
    ///   variable can attain and the probability that the value of the random variable is
    ///   within any (measurable) subset of that range.</para>  
    /// <para>
    ///   The function describing the probability that a given value will occur is called
    ///   the probability function (or probability density function, abbreviated PDF), and
    ///   the function describing the cumulative probability that a given value or any value
    ///   smaller than it will occur is called the distribution function (or cumulative
    ///   distribution function, abbreviated CDF).</para>  
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Probability_distribution">
    ///       http://en.wikipedia.org/wiki/Probability_distribution</a></description></item>
    ///     <item><description><a href="http://mathworld.wolfram.com/StatisticalDistribution.html">
    ///       http://mathworld.wolfram.com/StatisticalDistribution.html</a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    [Serializable]
    public abstract class UnivariateContinuousDistribution : IDistribution, IUnivariateDistribution
    {
        /// <summary>
        ///   Gets the Standard Deviation (the square root of
        ///   the variance) for the current distribution.
        /// </summary>
        public double StandardDeviation
        {
            get { return System.Math.Sqrt(Variance); }
        }

        #region IDistribution explicit members

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
        double IDistribution.DistributionFunction(params double[] x)
        {
            return DistributionFunction(x[0]);
        }

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
        double IDistribution.ProbabilityFunction(params double[] x)
        {
            return ProbabilityDensityFunction(x[0]);
        }

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
        IDistribution IDistribution.Fit(Array observations, double[] weights)
        {
            var univariate = observations as double[];
            if (univariate != null) return Fit(univariate, weights);

            var multivariate = observations as double[][];
            if (multivariate != null) return Fit(Matrix.Combine(multivariate), weights);

            throw new ArgumentException("Invalid input type.", "observations");
        }

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
        IDistribution IDistribution.Fit(Array observations)
        {
            var weights = new double[observations.Length];

            // Create equal weights for the observations
            for (int i = 0; i < weights.Length; i++)
                weights[i] = 1.0/observations.Length;

            return (this as IDistribution).Fit(observations, weights);
        }

        #endregion

        #region IDistribution Members

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        public abstract object Clone();

        #endregion

        #region IUnivariateDistribution Members

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        public abstract double Mean { get; }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        public abstract double Variance { get; }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        public abstract double Entropy { get; }

        #endregion

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   the this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">
        ///   A single point in the distribution range.</param>
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        public abstract double DistributionFunction(double x);

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">
        ///   A single point in the distribution range.</param>
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.</returns>
        ///   
        public abstract double ProbabilityDensityFunction(double x);

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
        public abstract IDistribution Fit(double[] observations, double[] weights);

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// <param name="observations">
        ///   The array of observations to fit the model against.
        /// </param>
        /// <returns>
        ///  Returns a new IDistribution fitted to the given observations.
        /// </returns>
        public virtual IDistribution Fit(double[] observations)
        {
            var weights = new double[observations.Length];

            // Create equal weights for the observations
            for (int i = 0; i < weights.Length; i++)
                weights[i] = 1.0/observations.Length;

            return Fit(observations, weights);
        }
    }
}