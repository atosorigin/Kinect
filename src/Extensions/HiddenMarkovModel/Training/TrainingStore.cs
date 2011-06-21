using System;
using System.Collections.Generic;

namespace Accord.Statistics.Models.Markov.Training
{
    public class TrainingStore
    {
        public TrainingStore()
        {
            Id = Guid.NewGuid();
            Items = new List<TrainingItem>();
        }

        public List<TrainingItem> Items { get; set; }
        public Guid Id { get; private set; }
    }
}