using System;
using System.Text;
using System.Collections.ObjectModel;
using Kinect.Common;
using Kinect.Core.Gestures.Model;
using Kinect.Common;

namespace Kinect.WPF.Models
{
    public class SemaphoreGame : ICopyAble<SemaphoreGame>
    {
        private static object _syncRoot = new object();
        private int _position;

        public event EventHandler Start;
        public event EventHandler Finished;
        public event EventHandler Updated;

        internal ObservableCollection<SemaphoreImage> Semaphores { get; set; }

        public SemaphoreImage Current
        {
            get
            {
                if (this._position >= this.Semaphores.Count)
                {
                    return null;
                }

                return this.Semaphores[this._position];
            }
        }

        public SemaphoreImage Next
        {
            get
            {
                if (this._position + 1 >= this.Semaphores.Count)
                {
                   return null;
                }

                return Semaphores[this._position + 1];
            }
        }

        public SemaphoreGame()
        {
            this._position = 0;
            this.Semaphores = new ObservableCollection<SemaphoreImage>();
        }

        public void SemaphoreDetected(Semaphore detected)
        {
            lock (_syncRoot)
            {
                if (this._position < this.Semaphores.Count && this.Semaphores[_position].Semaphore.Equals(detected))
                {
                    if (this._position == 0)
                    {
                        OnStart();
                    }

                    this._position++;

                    if (this._position == Semaphores.Count)
                    {
                        OnFinished();
                    }

                    this.OnUpdated();
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (SemaphoreImage item in this.Semaphores)
            {
                sb.Append(item.Semaphore.Char);
            }

            return sb.ToString();
        }

        public string GetTodoSentence()
        {
            return ToString().Substring(this._position);
        }

        public SemaphoreGame CreateCopy()
        {
            var game = new SemaphoreGame();
            game.Semaphores = Semaphores.CreateCopy();
            return game;
        }

        protected virtual void OnStart()
        {
            var handler = Start;

            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        protected virtual void OnFinished()
        {
            var handler = this.Finished;

            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        protected virtual void OnUpdated()
        {
            var handler = this.Updated;

            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }
    }
}
