using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;
using Accord.Statistics.Models.Markov.Topology;
using Accord.Statistics.Models.Markov.Training;

namespace Kinect.GestureDetection.Models
{
    internal class GestureDetection
    {
        private static object _syncRoot = new object();
        private readonly int _iterations;
        private readonly double _tolerance = 0.0001;
        private readonly TrainingStore _trainingstore;
        public int ObservationLength = 6;
        private ContinuousSequenceClassifier _classifier;

        //Configuration parameters


        public GestureDetection()
        {
            _trainingstore = new TrainingStore();
            AddTrainingData(new List<double> {1, 2, 3, 4, 5, 6}, "Right circle", 1);
            AddTrainingData(new List<double> {6, 5, 4, 3, 2, 1}, "Left circle", 1);
        }

        public void TrainHiddenMarkovModel()
        {
            //Trainigset
            double[][] data = (from item in _trainingstore.Items select item.Data.ToArray()).ToArray();
            var indexes = new int[data.Count()];
            for (int i = 0; i < data.Count(); i++)
            {
                indexes[i] = i;
            }

            //Classifier
            var density = new NormalDistribution(1);
            _classifier = new ContinuousSequenceClassifier(_trainingstore.Items.Count, new Ergodic(ObservationLength),
                                                           density);

            //Learn model
            var teacher = new SequenceClassifierLearning(_classifier,
                                                         modelIndex =>
                                                         new ContinuousBaumWelchLearning(_classifier.Models[modelIndex])
                                                             {
                                                                 Tolerance = _tolerance,
                                                                 Iterations = _iterations
                                                             }
                );

            teacher.Run(data, indexes);

            //Determine max likelihoods for all training items
            double maxLikelihood;
            foreach (TrainingItem item in _trainingstore.Items)
            {
                _classifier.Compute(item.Data.ToArray(), out maxLikelihood);
                item.MaxLikelihood = maxLikelihood;
            }
        }

        public void AddTrainingData(List<double> observations, string name, double treshhold)
        {
            var _newTrainingItem = new TrainingItem(name, treshhold)
                                       {
                                           Data = observations
                                       };
            _trainingstore.Items.Add(_newTrainingItem);
            TrainHiddenMarkovModel();
        }

        public string ProcessObservation(List<double> observations)
        {
            double likelihood;
            int outcome = _classifier.Compute(observations.ToArray(), out likelihood);
            if (likelihood >= _trainingstore.Items[outcome].MaxLikelihood*_trainingstore.Items[outcome].TreshHold)
            {
                return _trainingstore.Items[outcome].Name;
            }
            else
            {
                return String.Empty;
            }
        }
    }
}