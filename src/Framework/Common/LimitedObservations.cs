using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class LimitedObservations<T> : List<T>
    {
        private static object _syncRoot = new object();

        private int _limit = -1;

        public int Limit
        {
            get { return this._limit; }
            set { this._limit = value; }
        }


        public LimitedObservations(int limit)
            : base(limit)
        {
            this.Limit = limit;
        }

        public new void InsertObservation(T item)
        {
            lock (_syncRoot)
            {
                if (this.Count >= this.Limit)
                {
                    this.RemoveAt(0);

                }
                base.Add(item);
            }
        }
    }

}
