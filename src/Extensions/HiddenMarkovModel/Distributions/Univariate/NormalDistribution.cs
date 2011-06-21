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
    ///   Normal (Gaussian) distribution.
    /// </summary>
    /// <remarks>
    ///   The Gaussian is the most widely used distribution for continuous
    ///   variables. In the case of a single variable, it is governed by
    ///   two parameters, the mean and the variance.
    /// </remarks>
    /// 
    [Serializable]
    public class NormalDistribution : UnivariateContinuousDistribution
    {
        // Distribution parameters
        private static readonly NormalDistribution standard = new NormalDistribution();
        private readonly double entropy;
        private readonly double mean;
        private readonly double variance;


        // Distribution measures


        /// <summary>
        ///   Constructs a Gaussian distribution with zero mean
        ///   and unit variance.
        /// </summary>
        public NormalDistribution()
            : this(0.0, 1.0)
        {
        }

        /// <summary>
        ///   Constructs a Gaussian distribution with given mean
        ///   and unit variance.
        /// </summary>
        /// <param name="mean"></param>
        public NormalDistribution(double mean)
            : this(mean, 1.0)
        {
        }

        /// <summary>
        ///   Constructs a Gaussian distribution with given mean
        ///   and given variance.
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="variance"></param>
        public NormalDistribution(double mean, double variance)
        {
            this.mean = mean;
            this.variance = variance;

            // Compute distribution measures
            double b = 2.0*System.Math.PI*variance;
            entropy = System.Math.Log(System.Math.Sqrt(b));
        }


        /// <summary>
        ///   Gets the Mean for the Gaussian distribution.
        /// </summary>
        public override double Mean
        {
            get { return mean; }
        }

        /// <summary>
        ///   Gets the Variance for the Gaussian distribution.
        /// </summary>
        public override double Variance
        {
            get { return variance; }
        }

        /// <summary>
        ///   Gets the Entropy for the Gaussian distribution.
        /// </summary>
        public override double Entropy
        {
            get { return entropy; }
        }

        /// <summary>
        ///   Gets the Standard Gaussian Distribution,
        ///   with zero mean and unit variance.
        /// </summary>
        public static NormalDistribution Standard
        {
            get { return standard; }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   the this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">
        ///   A single point in the distribution range.</param>
        /// <remarks>
        /// <para>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.</para>
        /// <para>
        ///  The calculation is computed through the relationship to
        ///  the function as <see cref="Accord.Math.Special.Erfc">erfc</see>(-z/sqrt(2)) / 2.</para>  
        ///  
        /// <para>    
        ///   References:
        ///   <list type="bullet">
        ///     <item><description><a href="http://mathworld.wolfram.com/NormalDistributionFunction.html">
        ///       http://mathworld.wolfram.com/NormalDistributionFunction.html</a></description></item>
        ///   </list></para>
        /// </remarks>
        public override double DistributionFunction(double x)
        {
            double z = ZScore(x);
            return Special.Erfc(-z/Special.Sqrt2)/2.0;
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   the Gaussian distribution evaluated at point <c>x</c>.
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
        public override double ProbabilityDensityFunction(double x)
        {
            double z = ZScore(x);
            return ((1.0/(Special.SqrtPI*variance))*System.Math.Exp((-z*z)/2.0));
        }

        /// <summary>
        ///   Gets the Z-Score for a given value.
        /// </summary>
        public double ZScore(double x)
        {
            return (x - mean)/variance;
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
        public override IDistribution Fit(double[] observations, double[] weights)
        {
            // Compute weighted mean
            double mean = observations.Mean(weights);

            // Compute weighted variance
            double variance = Tools.Variance(observations, mean, weights);

            return new NormalDistribution(mean, variance);
        }


        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new NormalDistribution(mean, variance);
        }
    }
}