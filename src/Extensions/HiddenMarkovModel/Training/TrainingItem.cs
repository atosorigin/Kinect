using System;
using System.Collections.Generic;

namespace Accord.Statistics.Models.Markov.Training
{
    public class TrainingItem
    {
        public TrainingItem(string name, double treshhold)
        {
            Id = Guid.NewGuid();
            Name = name;
            Data = new List<Double>();
            TreshHold = treshhold;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public List<Double> Data { get; set; }
        public double MaxLikelihood { get; set; }
        public double TreshHold { get; set; }
    }
}