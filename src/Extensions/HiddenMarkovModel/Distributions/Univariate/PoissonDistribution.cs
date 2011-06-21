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
    ///   Poisson probability distribution.
    /// </summary>
    /// <remarks>
    ///   <para>The Poisson distribution is a discrete probability distribution that
    ///   expresses the probability of a number of events occurring in a fixed
    ///   period of time if these events occur with a known average rate and
    ///   independently of the time since the last event. (The Poisson distribution
    ///   can also be used for the number of events in other specified intervals
    ///   such as distance, area or volume.)</para>
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Poisson_distribution">
    ///       http://en.wikipedia.org/wiki/Poisson_distribution</a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    [Serializable]
    public class PoissonDistribution : UnivariateDiscreteDistribution
    {
        // distribution parameters

        // derived values
        private readonly double epml;
        private readonly double lambda;

        // distribution measures
        private double? entropy;


        /// <summary>
        ///   Creates a new Poisson distribution with the given lambda.
        /// </summary>
        /// <param name="lambda">The Poisson's lambda parameter.</param>
        public PoissonDistribution(double lambda)
        {
            this.lambda = lambda;
            epml = System.Math.Exp(-lambda);
        }

        /// <summary>
        /// Gets the mean for this distribution.
        /// </summary>
        public override double Mean
        {
            get { return lambda; }
        }

        /// <summary>
        /// Gets the variance for this distribution.
        /// </summary>
        public override double Variance
        {
            get { return lambda; }
        }

        /// <summary>
        /// Gets the entropy for this distribution.
        /// </summary>
        /// <remarks>
        ///   A closed form expression for the entropy of a Poisson
        ///   distribution is unknown. This property returns an approximation
        ///   for large lambda.
        /// </remarks>
        public override double Entropy
        {
            get
            {
                if (entropy == null)
                {
                    entropy = 0.5*System.Math.Log(2.0*System.Math.PI*lambda)
                              - 1/(12*lambda)
                              - 1/(24*lambda*lambda)
                              - 19/(360*lambda*lambda*lambda);
                }

                return entropy.Value;
            }
        }

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
        public override double DistributionFunction(int x)
        {
            return Special.Igam(x + 1, lambda)/Special.Factorial(x);
        }

        /// <summary>
        ///   Gets the probability mass function (pmf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">
        ///   A single point in the distribution range.</param>
        /// <remarks>
        ///   The Probability Mass Function (PMF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.</returns>
        ///   
        public override double ProbabilityMassFunction(int x)
        {
            return (System.Math.Pow(lambda, x)/Special.Factorial(x))*epml;
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
            return new PoissonDistribution(mean);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new PoissonDistribution(lambda);
        }
    }
}