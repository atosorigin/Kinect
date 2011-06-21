// Accord Statistics Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//

using System;
using Accord.Statistics.Distributions;
using Accord.Statistics.Models.Markov.Topology;

namespace Accord.Statistics.Models.Markov
{
    /// <summary>
    ///   Continuous-density Hidden Markov Model Set for Sequence Classification.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class uses a set of hidden Markov models to classify sequences of
    ///   real (double-precision floating point) numbers or arrays of those numbers.
    ///   Each model will try to learn and recognize each of the different output classes.
    /// </remarks>
    /// 
    /// <example>
    ///   <code>
    ///   // We will try to create a Continuous density Hidden Markov
    ///   // Model to detect a sequence and the same sequence backwards.
    ///   double[][] sequences = new double[][] 
    ///   {
    ///       new double[] { 0,1,2,3,4 }, // This is the first  sequence with label = 0
    ///       new double[] { 4,3,2,1,0 }, // This is the second sequence with label = 1
    ///   };
    ///   
    ///   // Labels for the sequences
    ///   int[] labels = { 0, 1 };
    ///
    ///   // Creates a sequence classifier containing 2 hidden Markov Models
    ///   //  with 2 states and an underlying Normal distribution as density.
    ///   NormalDistribution density = new NormalDistribution();
    ///   var classifier = new ContinuousSequenceClassifier(2, new Ergodic(2), density);
    ///
    ///   // Configure the learning algorithms to train the sequence classifier
    ///   var teacher = new SequenceClassifierLearning(classifier,
    ///
    ///   // Train each model until the log-likelihood changes less than 0.001
    ///   modelIndex => new ContinuousBaumWelchLearning(classifier.Models[modelIndex])
    ///   {
    ///           Tolerance = 0.0001,
    ///           Iterations = 0
    ///       }
    ///   );
    ///   
    ///   // Train the sequence classifier using the algorithm
    ///   teacher.Run(sequences, labels);
    ///   
    ///   
    ///   // Calculate the probability that the given
    ///   //  sequences originated from the model
    ///   double likelihood;
    ///   
    ///   // Try to classify the first sequence (output should be 0)
    ///   int c1 = classifier.Compute(sequences[0], out likelihood);
    ///   
    ///   // Try to classify the second sequence (output should be 1)
    ///   int c2 = classifier.Compute(sequences[1], out likelihood);
    ///   </code>
    /// </example>
    /// 
    [Serializable]
    public class ContinuousSequenceClassifier : SequenceClassifierBase<ContinuousHiddenMarkovModel>,
                                                ISequenceClassifier
    {
        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        public ContinuousSequenceClassifier(int classes, ITopology topology, IDistribution initial)
            : base(classes)
        {
            for (int i = 0; i < classes; i++)
                Models[i] = new ContinuousHiddenMarkovModel(topology, initial);
        }

        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        public ContinuousSequenceClassifier(int classes, ITopology topology, IDistribution initial, string[] names)
            : base(classes)
        {
            for (int i = 0; i < classes; i++)
                Models[i] = new ContinuousHiddenMarkovModel(topology, initial) {Tag = names[i]};
        }

        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        public ContinuousSequenceClassifier(int classes, ITopology[] topology, IDistribution[] initial, string[] names)
            : base(classes)
        {
            for (int i = 0; i < classes; i++)
                Models[i] = new ContinuousHiddenMarkovModel(topology[i], initial[i]) {Tag = names[i]};
        }

        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        public ContinuousSequenceClassifier(ContinuousHiddenMarkovModel[] models)
            : base(models)
        {
        }

        #region ISequenceClassifier Members

        /// <summary>
        ///   Computes the most likely class for a given sequence.
        /// </summary>
        public new int Compute(Array sequence, out double[] likelihoods)
        {
            return base.Compute(sequence, out likelihoods);
        }

        #endregion

        /// <summary>
        ///   Computes the most likely class for a given sequence.
        /// </summary>
        public new int Compute(Array sequence)
        {
            return base.Compute(sequence);
        }

        /// <summary>
        ///   Computes the most likely class for a given sequence.
        /// </summary>
        public new int Compute(Array sequence, out double likelihood)
        {
            return base.Compute(sequence, out likelihood);
        }
    }
}