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
    ///   Custom Topology for Hidden Markov Model.
    /// </summary>
    /// 
    /// <remarks>
    ///  <para>
    ///   An Hidden Markov Model Topology specifies how many states and which
    ///   initial probabilities a Markov model should have. Two common topologies
    ///   can be discussed in terms of transition state probabilities and are
    ///   available to construction through the <see cref="Ergodic"/> and
    ///   <see cref="Forward"/> classes implementing the <see cref="ITopology"/>
    ///   interface.</para>
    ///   
    ///  <para>Topology specification is important with regard to both learning and
    ///   performance: A model with too many states (and thus too many settable
    ///   parameters) will require too much training data while an model with an
    ///   insufficient number of states will prohibit the HMM from capturing subtle
    ///   statistical patterns.</para>
    ///   
    ///  <para>This custom implementation allows for arbitrarily specification of
    ///   the state transition matrix and initial state probabilities for
    ///   <see cref="IHiddenMarkovModel">hidden Markov models</see>.</para>
    ///   
    /// </remarks>
    ///   
    /// <seealso cref="HiddenMarkovModel"/>
    /// <seealso cref="Accord.Statistics.Models.Markov.Topology.ITopology"/>
    /// <seealso cref="Accord.Statistics.Models.Markov.Topology.Ergodic"/>
    /// <seealso cref="Accord.Statistics.Models.Markov.Topology.Forward"/>
    ///
    [Serializable]
    public class Custom : ITopology
    {
        private readonly double[] pi;
        private readonly int states;
        private readonly double[,] transitions;

        /// <summary>
        ///   Creates a new custom topology with user-defined
        ///   transition matrix and initial state probabilities.
        /// </summary>
        public Custom(double[,] transitions, double[] initial)
        {
            if (transitions == null)
            {
                throw new ArgumentNullException("transitions");
            }

            if (initial == null)
            {
                throw new ArgumentNullException("initial");
            }

            if (transitions.GetLength(0) != transitions.GetLength(1))
            {
                if (transitions.GetLength(0) != transitions.GetLength(1))
                    throw new ArgumentException(
                        "Transition matrix should be square.",
                        "transitions");
            }

            if (initial.Length != transitions.GetLength(0))
            {
                throw new ArgumentException(
                    "Initial probabilities should have the same length as the number of states in the transition matrix.",
                    "initial");
            }

            states = transitions.GetLength(0);
            this.transitions = transitions;
            pi = initial;
        }


        /// <summary>
        ///   Gets the initial state probabilities.
        /// </summary>
        public double[] Initial
        {
            get { return pi; }
        }

        /// <summary>
        ///   Gets the state-transitions matrix.
        /// </summary>
        public double[,] Transitions
        {
            get { return transitions; }
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
            transitionMatrix = (double[,]) transitions.Clone();
            initialState = (double[]) pi.Clone();
            return states;
        }

        #endregion
    }
}