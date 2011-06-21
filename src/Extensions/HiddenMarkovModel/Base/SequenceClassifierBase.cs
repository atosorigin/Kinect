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

namespace Accord.Statistics.Models.Markov
{
    /// <summary>
    ///   Base class for (HMM) Sequence Classifiers.
    /// </summary>
    /// 
    [Serializable]
    public abstract class SequenceClassifierBase<TModel> : ISequenceClassifier
        where TModel : IHiddenMarkovModel
    {
        private readonly TModel[] models;

        /// <summary>
        ///   Initializes a new instance of the <see cref="SequenceClassifierBase&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="classes">The number of classes in the classification problem.</param>
        protected SequenceClassifierBase(int classes)
        {
            models = new TModel[classes];
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SequenceClassifierBase&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="models">The models specializing in each of the classes of the classification problem.</param>
        protected SequenceClassifierBase(TModel[] models)
        {
            this.models = models;
        }


        /// <summary>
        ///   Gets the collection of models specialized in each class
        ///   of the sequence classification problem.
        /// </summary>
        public TModel[] Models
        {
            get { return models; }
        }

        /// <summary>
        ///   Gets the <see cref="IHiddenMarkovModel">Hidden Markov
        ///   Model</see> implementation responsible for recognizing
        ///   each of the classes given the desired class label.
        /// </summary>
        /// <param name="label">The class label of the model to get.</param>
        public TModel this[int label]
        {
            get { return models[label]; }
        }

        #region ISequenceClassifier Members

        /// <summary>
        ///   Gets the number of classes which can be recognized by this classifier.
        /// </summary>
        public int Classes
        {
            get { return models.Length; }
        }

        #endregion

        /// <summary>
        ///   Computes the most likely class for a given sequence.
        /// </summary>
        protected int Compute(Array sequence)
        {
            double[] likelihoods;
            return Compute(sequence, out likelihoods);
        }

        /// <summary>
        ///   Computes the most likely class for a given sequence.
        /// </summary>
        protected int Compute(Array sequence, out double likelihood)
        {
            double[] likelihoods;
            int label = Compute(sequence, out likelihoods);
            likelihood = likelihoods[label];
            return label;
        }


        /// <summary>
        ///   Computes the most likely class for a given sequence.
        /// </summary>
        protected int Compute(Array sequence, out double[] likelihoods)
        {
            var response = new double[models.Length];

            // For every model in the set,
#if !DEBUG
            AForge.Parallel.For(0, models.Length, i =>
#else
            for (int i = 0; i < models.Length; i++)
#endif
            {
                // Evaluate the probability for the given sequence
                response[i] = models[i].Evaluate(sequence);
            }
#if !DEBUG
            );
#endif

            int imax;
            response.Max(out imax);
            likelihoods = response;

            // Returns the index of the most likely model.
            return imax;
        }

        #region ISequenceClassifier implementation

        /// <summary>
        ///   Computes the most likely class for a given sequence.
        /// </summary>
        int ISequenceClassifier.Compute(Array sequence, out double[] likelihoods)
        {
            return Compute(sequence, out likelihoods);
        }

        #endregion
    }
}