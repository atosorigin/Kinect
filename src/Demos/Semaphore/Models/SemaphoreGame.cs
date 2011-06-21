using System;
using System.Collections.ObjectModel;
using System.Text;
using Kinect.Common;

namespace Kinect.Semaphore.Models
{
    public class SemaphoreGame : ICopyAble<SemaphoreGame>
    {
        private static readonly object _syncRoot = new object();
        private int _position;

        public SemaphoreGame()
        {
            _position = 0;
            Semaphores = new ObservableCollection<SemaphoreImage>();
        }

        internal ObservableCollection<SemaphoreImage> Semaphores { get; set; }

        public SemaphoreImage Current
        {
            get
            {
                if (_position >= Semaphores.Count)
                {
                    return null;
                }

                return Semaphores[_position];
            }
        }

        public SemaphoreImage Next
        {
            get
            {
                if (_position + 1 >= Semaphores.Count)
                {
                    return null;
                }

                return Semaphores[_position + 1];
            }
        }

        #region ICopyAble<SemaphoreGame> Members

        public SemaphoreGame CreateCopy()
        {
            var game = new SemaphoreGame();
            game.Semaphores = Semaphores.CreateCopy();
            return game;
        }

        #endregion

        public event EventHandler Start;
        public event EventHandler Finished;
        public event EventHandler Updated;

        public void SemaphoreDetected(Core.Gestures.Model.Semaphore detected)
        {
            lock (_syncRoot)
            {
                if (_position < Semaphores.Count && Semaphores[_position].Semaphore.Equals(detected))
                {
                    if (_position == 0)
                    {
                        OnStart();
                    }

                    _position++;

                    if (_position == Semaphores.Count)
                    {
                        OnFinished();
                    }

                    OnUpdated();
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (SemaphoreImage item in Semaphores)
            {
                sb.Append(item.Semaphore.Char);
            }

            return sb.ToString();
        }

        public string GetTodoSentence()
        {
            return ToString().Substring(_position);
        }

        protected virtual void OnStart()
        {
            EventHandler handler = Start;

            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        protected virtual void OnFinished()
        {
            EventHandler handler = Finished;

            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        protected virtual void OnUpdated()
        {
            EventHandler handler = Updated;

            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }
    }
}