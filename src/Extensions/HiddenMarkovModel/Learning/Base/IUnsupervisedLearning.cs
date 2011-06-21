// Accord Statistics Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//

using System;

namespace Accord.Statistics.Models.Markov.Learning
{
    /// <summary>
    ///   Common interface for unsupervised learning algorithms for
    ///   hidden Markov models such as the Baum-Welch learning algorithm.
    /// </summary>
    public interface IUnsupervisedLearning
    {
        /// <summary>
        ///   Runs the learning algorithm.
        /// </summary>
        /// <remarks>
        ///   Learning problem. Given some training observation sequences O = {o1, o2, ..., oK}
        ///   and general structure of HMM (numbers of hidden and visible states), determine
        ///   HMM parameters M = (A, B, pi) that best fit training data. 
        /// </remarks>
        double Run(Array[] observations);
    }
}