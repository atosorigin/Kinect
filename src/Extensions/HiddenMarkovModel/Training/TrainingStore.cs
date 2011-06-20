using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accord.Statistics.Models.Markov.Training
{
    public class TrainingStore
    {
        public List<TrainingItem> Items { get; set; }
        public Guid Id { get; private set; }

        public TrainingStore()
        {
            Id = Guid.NewGuid();
            Items = new List<TrainingItem>();
        }
    }
}
