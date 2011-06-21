// Accord Statistics Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//

using System;

namespace Accord.Statistics.Distributions.Univariate
{
    /// <summary>
    ///   Bernoulli probability distribution.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   The Bernoulli distribution is a distribution for a single
    ///   binary variable x E {0,1}, representing, for example, the
    ///   flipping of a coin. It is governed by a single continuous
    ///   parameter representing the probability of an observation
    ///   to be equal to 1.</para>
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Bernoulli_distribution">
    ///       http://en.wikipedia.org/wiki/Bernoulli_distribution</a></description></item>
    ///     <item><description>
    ///       C. Bishop. “Pattern Recognition and Machine Learning”. Springer. 2006.</description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    [Serializable]
    public class BernoulliDistribution : UnivariateDiscreteDistribution
    {
        // distribution parameters

        // derived parameter values
        private readonly double complement;

        // distribution measures
        private readonly double entropy;
        private readonly double probability;
        private readonly double variance;


        /// <summary>
        ///   Creates a new <see cref="BernoulliDistribution">Bernoulli</see> distribution.
        /// </summary>
        /// <param name="mean">The probability of an observation being equal to 1.</param>
        public BernoulliDistribution(double mean)
        {
            probability = mean;
            complement = 1.0 - mean;

            variance = mean*(1.0 - mean);
            entropy = -mean*System.Math.Log(mean) -
                      (complement)*System.Math.Log(complement);
        }

        /// <summary>
        /// Gets the mean for this distribution.
        /// </summary>
        public override double Mean
        {
            get { return probability; }
        }

        /// <summary>
        /// Gets the variance for this distribution.
        /// </summary>
        public override double Variance
        {
            get { return variance; }
        }

        /// <summary>
        /// Gets the entropy for this distribution.
        /// </summary>
        public override double Entropy
        {
            get { return entropy; }
        }

        /// <summary>
        /// Gets the cumulative distribution function (cdf) for
        /// the this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        /// <returns></returns>
        /// <remarks>
        /// The Cumulative Distribution Function (CDF) describes the cumulative
        /// probability that a given value or any value smaller than it will occur.
        /// </remarks>
        public override double DistributionFunction(int x)
        {
            if (x < 0) return 0;
            if (x >= 1) return 1;
            return complement;
        }

        /// <summary>
        /// Gets the probability mass function (pmf) for
        /// this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        /// <returns>
        /// The probability of <c>x</c> occurring
        /// in the current distribution.
        /// </returns>
        /// <remarks>
        /// The Probability Mass Function (PMF) describes the
        /// probability that a given value <c>x</c> will occur.
        /// </remarks>
        public override double ProbabilityMassFunction(int x)
        {
            if (x == 1) return probability;
            if (x == 0) return complement;
            return 0;
        }

        /// <summary>
        /// Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// <param name="observations">The array of observations to fit the model against.</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <returns>
        /// Returns a new IDistribution fitted to the given observations.
        /// </returns>
        public override IDistribution Fit(double[] observations, double[] weights)
        {
            double mean = observations.Mean(weights);
            return new BernoulliDistribution(mean);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new BernoulliDistribution(probability);
        }
    }
}