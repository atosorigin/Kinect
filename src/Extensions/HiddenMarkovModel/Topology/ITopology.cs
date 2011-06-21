// Accord Statistics Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//

namespace Accord.Statistics.Models.Markov.Topology
{
    /// <summary>
    ///   Hidden Markov model topology (architecture) specification.
    /// </summary>
    /// 
    /// <remarks>
    ///  <para>
    ///   An Hidden Markov Model Topology specifies how many states and which
    ///   initial probabilities a Markov model should have. Two common topologies
    ///   can be discussed in terms of transition state probabilities and are
    ///   available to construction through the <see cref="Ergodic"/> and
    ///   <see cref="Forward"/> classes implementing this interface.</para>
    ///   
    ///  <para>Topology specification is important with regard to both learning and
    ///   performance: A model with too many states (and thus too many settable
    ///   parameters) will require too much training data while an model with an
    ///   insufficient number of states will prohibit the HMM from capturing subtle
    ///   statistical patterns.</para>
    /// 
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
    ///   
    /// </remarks>
    /// 
    /// <seealso cref="HiddenMarkovModel"/>
    /// <seealso cref="Accord.Statistics.Models.Markov.Topology.Ergodic"/>
    /// <seealso cref="Accord.Statistics.Models.Markov.Topology.Forward"/>
    /// <seealso cref="Accord.Statistics.Models.Markov.Topology.Custom"/>
    /// 
    public interface ITopology
    {
        /// <summary>
        ///   Gets the number of states in this topology.
        /// </summary>
        int States { get; }

        /// <summary>
        ///   Creates the state transitions matrix and the 
        ///   initial state probabilities for this topology.
        /// </summary>
        int Create(out double[,] transitionMatrix, out double[] initialState);
    }
}