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
    ///   Chi-Square (χ²) probability distribution
    /// </summary>
    /// <remarks>
    /// <para>
    ///   In probability theory and statistics, the chi-square distribution (also chi-squared
    ///   or χ²-distribution) with k degrees of freedom is the distribution of a sum of the 
    ///   squares of k independent standard normal random variables. It is one of the most 
    ///   widely used probability distributions in inferential statistics, e.g. in hypothesis 
    ///   testing, or in construction of confidence intervals.</para>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Chi-square_distribution">
    ///       http://en.wikipedia.org/wiki/Chi-square_distribution</a></description></item>
    ///   </list></para>     
    /// </remarks>
    /// 
    [Serializable]
    public class ChiSquareDistribution : UnivariateContinuousDistribution
    {
        //  Distribution parameters
        private readonly int degreesOfFreedom;

        /// <summary>
        ///   Constructs a new Chi-Square distribution
        ///   with given degrees of freedom.
        /// </summary>
        public ChiSquareDistribution(int degreesOfFreedom)
        {
            this.degreesOfFreedom = degreesOfFreedom;
        }

        /// <summary>
        ///   Gets the Degrees of Freedom for this distribution.
        /// </summary>
        public int DegreesOfFreedom
        {
            get { return degreesOfFreedom; }
        }


        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        public override double Mean
        {
            get { return degreesOfFreedom; }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        public override double Variance
        {
            get { return 2.0*degreesOfFreedom; }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        public override double Entropy
        {
            get
            {
                double kd2 = degreesOfFreedom/2.0;
                double m1 = System.Math.Log(2.0*Special.Gamma(kd2));
                double m2 = (1.0 - kd2)*Special.Digamma(kd2);
                return kd2 + m1 + m2;
            }
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   the χ² distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.</para>
        /// <para>
        ///   References:
        ///   <list type="bullet">
        ///     <item><description>
        ///       <a href="http://www.mathworks.com/access/helpdesk/help/toolbox/stats/chi2pdf.html">
        ///       http://www.mathworks.com/access/helpdesk/help/toolbox/stats/chi2pdf.html</a></description></item>
        ///   </list></para>
        /// </remarks>
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.</returns>
        ///   
        public override double ProbabilityDensityFunction(double x)
        {
            double v = degreesOfFreedom;
            double m1 = System.Math.Pow(x, (v - 2.0)/2.0);
            double m2 = System.Math.Exp(-x/2.0);
            double m3 = System.Math.Pow(2, v/2.0)*Special.Gamma(v/2.0);
            return (m1*m2)/m3;
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   the χ² distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        public override double DistributionFunction(double x)
        {
            return Special.ChiSq(degreesOfFreedom, x);
        }

        /// <summary>
        ///   Gets the complementary cumulative distribution
        ///   function for the χ² evaluated at point <c>x</c>.
        /// </summary>
        public double SurvivalFunction(double x)
        {
            return Special.ChiSqc(degreesOfFreedom, x);
        }

        /// <summary>
        ///   This method is not supported.
        /// </summary>
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
            return new ChiSquareDistribution(degreesOfFreedom);
        }
    }
}