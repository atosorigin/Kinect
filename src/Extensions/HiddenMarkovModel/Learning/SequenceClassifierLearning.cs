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

namespace Accord.Statistics.Models.Markov.Learning
{
    /// <summary>
    ///   Configuration function delegate for Sequence Classifier Learning algorithms.
    /// </summary>
    public delegate IUnsupervisedLearning SequenceClassifierLearningAlgorithmConfiguration(int modelIndex);


    /// <summary>
    ///   Sequence Classifier learning algorithm.
    /// </summary>
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
    public class SequenceClassifierLearning
    {
        private readonly ISequenceClassifier classifier;
        private SequenceClassifierLearningAlgorithmConfiguration algorithm;


        /// <summary>
        ///   Creates a new instance of the learning algorithm for a given 
        ///   Markov sequence classifier using the specified configuration
        ///   function.
        /// </summary>
        public SequenceClassifierLearning(ISequenceClassifier classifier,
                                          SequenceClassifierLearningAlgorithmConfiguration algorithm)
        {
            this.classifier = classifier;
            this.algorithm = algorithm;
        }

        /// <summary>
        ///   Gets or sets the configuration function specifying which
        ///   training algorithm should be used for each of the models
        ///   in the hidden Markov model set.
        /// </summary>
        public SequenceClassifierLearningAlgorithmConfiguration Algorithm
        {
            get { return algorithm; }
            set { algorithm = value; }
        }


        /// <summary>
        ///   Trains each model to recognize each of the output labels.
        /// </summary>
        /// <returns>The sum log-likelihood for all models after training.</returns>
        public double Run<T>(T[] inputs, int[] outputs)
        {
            double sum = 0;
            int classes = classifier.Classes;

            // For each model,
#if !DEBUG
            AForge.Parallel.For(0, classes, i =>
#else
            for (int i = 0; i < classes; i++)
#endif
            {
                // Select the input/output set corresponding
                //  to the model's specialization class
                int[] inx = outputs.Find(y => y == i);
                T[] observations = inputs.Submatrix(inx);

                if (observations.Length > 0)
                {
                    // Create and configure the learning algorithm
                    IUnsupervisedLearning teacher = algorithm(i);

                    // Train the current model in the input/output subset
                    sum += teacher.Run(observations as Array[]);
                }
            }
#if !DEBUG
            );
#endif

            // Returns the sum log-likelihood for all models.
            return sum;
        }
    }
}