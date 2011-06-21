// Accord Statistics Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//

using System;
using Accord.Statistics.Models.Markov.Topology;

namespace Accord.Statistics.Models.Markov
{
    /// <summary>
    ///   Discrete-density Hidden Markov Model Set for Sequence Classification.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class uses a set of hidden Markov models to classify integer sequences.
    ///   Each model will try to learn and recognize each of the different output classes.
    /// </remarks>
    /// 
    /// <example>
    ///   <code>
    ///   // Declare some testing data
    ///   int[][] inputs = new int[][]
    ///   {
    ///       new int[] { 0,1,1,0 },   // Class 0
    ///       new int[] { 0,0,1,0 },   // Class 0
    ///       new int[] { 0,1,1,1,0 }, // Class 0
    ///       new int[] { 0,1,0 },     // Class 0
    ///   
    ///       new int[] { 1,0,0,1 },   // Class 1
    ///       new int[] { 1,1,0,1 },   // Class 1
    ///       new int[] { 1,0,0,0,1 }, // Class 1
    ///       new int[] { 1,0,1 },     // Class 1
    ///   };
    ///   
    ///   int[] outputs = new int[]
    ///   {
    ///       0,0,0,0, // First four sequences are of class 0
    ///       1,1,1,1, // Last four sequences are of class 1
    ///   };
    ///   
    ///   
    ///   // We are trying to predict two different classes
    ///   int classes = 2;
    ///
    ///   // Each sequence may have up to two symbols (0 or 1)
    ///   int symbols = 2;
    ///
    ///   // Nested models will have two states each
    ///   int[] states = new int[] { 2, 2 };
    ///
    ///   // Creates a new Hidden Markov Model Classifier with the given parameters
    ///   SequenceClassifier classifier = new SequenceClassifier(classes, states, symbols);
    ///   
    ///   
    ///   // Create a new learning algorithm to train the sequence classifier
    ///   var teacher = new SequenceClassifierLearning(classifier,
    ///   
    ///   // Train each model until the log-likelihood changes less than 0.001
    ///   modelIndex => new BaumWelchLearning(classifier.Models[modelIndex])
    ///   {
    ///           Tolerance = 0.001,
    ///           Iterations = 0
    ///       }
    ///   );
    ///   
    ///   // Train the sequence classifier using the algorithm
    ///   double likelihood = teacher.Run(inputs, outputs);
    ///   
    ///   </code>
    /// </example>
    /// 
    [Serializable]
    public class SequenceClassifier : SequenceClassifierBase<HiddenMarkovModel>, ISequenceClassifier
    {
        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        public SequenceClassifier(int classes, ITopology topology, int symbols, string[] names)
            : base(classes)
        {
            for (int i = 0; i < classes; i++)
                Models[i] = new HiddenMarkovModel(topology, symbols) {Tag = names[i]};
        }

        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        public SequenceClassifier(int classes, ITopology topology, int symbols)
            : base(classes)
        {
            for (int i = 0; i < classes; i++)
                Models[i] = new HiddenMarkovModel(topology, symbols);
        }

        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        public SequenceClassifier(int classes, int[] states, int symbols, string[] names)
            : base(classes)
        {
            for (int i = 0; i < classes; i++)
                Models[i] = new HiddenMarkovModel(new Ergodic(states[i]), symbols) {Tag = names[i]};
        }

        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        public SequenceClassifier(int classes, int[] states, int symbols)
            : base(classes)
        {
            for (int i = 0; i < classes; i++)
                Models[i] = new HiddenMarkovModel(new Ergodic(states[i]), symbols);
        }


        /// <summary>
        ///   Computes the most likely class for a given sequence.
        /// </summary>
        public int Compute(int[] sequence)
        {
            return base.Compute(sequence);
        }

        /// <summary>
        ///   Computes the most likely class for a given sequence.
        /// </summary>
        public int Compute(int[] sequence, out double likelihood)
        {
            return base.Compute(sequence, out likelihood);
        }

        /// <summary>
        ///   Computes the most likely class for a given sequence.
        /// </summary>
        public int Compute(int[] sequence, out double[] likelihoods)
        {
            return base.Compute(sequence, out likelihoods);
        }
    }
}