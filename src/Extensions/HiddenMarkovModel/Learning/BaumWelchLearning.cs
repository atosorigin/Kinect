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
    ///   Baum-Welch learning algorithm for discrete density Hidden Markov Models.
    /// </summary>
    /// 
    public class BaumWelchLearning : BaumWelchLearningBase, IUnsupervisedLearning
    {
        private readonly HiddenMarkovModel model;

        private int[][] discreteObservations;


        /// <summary>
        ///   Creates a new instance of the Baum-Welch learning algorithm.
        /// </summary>
        public BaumWelchLearning(HiddenMarkovModel model)
            : base(model)
        {
            this.model = model;
        }

        #region IUnsupervisedLearning Members

        /// <summary>
        ///   Runs the Baum-Welch learning algorithm for hidden Markov models.
        /// </summary>
        /// <param name="observations">The sequences of univariate or multivariate observations used to train the model.
        ///   Can be either of type double[] (for the univariate case) or double[][] for the
        ///   multivariate case.</param>
        /// <returns>
        ///   The average log-likelihood for the observations after the model has been trained.
        /// </returns>
        /// <remarks>
        ///   Learning problem. Given some training observation sequences O = {o1, o2, ..., oK}
        ///   and general structure of HMM (numbers of hidden and visible states), determine
        ///   HMM parameters M = (A, B, pi) that best fit training data.
        /// </remarks>
        double IUnsupervisedLearning.Run(Array[] observations)
        {
            return Run(observations as int[][]);
        }

        #endregion

        /// <summary>
        ///   Runs the Baum-Welch learning algorithm for hidden Markov models.
        /// </summary>
        /// <param name="observations">An array of observation sequences to be used to train the model.</param>
        /// <returns>
        ///   The average log-likelihood for the observations after the model has been trained.
        /// </returns>
        /// <remarks>
        ///   Learning problem. Given some training observation sequences O = {o1, o2, ..., oK}
        ///   and general structure of HMM (numbers of hidden and visible states), determine
        ///   HMM parameters M = (A, B, pi) that best fit training data.
        /// </remarks>
        public double Run(params int[][] observations)
        {
            discreteObservations = observations;

            return base.Run(discreteObservations);
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
        protected override void ComputeForwardBackward(int index, out double[,] fwd, out double[,] bwd,
                                                       out double[] scaling)
        {
            fwd = ForwardBackwardAlgorithm.Forward(model, discreteObservations[index], out scaling);
            bwd = ForwardBackwardAlgorithm.Backward(model, discreteObservations[index], scaling);
        }

        /// <summary>
        ///   Updates the emission probability matrix.
        /// </summary>
        /// <remarks>
        ///   Implementations of this method should use the observations
        ///   in the training data and the Gamma probability matrix to
        ///   update the probability distributions of symbol emissions.
        /// </remarks>
        protected override void UpdateEmissions()
        {
            double[,] B = model.Emissions;
            int states = model.States;
            int symbols = model.Symbols;

            for (int i = 0; i < states; i++)
            {
                for (int j = 0; j < symbols; j++)
                {
                    double sum = 0, num = 0;

                    for (int k = 0; k < discreteObservations.Length; k++)
                    {
                        int T = discreteObservations[k].Length;
                        double[,] gammak = Gamma[k];

                        for (int l = 0; l < T; l++)
                        {
                            if (discreteObservations[k][l] == j)
                                num += gammak[l, i];

                            sum += gammak[l, i];
                        }
                    }

                    // avoid locking a parameter in zero.
                    if (num == 0) num = 1e-10;

                    B[i, j] = (sum != 0) ? num/sum : num;
                }
            }
        }

        /// <summary>
        ///   Computes the ksi matrix of probabilities for a given observation
        ///   referenced by its index in the input training data.
        /// </summary>
        /// <param name="index">The index of the observation in the input training data.</param>
        /// <param name="fwd">The matrix of forward probabilities for the observation.</param>
        /// <param name="bwd">The matrix of backward probabilities for the observation.</param>
        /// <param name="scaling">The scaling vector computed in previous calculations.</param>
        protected override void ComputeKsi(int index, double[,] fwd, double[,] bwd, double[] scaling)
        {
            int states = model.States;
            double[,] A = model.Transitions;
            double[,] B = model.Emissions;

            int[] sequence = discreteObservations[index];
            double[][,] ksi = Ksi[index];


            for (int t = 0; t < sequence.Length - 1; t++)
            {
                double s = 0;
                double c = scaling[t + 1];
                int x = sequence[t + 1];
                double[,] ksit = ksi[t];

                for (int k = 0; k < states; k++)
                    for (int l = 0; l < states; l++)
                        s += ksit[k, l] = c*fwd[t, k]*A[k, l]*bwd[t + 1, l]*B[l, x];

                if (s != 0) // Scaling
                {
                    for (int k = 0; k < states; k++)
                        for (int l = 0; l < states; l++)
                            ksit[k, l] /= s;
                }
            }
        }
    }
}