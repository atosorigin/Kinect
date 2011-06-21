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
    ///   Base class for Hidden Markov Models.
    /// </summary>
    /// 
    [Serializable]
    public abstract class HiddenMarkovModelBase
    {
        // Model is defined as M = (A, B, pi)
        private readonly double[,] A; // Transition probabilities
        private readonly double[] pi; // Initial state probabilities
        private readonly int states; // number of states
        private object tag;


        /// <summary>
        ///   Constructs a new Hidden Markov Model.
        /// </summary>
        protected HiddenMarkovModelBase(ITopology topology)
        {
            states = topology.Create(out A, out pi);
        }


        /// <summary>
        ///   Gets the number of states of this model.
        /// </summary>
        public int States
        {
            get { return states; }
        }

        /// <summary>
        ///   Gets the initial probabilities for this model.
        /// </summary>
        public double[] Probabilities
        {
            get { return pi; }
        }

        /// <summary>
        ///   Gets the Transition matrix (A) for this model.
        /// </summary>
        public double[,] Transitions
        {
            get { return A; }
        }

        /// <summary>
        ///   Gets or sets a user-defined tag.
        /// </summary>
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }
    }
}