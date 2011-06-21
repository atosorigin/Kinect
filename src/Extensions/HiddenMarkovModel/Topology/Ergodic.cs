// Accord Statistics Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//

using System;

namespace Accord.Statistics.Models.Markov.Topology
{
    /// <summary>
    ///   Ergodic (fully-connected) Topology for Hidden Markov Models.
    /// </summary>
    /// 
    /// <remarks>
    ///  <para>
    ///   Ergodic models are commonly used to represent models in which a single
    ///   (large) sequence of observations is used for training (such as when a
    ///   training sequence does not have well defined starting and ending points
    ///   and can potentially be infinitely long).</para>
    ///   
    ///  <para>Models starting with an ergodic transition-state topology typically
    ///   have only a small number of states.</para>
    ///   
    ///  <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Alexander Schliep, "Learning Hidden Markov Model Topology".</description></item>
    ///     <item><description>
    ///       Richard Hughey and Anders Krogh, "Hidden Markov models for sequence analysis: 
    ///       extension and analysis of the basic method", CABIOS 12(2):95-107, 1996. Available in:
    ///       http://compbio.soe.ucsc.edu/html_format_papers/hughkrogh96/cabios.html</description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <seealso cref="HiddenMarkovModel"/>
    /// <seealso cref="Accord.Statistics.Models.Markov.Topology.ITopology"/>
    /// <seealso cref="Accord.Statistics.Models.Markov.Topology.Forward"/>
    /// <seealso cref="Accord.Statistics.Models.Markov.Topology.Custom"/>
    /// 
    /// <example>
    ///  <para>
    ///   In a second example, we will create an Ergodic (fully connected)
    ///   discrete-density hidden Markov model with uniform probabilities.</para>
    ///   
    ///   <code>
    ///   // Create a new Ergodic hidden Markov model with three
    ///   //   fully-connected states and four sequence symbols.
    ///   var model = new HiddenMarkovModel(new Ergodic(3), 4);
    ///
    ///   // After creation, the state transition matrix for the model
    ///   // should be given by:
    ///   //
    ///   //       { 0.33, 0.33, 0.33 }
    ///   //       { 0.33, 0.33, 0.33 }
    ///   //       { 0.33, 0.33, 0.33 }
    ///   //       
    ///   // in which all state transitions are allowed.
    ///   </code>
    /// 
    /// </example>
    [Serializable]
    public class Ergodic : ITopology
    {
        private readonly double[] pi;
        private readonly int states;
        private bool random;


        /// <summary>
        ///   Creates a new Ergodic topology for a given number of states.
        /// </summary>
        /// <param name="states">The number of states in the model.</param>
        public Ergodic(int states)
            : this(states, false)
        {
        }

        /// <summary>
        ///   Creates a new Ergodic topology for a given number of states.
        /// </summary>
        /// <param name="states">The number of states in the model.</param>
        /// <param name="random">True to use random initial values, false
        /// to use a uniform distribution.</param>
        public Ergodic(int states, bool random)
        {
            if (states <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "states", "Number of states should be higher than zero.");
            }

            this.states = states;
            this.random = random;
            pi = new double[states];

            //for (int i = 0; i < pi.Length; i++)
            //    pi[i] = 1.0 / states;
            pi[0] = 1.0;
        }

        /// <summary>
        ///   Gets or sets whether the transition matrix
        ///   should be initialized with random probabilities
        ///   or not. Default is false.
        /// </summary>
        public bool Random
        {
            get { return random; }
            set { random = value; }
        }

        /// <summary>
        ///   Gets the initial state probabilities.
        /// </summary>
        public double[] Initial
        {
            get { return pi; }
        }

        #region ITopology Members

        /// <summary>
        ///   Gets the number of states in this topology.
        /// </summary>
        public int States
        {
            get { return states; }
        }


        /// <summary>
        ///   Creates the state transitions matrix and the
        ///   initial state probabilities for this topology.
        /// </summary>
        public int Create(out double[,] transitionMatrix, out double[] initialState)
        {
            var A = new double[States,States];

            if (random)
            {
                // Create A using random uniform distribution

                for (int i = 0; i < states; i++)
                {
                    double sum = 0.0;
                    for (int j = 0; j < states; j++)
                        sum += A[i, j] = Math.Tools.Random.NextDouble();

                    for (int j = 0; j < states; j++)
                        A[i, j] /= sum;
                }
            }
            else
            {
                // Create A using equal uniform probabilities,

                for (int i = 0; i < states; i++)
                    for (int j = 0; j < states; j++)
                        A[i, j] = 1.0/states;
            }

            transitionMatrix = A;
            initialState = (double[]) pi.Clone();
            return States;
        }

        #endregion
    }
}