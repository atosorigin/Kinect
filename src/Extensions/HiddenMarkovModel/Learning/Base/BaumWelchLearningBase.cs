// Accord Statistics Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//

using System;

namespace Accord.Statistics.Models.Markov.Learning
{
    /// <summary>
    ///   Base class for implementations of the Baum-Welch learning algorithm.
    /// </summary>
    /// 
    public abstract class BaumWelchLearningBase
    {
        private readonly IHiddenMarkovModel model;
        private int maxIterations = 100;
        private double tolerance;

        /// <summary>
        ///   Initializes a new instance of the <see cref="BaumWelchLearningBase"/> class.
        /// </summary>
        protected BaumWelchLearningBase(IHiddenMarkovModel model)
        {
            this.model = model;
        }


        /// <summary>
        ///   Gets the Ksi matrix of probabilities created during the
        ///   last iteration of the Baum-Welch learning algorithm.
        /// </summary>
        public double[][][,] Ksi { get; protected set; }

        /// <summary>
        ///   Gets the Gamma matrix of probabilities created during the
        ///   last iteration of the Baum-Welch learning algorithm.
        /// </summary>
        public double[][,] Gamma { get; protected set; }


        /// <summary>
        ///   Gets or sets the maximum change in the average log-likelihood
        ///   after an iteration of the algorithm used to detect convergence.
        /// </summary>
        /// <remarks>
        ///   This is the likelihood convergence limit L between two iterations of the algorithm. The
        ///   algorithm will stop when the change in the likelihood for two consecutive iterations
        ///   has not changed by more than L percent of the likelihood. If left as zero, the
        ///   algorithm will ignore this parameter and iterates over a number of fixed iterations
        ///   specified by the previous parameter.
        /// </remarks>
        public double Tolerance
        {
            get { return tolerance; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "Tolerance should be positive.");

                tolerance = value;
            }
        }

        /// <summary>
        ///   Gets or sets the maximum number of iterations
        ///   performed by the learning algorithm.
        /// </summary>
        /// <remarks>
        ///   This is the maximum number of iterations to be performed by the learning algorithm. If
        ///   specified as zero, the algorithm will learn until convergence of the model average
        ///   likelihood respecting the desired limit.
        /// </remarks>
        public int Iterations
        {
            get { return maxIterations; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value",
                                                          "The maximum number of iterations should be positive.");

                maxIterations = value;
            }
        }


        /// <summary>
        ///   Checks if a model has converged given the likelihoods between two iterations
        ///   of the Baum-Welch algorithm and a criteria for convergence.
        /// </summary>
        protected bool HasConverged(double oldLikelihood, double newLikelihood, int currentIteration)
        {
            // Update and verify stop criteria
            if (tolerance > 0)
            {
                // Stopping criteria is likelihood convergence
                if (System.Math.Abs(oldLikelihood - newLikelihood) <= tolerance)
                    return true;

                if (maxIterations > 0)
                {
                    // Maximum iterations should also be respected
                    if (currentIteration >= maxIterations)
                        return true;
                }
            }
            else
            {
                // Stopping criteria is number of iterations
                if (currentIteration == maxIterations)
                    return true;
            }

            // Check if we have reached an invalid state
            if (Double.IsNaN(newLikelihood) || Double.IsInfinity(newLikelihood))
            {
                return true;
            }

            return false;
        }


        /// <summary>
        ///   Runs the Baum-Welch learning algorithm for hidden Markov models.
        /// </summary>
        /// <remarks>
        ///   Learning problem. Given some training observation sequences O = {o1, o2, ..., oK}
        ///   and general structure of HMM (numbers of hidden and visible states), determine
        ///   HMM parameters M = (A, B, pi) that best fit training data. 
        /// </remarks>
        /// <param name="observations">
        ///   The sequences of univariate or multivariate observations used to train the model.
        ///   Can be either of type double[] (for the univariate case) or double[][] for the
        ///   multivariate case.
        /// </param>
        /// <returns>
        ///   The average log-likelihood for the observations after the model has been trained.
        /// </returns>
        protected double Run(Array[] observations)
        {
            // Baum-Welch algorithm.

            // The Baum–Welch algorithm is a particular case of a generalized expectation-maximization
            // (GEM) algorithm. It can compute maximum likelihood estimates and posterior mode estimates
            // for the parameters (transition and emission probabilities) of an HMM, when given only
            // emissions as training data.

            // The algorithm has two steps:
            //  - Calculating the forward probability and the backward probability for each HMM state;
            //  - On the basis of this, determining the frequency of the transition-emission pair values
            //    and dividing it by the probability of the entire string. This amounts to calculating
            //    the expected count of the particular transition-emission pair. Each time a particular
            //    transition is found, the value of the quotient of the transition divided by the probability
            //    of the entire string goes up, and this value can then be made the new value of the transition.


            // Grab model information
            int states = model.States;
            double[,] A = model.Transitions;
            double[] pi = model.Probabilities;


            // Initialize the algorithm
            int N = observations.Length;
            Ksi = new double[N][][,]; // also referred as ksi, psi or xi
            Gamma = new double[N][,];

            for (int i = 0; i < observations.Length; i++)
            {
                int T = observations[i].Length;

                Ksi[i] = new double[T][,];
                Gamma[i] = new double[T,states];

                for (int t = 0; t < T; t++)
                    Ksi[i][t] = new double[states,states];
            }

            int iteration = 1;
            bool stop = false;


            // Initialize the model log-likelihoods
            double newLikelihood = 0;
            double oldLikelihood = Double.MinValue;


            do // Until convergence or max iterations is reached
            {
                // For each sequence in the observations input
                for (int i = 0; i < observations.Length; i++)
                {
                    int T = observations[i].Length;

                    double[,] gamma = Gamma[i];

                    double[,] fwd, bwd;
                    double[] scaling;


                    // 1st step - Calculating the forward probability and the
                    //            backward probability for each HMM state.
                    ComputeForwardBackward(i, out fwd, out bwd, out scaling);


                    // 2nd step - Determining the frequency of the transition-emission pair values
                    //            and dividing it by the probability of the entire string.

                    // Calculate gamma values for next computations
                    for (int t = 0; t < T; t++)
                    {
                        double s = 0;

                        for (int k = 0; k < states; k++)
                            s += gamma[t, k] = fwd[t, k]*bwd[t, k];

                        if (s != 0) // Scaling
                        {
                            for (int k = 0; k < states; k++)
                                gamma[t, k] /= s;
                        }
                    }

                    // Calculate ksi values for next computations
                    ComputeKsi(i, fwd, bwd, scaling);

                    // Compute log-likelihood for the given sequence
                    for (int t = 0; t < scaling.Length; t++)
                        newLikelihood += System.Math.Log(scaling[t]);
                }


                // Average the likelihood for all sequences
                newLikelihood /= observations.Length;


                // Check if the model has converged or if we should stop
                if (!HasConverged(oldLikelihood, newLikelihood, iteration))
                {
                    // We haven't converged yet

                    // 3. Continue with parameter re-estimation
                    iteration++;
                    oldLikelihood = newLikelihood;
                    newLikelihood = 0.0;

                    // 3.1 Re-estimation of initial state probabilities 
                    for (int k = 0; k < states; k++)
                    {
                        double sum = 0;
                        for (int i = 0; i < Gamma.Length; i++)
                            sum += Gamma[i][0, k];
                        pi[k] = sum/N;
                    }

                    // 3.2 Re-estimation of transition probabilities 
                    for (int i = 0; i < states; i++)
                    {
                        for (int j = 0; j < states; j++)
                        {
                            double den = 0, num = 0;

                            for (int k = 0; k < Gamma.Length; k++)
                            {
                                int T = observations[k].Length;

                                double[,] gammak = Gamma[k];
                                double[][,] ksik = Ksi[k];

                                for (int l = 0; l < T - 1; l++)
                                {
                                    num += ksik[l][i, j];
                                    den += gammak[l, i];
                                }
                            }

                            A[i, j] = (den != 0) ? num/den : 0.0;
                        }
                    }

                    // 3.3 Re-estimation of emission probabilities
                    UpdateEmissions();
                }
                else
                {
                    stop = true; // The model has converged.
                }
            } while (!stop);


            // Returns the model average log-likelihood
            return newLikelihood;
        }

        /// <summary>
        ///   Computes the forward and backward probabilities matrices
        ///   for a given observation referenced by its index in the
        ///   input training data.
        /// </summary>
        /// <param name="index">The index of the observation in the input training data.</param>
        /// <param name="fwd">Returns the computed forward probabilities matrix.</param>
        /// <param name="bwd">Returns the computed backward probabilities matrix.</param>
        /// <param name="scaling">Returns the scaling parameters used during calculations.</param>
        protected abstract void ComputeForwardBackward(int index, out double[,] fwd, out double[,] bwd,
                                                       out double[] scaling);

        /// <summary>
        ///   Computes the ksi matrix of probabilities for a given observation
        ///   referenced by its index in the input training data.
        /// </summary>
        /// <param name="index">The index of the observation in the input training data.</param>
        /// <param name="fwd">The matrix of forward probabilities for the observation.</param>
        /// <param name="bwd">The matrix of backward probabilities for the observation.</param>
        /// <param name="scaling">The scaling vector computed in previous calculations.</param>
        protected abstract void ComputeKsi(int index, double[,] fwd, double[,] bwd, double[] scaling);

        /// <summary>
        ///   Updates the emission probability matrix.
        /// </summary>
        /// <remarks>
        ///   Implementations of this method should use the observations
        ///   in the training data and the Gamma probability matrix to
        ///   update the probability distributions of symbol emissions.
        /// </remarks>
        protected abstract void UpdateEmissions();
    }
}