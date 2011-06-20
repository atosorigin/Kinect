using System.Collections.Generic;

namespace Kinect.Common
{
    public class LimitedQueue<T> : Queue<T>
    {
        private static readonly object _syncRoot = new object();

        private int _limit = -1;

        public LimitedQueue(int limit)
            : base(limit)
        {
            Limit = limit;
        }

        public int Limit
        {
            get { return _limit; }
            set { _limit = value; }
        }

        public new void Enqueue(T item)
        {
            lock (_syncRoot)
            {
                if (Count >= Limit)
                {
                    Dequeue();
                }

                base.Enqueue(item);
            }
        }
    }
}