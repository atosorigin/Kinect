using System.Collections.Generic;

namespace Kinect.Common
{
    public class LimitedObservations<T> : List<T>
    {
        private static readonly object _syncRoot = new object();

        private int _limit = -1;


        public LimitedObservations(int limit)
            : base(limit)
        {
            Limit = limit;
        }

        public int Limit
        {
            get { return _limit; }
            set { _limit = value; }
        }

        public void InsertObservation(T item)
        {
            lock (_syncRoot)
            {
                if (Count >= Limit)
                {
                    RemoveAt(0);
                }
                base.Add(item);
            }
        }
    }
}