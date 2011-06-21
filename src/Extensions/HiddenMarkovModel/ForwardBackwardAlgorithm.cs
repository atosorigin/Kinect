// Accord Statistics Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//

using Accord.Statistics.Distributions;

namespace Accord.Statistics.Models.Markov
{
    /// <summary>
    ///   Forward-Backward algorithms for Hidden Markov Models.
    /// </summary>
    /// 
    public static class ForwardBackwardAlgorithm
    {
        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        public static double[,] Forward(HiddenMarkovModel model, int[] observations, out double[] scaling)
        {
            int states = model.States;
            double[,] A = model.Transitions;
            double[,] B = model.Emissions;
            double[] pi = model.Probabilities;

            int T = observations.Length;
            var fwd = new double[T,states];
            scaling = new double[T];


            // 1. Initialization
            for (int i = 0; i < states; i++)
                scaling[0] += fwd[0, i] = pi[i]*B[i, observations[0]];

            if (scaling[0] != 0) // Scaling
            {
                for (int i = 0; i < states; i++)
                    fwd[0, i] /= scaling[0];
            }


            // 2. Induction
            for (int t = 1; t < T; t++)
            {
                int obs = observations[t];

                for (int i = 0; i < states; i++)
                {
                    double sum = 0.0;
                    for (int j = 0; j < states; j++)
                        sum += fwd[t - 1, j]*A[j, i];
                    fwd[t, i] = sum*B[i, obs];

                    scaling[t] += fwd[t, i]; // scaling coefficient
                }

                if (scaling[t] != 0) // Scaling
                {
                    for (int i = 0; i < states; i++)
                        fwd[t, i] /= scaling[t];
                }
            }

            return fwd;
        }

        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        public static double[,] Forward(HiddenMarkovModel model, int[] observations, out double logLikelihood)
        {
            double[] coefficients;
            double[,] fwd = Forward(model, observations, out coefficients);

            logLikelihood = 0;
            for (int i = 0; i < coefficients.Length; i++)
                logLikelihood += System.Math.Log(coefficients[i]);

            return fwd;
        }


        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        public static double[,] Forward(ContinuousHiddenMarkovModel model, double[][] observations, out double[] scaling)
        {
            int states = model.States;
            double[,] A = model.Transitions;
            IDistribution[] B = model.Emissions;
            double[] pi = model.Probabilities;

            int T = observations.Length;
            var fwd = new double[T,states];
            scaling = new double[T];


            // 1. Initialization
            for (int i = 0; i < states; i++)
                scaling[0] += fwd[0, i] = pi[i]*B[i].ProbabilityFunction(observations[0]);

            if (scaling[0] != 0) // Scaling
            {
                for (int i = 0; i < states; i++)
                    fwd[0, i] /= scaling[0];
            }


            // 2. Induction
            for (int t = 1; t < T; t++)
            {
                double[] obs = observations[t];

                for (int i = 0; i < states; i++)
                {
                    double sum = 0.0;
                    for (int j = 0; j < states; j++)
                        sum += fwd[t - 1, j]*A[j, i];
                    fwd[t, i] = sum*B[i].ProbabilityFunction(obs);

                    scaling[t] += fwd[t, i]; // scaling coefficient
                }

                if (scaling[t] != 0) // Scaling
                {
                    for (int i = 0; i < states; i++)
                        fwd[t, i] /= scaling[t];
                }
            }

            return fwd;
        }

        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        public static double[,] Forward(ContinuousHiddenMarkovModel model, double[][] observations,
                                        out double logLikelihood)
        {
            double[] coefficients;
            double[,] fwd = Forward(model, observations, out coefficients);

            logLikelihood = 0;
            for (int i = 0; i < coefficients.Length; i++)
                logLikelihood += System.Math.Log(coefficients[i]);

            return fwd;
        }


        /// <summary>
        ///   Computes Backward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        public static double[,] Backward(HiddenMarkovModel model, int[] observations, double[] scaling)
        {
            int states = model.States;
            double[,] A = model.Transitions;
            double[,] B = model.Emissions;
            double[] pi = model.Probabilities;

            int T = observations.Length;
            var bwd = new double[T,states];

            // For backward variables, we use the same scale factors
            //   for each time t as were used for forward variables.

            // 1. Initialization
            for (int i = 0; i < states; i++)
                bwd[T - 1, i] = 1.0/scaling[T - 1];

            // 2. Induction
            for (int t = T - 2; t >= 0; t--)
            {
                for (int i = 0; i < states; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < states; j++)
                        sum += A[i, j]*B[j, observations[t + 1]]*bwd[t + 1, j];
                    bwd[t, i] += sum/scaling[t];
                }
            }

            return bwd;
        }

        /// <summary>
        ///   Computes Backward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        public static double[,] Backward(ContinuousHiddenMarkovModel model, double[][] observations, double[] scaling)
        {
            int states = model.States;
            double[,] A = model.Transitions;
            IDistribution[] B = model.Emissions;
            double[] pi = model.Probabilities;

            int T = observations.Length;
            var bwd = new double[T,states];

            // For backward variables, we use the same scale factors
            //   for each time t as were used for forward variables.

            // 1. Initialization
            for (int i = 0; i < states; i++)
                bwd[T - 1, i] = 1.0/scaling[T - 1];

            // 2. Induction
            for (int t = T - 2; t >= 0; t--)
            {
                for (int i = 0; i < states; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < states; j++)
                        sum += A[i, j]*B[j].ProbabilityFunction(observations[t + 1])*bwd[t + 1, j];
                    bwd[t, i] += sum/scaling[t];
                }
            }

            return bwd;
        }
    }
}