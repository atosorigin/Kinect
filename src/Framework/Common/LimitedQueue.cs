using System.Collections.Generic;

namespace Kinect.Common
{
    public class LimitedQueue<T> : Queue<T>
    {
        private static object _syncRoot = new object();

        private int _limit = -1;

        public int Limit
        {
            get { return this._limit; }
            set { this._limit = value; }
        }

        public LimitedQueue(int limit)
            : base(limit)
        {
            this.Limit = limit;
        }

        public new void Enqueue(T item)
        {
            lock (_syncRoot)
            {
                if (this.Count >= this.Limit)
                {
                    this.Dequeue();
                }

                base.Enqueue(item);
            }
        }
    }
}
