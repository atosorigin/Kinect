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
    ///   F (Fisher-Snedecor) distribution.
    /// </summary>
    /// 
    [Serializable]
    public class FDistribution : UnivariateContinuousDistribution
    {
        // distribution parameters
        private readonly double b;
        private readonly int d1;
        private readonly int d2;

        // derived values


        /// <summary>
        ///   Constructs a F-distribution with
        ///   the given degrees of freedom.
        /// </summary>
        /// <param name="degrees1">The first degree of freedom.</param>
        /// <param name="degrees2">The second degree of freedom.</param>
        public FDistribution(int degrees1, int degrees2)
        {
            d1 = degrees1;
            d2 = degrees2;

            b = Special.Beta(degrees1*0.5, degrees2*0.5);
        }

        /// <summary>
        ///   Gets the first degree of freedom.
        /// </summary>
        public int DegreesOfFreedom1
        {
            get { return d1; }
        }

        /// <summary>
        ///   Gets the second degree of freedom.
        /// </summary>
        public int DegreesOfFreedom2
        {
            get { return d2; }
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        public override double Mean
        {
            get
            {
                if (d2 <= 2) return Double.NaN;

                return d2/(d2 - 2.0);
            }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        public override double Variance
        {
            get
            {
                if (d2 <= 4) return Double.NaN;

                return (2.0*d2*d2*(d1 + d2 - 2))/
                       (d1*(d2 - 2)*(d2 - 2)*(d2 - 4));
            }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        public override double Entropy
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   the F-distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        /// <returns></returns>
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        public override double DistributionFunction(double x)
        {
            double u = (d1*x)/(d1*x + d2);
            return Special.Ibeta(d1*0.5, d2*0.5, u);
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   the F-distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
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
            double u = System.Math.Pow(d1*x, d1)*System.Math.Pow(d2, d2)/
                       System.Math.Pow(d1*x + d2, d1 + d2);
            return System.Math.Sqrt(u)/(x*b);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// <param name="observations">The array of observations to fit the model against.</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <returns>
        ///   Returns a new IDistribution fitted to the given observations.
        /// </returns>
        public override IDistribution Fit(double[] observations, double[] weights)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new FDistribution(d1, d2);
        }
    }
}