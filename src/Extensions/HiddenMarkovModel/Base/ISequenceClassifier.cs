// Accord Statistics Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//

using System;

namespace Accord.Statistics.Models.Markov
{
    /// <summary>
    ///   Common interface for sequence classifiers using <see cref="IHiddenMarkovModel">
    ///   hidden Markov models</see>.
    /// </summary>
    public interface ISequenceClassifier
    {
        /// <summary>
        ///   Gets the number of classes which can be recognized by this classifier.
        /// </summary>
        int Classes { get; }

        /// <summary>
        ///   Computes the most likely class for a given sequence.
        /// </summary>
        int Compute(Array sequence, out double[] likelihoods);
    }
}