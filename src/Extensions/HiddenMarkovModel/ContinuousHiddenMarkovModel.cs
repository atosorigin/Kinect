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
using Accord.Statistics.Distributions;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Models.Markov.Topology;

namespace Accord.Statistics.Models.Markov
{
    /// <summary>
    ///   Continuous-density Hidden Markov Model.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Hidden Markov Models (HMM) are stochastic methods to model temporal and sequence
    ///   data. They are especially known for their application in temporal pattern recognition
    ///   such as speech, handwriting, gesture recognition, part-of-speech tagging, musical
    ///   score following, partial discharges and bioinformatics.</para>
    /// <para>
    ///   Dynamical systems of discrete nature assumed to be governed by a Markov chain emits
    ///   a sequence of observable outputs. Under the Markov assumption, it is also assumed that
    ///   the latest output depends only on the current state of the system. Such states are often
    ///   not known from the observer when only the output values are observable.</para>
    ///   
    /// <para>
    ///   Hidden Markov Models attempt to model such systems and allow, among other things,
    ///   <list type="number">
    ///     <item><description>
    ///       To infer the most likely sequence of states that produced a given output sequence,</description></item>
    ///     <item><description>
    ///       Infer which will be the most likely next state (and thus predicting the next output),</description></item>
    ///     <item><description>
    ///       Calculate the probability that a given sequence of outputs originated from the system
    ///       (allowing the use of hidden Markov models for sequence classification).</description></item>
    ///     </list></para>
    ///     
    ///  <para>     
    ///   The “hidden” in Hidden Markov Models comes from the fact that the observer does not
    ///   know in which state the system may be in, but has only a probabilistic insight on where
    ///   it should be.</para>
    ///   
    ///  <para>
    ///   The continuous Hidden Markov Model uses a continuous probability density function (such
    ///   as <see cref="Accord.Statistics.Distributions.Univariate.NormalDistribution">Gaussian</see>
    ///   <see cref="Accord.Statistics.Distributions.Univariate.Mixture{T}">Mixture Model</see>)
    ///   for computing the state probability. In other words, in a continuous HMM the matrix of emission
    ///   probabilities B is replaced by an array of continuous probability density functions.</para>
    ///  
    ///  <para>
    ///   If a <see cref="Accord.Statistics.Distributions.Univariate.GeneralDiscreteDistribution">general
    ///   discrete distribution</see> is used as the underlying probability density function, the
    ///   model becomes equivalent to the <see cref="HiddenMarkovModel">discrete Hidden Markov Model</see>.
    ///  </para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       http://en.wikipedia.org/wiki/Hidden_Markov_model</description></item>
    ///   </list></para>
    /// </remarks>
    ///
    /// <seealso cref="HiddenMarkovModel">Discrete-density Hidden Markov Model</seealso>
    /// 
    /// <example>
    ///   In the following example, we will create a Continuous Hidden Markov Model
    ///   using a Generic Discrete Probability Distribution to reproduce the same
    ///   code example given in <seealso cref="HiddenMarkovModel"/> documentation.
    ///   <code>
    ///   // Continuous Markov Models can operate using any
    ///   // probability distribution, including discrete ones. 
    ///   
    ///   // In the follwing example, we will try to create a
    ///   // Continuous Hidden Markov Model using a discrete
    ///   // distribution to detect if a given sequence starts
    ///   // with a zero and has any number of ones after that.
    ///   
    ///   double[][] sequences = new double[][] 
    ///   {
    ///       new double[] { 0,1,1,1,1,0,1,1,1,1 },
    ///       new double[] { 0,1,1,1,0,1,1,1,1,1 },
    ///       new double[] { 0,1,1,1,1,1,1,1,1,1 },
    ///       new double[] { 0,1,1,1,1,1         },
    ///       new double[] { 0,1,1,1,1,1,1       },
    ///       new double[] { 0,1,1,1,1,1,1,1,1,1 },
    ///       new double[] { 0,1,1,1,1,1,1,1,1,1 },
    ///   };
    ///   
    ///   // Create a new Hidden Markov Model with 3 states and
    ///   //  a generic discrete distribution with two symbols
    ///   ContinuousHiddenMarkovModel hmm = new ContinuousHiddenMarkovModel(3, 2);
    ///   
    ///   // Try to fit the model to the data until the difference in
    ///   //  the average log-likelihood changes only by as little as 0.0001
    ///   var teacher = new ContinuousBaumWelchLearning(hmm) { Tolerance = 0.0001, Iterations = 0 };
    ///   double ll = teacher.Run(sequences);
    ///   
    ///   // Calculate the probability that the given
    ///   //  sequences originated from the model
    ///   double l1 = hmm.Evaluate(new double[] { 0, 1 });       // 0.999
    ///   double l2 = hmm.Evaluate(new double[] { 0, 1, 1, 1 }); // 0.916
    ///   
    ///   // Sequences which do not start with zero have much lesser probability.
    ///   double l3 = hmm.Evaluate(new double[] { 1, 1 });       // 0.000
    ///   double l4 = hmm.Evaluate(new double[] { 1, 0, 0, 0 }); // 0.000
    ///   
    ///   // Sequences which contains few errors have higher probabability
    ///   //  than the ones which do not start with zero. This shows some
    ///   //  of the temporal elasticity and error tolerance of the HMMs.
    ///   double l5 = hmm.Evaluate(new double[] { 0, 1, 0, 1, 1, 1, 1, 1, 1 }); // 0.034
    ///   double l6 = hmm.Evaluate(new double[] { 0, 1, 1, 1, 1, 1, 1, 0, 1 }); // 0.034
    ///   </code>
    /// </example>
    /// 
    [Serializable]
    public class ContinuousHiddenMarkovModel : HiddenMarkovModelBase, IHiddenMarkovModel
    {
        // Model is defined as M = (A, B, pi)
        // Parameters (A, pi) are defined in base class
        private readonly IDistribution[] B; // emission probabilities
        private readonly int dimension;

        #region Constructors

        /// <summary>
        ///   Constructs a new Hidden Markov Model with discrete state probabilities.
        /// </summary>
        /// <param name="topology">
        ///   A <see cref="Topology"/> object specifying the initial values of the matrix of transition 
        ///   probabilities <c>A</c> and initial state probabilities <c>pi</c> to be used by this model.
        /// </param>
        /// <param name="emissions">
        ///   The initial emission probability distribution to be used by each of the states.
        /// </param>
        public ContinuousHiddenMarkovModel(ITopology topology, IDistribution emissions)
            : base(topology)
        {
            if (emissions == null)
            {
                throw new ArgumentNullException("emissions");
            }

            // Initialize B using the initial distribution
            B = new IDistribution[States];

            for (int i = 0; i < B.Length; i++)
                B[i] = (IDistribution) emissions.Clone();

            if (B[0] is IMultivariateDistribution)
                dimension = ((IMultivariateDistribution) B[0]).Dimension;
            else dimension = 1;
        }

        /// <summary>
        ///   Constructs a new Hidden Markov Model with discrete state probabilities.
        /// </summary>
        /// <param name="topology">
        ///   A <see cref="Topology"/> object specifying the initial values of the matrix of transition 
        ///   probabilities <c>A</c> and initial state probabilities <c>pi</c> to be used by this model.
        /// </param>
        /// <param name="symbols">The number of output symbols used for this model.</param>
        public ContinuousHiddenMarkovModel(ITopology topology, int symbols)
            : base(topology)
        {
            if (symbols <= 0)
            {
                throw new ArgumentOutOfRangeException("symbols",
                                                      "Number of symbols should be higher than zero.");
            }

            // Initialize B with a uniform discrete distribution
            B = new IDistribution[States];
            for (int i = 0; i < B.Length; i++)
                B[i] = new GeneralDiscreteDistribution(symbols);

            dimension = 1;
        }

        /// <summary>
        ///   Constructs a new Hidden Markov Model.
        /// </summary>
        /// <param name="topology">
        ///   A <see cref="Topology"/> object specifying the initial values of the matrix of transition 
        ///   probabilities <c>A</c> and initial state probabilities <c>pi</c> to be used by this model.
        /// </param>
        /// <param name="emissions">
        ///   The initial emission probability distributions for each state.
        /// </param>
        public ContinuousHiddenMarkovModel(ITopology topology, IDistribution[] emissions)
            : base(topology)
        {
            if (emissions == null)
            {
                throw new ArgumentNullException("emissions");
            }

            if (emissions.GetLength(0) != States)
            {
                throw new ArgumentException(
                    "The emission matrix should have the same number of rows as the number of states in the model.",
                    "emissions");
            }

            B = emissions;

            if (B[0] is IMultivariateDistribution)
                dimension = ((IMultivariateDistribution) B[0]).Dimension;
            else dimension = 1;
        }

        /// <summary>
        ///   Constructs a new Hidden Markov Model.
        /// </summary>
        /// <param name="transitions">The transitions matrix A for this model.</param>
        /// <param name="emissions">The emissions matrix B for this model.</param>
        /// <param name="probabilities">The initial state probabilities for this model.</param>
        public ContinuousHiddenMarkovModel(double[,] transitions, double[,] emissions, double[] probabilities)
            : base(new Custom(transitions, probabilities))
        {
            if (emissions == null)
            {
                throw new ArgumentNullException("emissions");
            }

            if (emissions.GetLength(0) != States)
            {
                throw new ArgumentException(
                    "The emission matrix should have the same number of rows as the number of states in the model.",
                    "emissions");
            }

            // Initialize B using a discrete distribution
            B = new GeneralDiscreteDistribution[States];
            for (int i = 0; i < B.Length; i++)
                B[i] = new GeneralDiscreteDistribution(Matrix.GetRow(emissions, i));

            dimension = 1;
        }

        /// <summary>
        ///   Constructs a new Hidden Markov Model.
        /// </summary>
        /// <param name="transitions">The transitions matrix A for this model.</param>
        /// <param name="emissions">The emissions matrix B for this model.</param>
        /// <param name="probabilities">The initial state probabilities for this model.</param>
        public ContinuousHiddenMarkovModel(double[,] transitions, IDistribution[] emissions, double[] probabilities)
            : this(new Custom(transitions, probabilities), emissions)
        {
        }

        /// <summary>
        ///   Constructs a new Hidden Markov Model with discrete state probabilities.
        /// </summary>
        /// <param name="states">The number of states for this model.</param>
        /// <param name="symbols">The number of output symbols used for this model.</param>
        public ContinuousHiddenMarkovModel(int states, int symbols)
            : this(new Ergodic(states), symbols)
        {
        }

        /// <summary>
        ///   Constructs a new Hidden Markov Model.
        /// </summary>
        /// <param name="states">The number of states for the model.</param>
        /// <param name="emissions">A initial distribution to be copied to all states in the model.</param>
        public ContinuousHiddenMarkovModel(int states, IDistribution emissions)
            : this(new Ergodic(states), emissions)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the number of dimensions in the
        ///   probability distributions for the states.
        /// </summary>
        public int Dimension
        {
            get { return dimension; }
        }

        /// <summary>
        ///   Gets the Emission matrix (B) for this model.
        /// </summary>
        public IDistribution[] Emissions
        {
            get { return B; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Calculates the most likely sequence of hidden states
        ///   that produced the given observation sequence.
        /// </summary>
        /// <remarks>
        ///   Decoding problem. Given the HMM M = (A, B, pi) and  the observation sequence 
        ///   O = {o1,o2, ..., oK}, calculate the most likely sequence of hidden states Si
        ///   that produced this observation sequence O. This can be computed efficiently
        ///   using the Viterbi algorithm.
        /// </remarks>
        /// <param name="observations">A sequence of observations.</param>
        /// <param name="probability">The state optimized probability.</param>
        /// <returns>The sequence of states that most likely produced the sequence.</returns>
        public int[] Decode(Array observations, out double probability)
        {
            return Decode(observations, false, out probability);
        }

        /// <summary>
        ///   Calculates the most likely sequence of hidden states
        ///   that produced the given observation sequence.
        /// </summary>
        /// <remarks>
        ///   Decoding problem. Given the HMM M = (A, B, pi) and  the observation sequence 
        ///   O = {o1,o2, ..., oK}, calculate the most likely sequence of hidden states Si
        ///   that produced this observation sequence O. This can be computed efficiently
        ///   using the Viterbi algorithm.
        /// </remarks>
        /// <param name="observations">A sequence of observations.</param>
        /// <param name="probability">The state optimized probability.</param>
        /// <param name="logarithm">True to return the log-likelihood, false to return
        /// the likelihood. Default is false (default is to return the likelihood).</param>
        /// <returns>The sequence of states that most likely produced the sequence.</returns>
        public int[] Decode(Array observations, bool logarithm, out double probability)
        {
            if (observations == null)
                throw new ArgumentNullException("observations");

            if (observations.Length == 0)
            {
                probability = 0.0;
                return new int[0];
            }

            if (!(observations is double[][] || observations is double[]))
                throw new ArgumentException("Argument should be either of type " +
                                            "double[] (for univariate observation) or double[][] (for " +
                                            "multivariate observation).", "observations");


            double[][] x = convert(observations);


            // Viterbi-forward algorithm.
            int T = x.Length;
            int states = States;
            int minState;
            double minWeight;
            double weight;

            double[] pi = Probabilities;
            double[,] A = Transitions;

            var s = new int[states,T];
            var a = new double[states,T];


            // Base
            for (int i = 0; i < states; i++)
                a[i, 0] = -System.Math.Log(pi[i]) - System.Math.Log(B[i].ProbabilityFunction(x[0]));

            // Induction
            for (int t = 1; t < T; t++)
            {
                double[] observation = x[t];

                for (int j = 0; j < states; j++)
                {
                    minState = 0;
                    minWeight = a[0, t - 1] - System.Math.Log(A[0, j]);

                    for (int i = 1; i < states; i++)
                    {
                        weight = a[i, t - 1] - System.Math.Log(A[i, j]);

                        if (weight < minWeight)
                        {
                            minState = i;
                            minWeight = weight;
                        }
                    }

                    a[j, t] = minWeight - System.Math.Log(B[j].ProbabilityFunction(observation));
                    s[j, t] = minState;
                }
            }

            // Find minimum value for time T-1
            minState = 0;
            minWeight = a[0, T - 1];

            for (int i = 1; i < states; i++)
            {
                if (a[i, T - 1] < minWeight)
                {
                    minState = i;
                    minWeight = a[i, T - 1];
                }
            }


            // Trackback
            var path = new int[T];
            path[T - 1] = minState;

            for (int t = T - 2; t >= 0; t--)
                path[t] = s[path[t + 1], t + 1];


            // Returns the sequence probability as an out parameter
            probability = logarithm ? -minWeight : System.Math.Exp(-minWeight);

            // Returns the most likely (Viterbi path) for the given sequence
            return path;
        }

        /// <summary>
        ///   Calculates the probability that this model has generated the given sequence.
        /// </summary>
        /// <remarks>
        ///   Evaluation problem. Given the HMM  M = (A, B, pi) and  the observation
        ///   sequence O = {o1, o2, ..., oK}, calculate the probability that model
        ///   M has generated sequence O. This can be computed efficiently using the
        ///   either the Viterbi or the Forward algorithms.
        /// </remarks>
        /// <param name="observations">
        ///   A sequence of observations.
        /// </param>
        /// <returns>
        ///   The probability that the given sequence has been generated by this model.
        /// </returns>
        public double Evaluate(Array observations)
        {
            return Evaluate(observations, false);
        }

        /// <summary>
        ///   Calculates the probability that this model has generated the given sequence.
        /// </summary>
        /// <remarks>
        ///   Evaluation problem. Given the HMM  M = (A, B, pi) and  the observation
        ///   sequence O = {o1, o2, ..., oK}, calculate the probability that model
        ///   M has generated sequence O. This can be computed efficiently using the
        ///   either the Viterbi or the Forward algorithms.
        /// </remarks>
        /// <param name="observations">
        ///   A sequence of observations.
        /// </param>
        /// <param name="logarithm">
        ///   True to return the log-likelihood, false to return
        ///   the likelihood. Default is false.
        /// </param>
        /// <returns>
        ///   The probability that the given sequence has been generated by this model.
        /// </returns>
        public double Evaluate(Array observations, bool logarithm)
        {
            if (observations == null)
                throw new ArgumentNullException("observations");

            if (observations.Length == 0)
                return 0.0;

            if (!(observations is double[][] || observations is double[]))
                throw new ArgumentException("Argument should be either of type " +
                                            "double[] (for univariate observation) or double[][] (for " +
                                            "multivariate observation).", "observations");


            double[][] obs = convert(observations);

            // Forward algorithm
            double logLikelihood;

            // Compute forward probabilities
            ForwardBackwardAlgorithm.Forward(this, obs, out logLikelihood);

            // Return the sequence probability
            return logarithm ? logLikelihood : System.Math.Exp(logLikelihood);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///   Converts a univariate or multivariate array
        ///   of observations into a two-dimensional jagged array.
        /// </summary>
        private double[][] convert(Array array)
        {
            var multivariate = array as double[][];
            if (multivariate != null) return multivariate;

            var univariate = array as double[];
            if (univariate != null) return Matrix.Split(univariate, Dimension);

            throw new ArgumentException("Invalid array argument type.", "array");
        }

        #endregion

        //---------------------------------------------

        //---------------------------------------------

        //---------------------------------------------
    }
}