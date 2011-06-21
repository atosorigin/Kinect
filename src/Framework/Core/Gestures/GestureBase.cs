using System;
using Kinect.Core.Eventing;

namespace Kinect.Core.Gestures
{
    /// <summary>
    /// Baseclass for creating gestures
    /// </summary>
    public abstract class GestureBase : IPipeline<IUserChangedEvent>
    {
        /// <summary>
        /// Locking object for syncing multiple threads
        /// </summary>
        protected static object SyncRoot = new object();

        /// <summary>
        /// Gets the name of the gesture.
        /// </summary>
        /// <value>
        /// The name of the gesture.
        /// </value>
        protected abstract string GestureName { get; }

        /// <summary>
        /// Gets or sets the history count.
        /// </summary>
        /// <value>
        /// The history count.
        /// </value>
        internal int HistoryCount { get; set; }

        #region IPipeline<IUserChangedEvent> Members

        /// <summary>
        /// Occurs when [processing event].
        /// </summary>
        public event EventHandler<ProcessEventArgs<IUserChangedEvent>> ProcessingEvent;

        /// <summary>
        /// Occurs when [processed event].
        /// </summary>
        public event EventHandler<ProcessEventArgs<IUserChangedEvent>> ProcessedEvent;

        /// <summary>
        /// Processes the specified evt.
        /// </summary>
        /// <param name="evt">The evt.</param>
        void IPipeline<IUserChangedEvent>.Process(IUserChangedEvent evt)
        {
            OnProcessingEvent(evt);
            Process(evt);
            OnProcessedEvent(evt);
        }

        #endregion

        /// <summary>
        /// Called when [processing event].
        /// </summary>
        /// <param name="evt">The evt.</param>
        protected virtual void OnProcessingEvent(IUserChangedEvent evt)
        {
            EventHandler<ProcessEventArgs<IUserChangedEvent>> handler = ProcessingEvent;
            if (handler != null)
            {
                handler(this, new ProcessEventArgs<IUserChangedEvent>(evt));
            }
        }

        /// <summary>
        /// Called when [processed event].
        /// </summary>
        /// <param name="evt">The evt.</param>
        protected virtual void OnProcessedEvent(IUserChangedEvent evt)
        {
            EventHandler<ProcessEventArgs<IUserChangedEvent>> handler = ProcessedEvent;
            if (handler != null)
            {
                handler(this, new ProcessEventArgs<IUserChangedEvent>(evt));
            }
        }

        /// <summary>
        /// Processes the specified evt.
        /// </summary>
        /// <param name="evt">The evt.</param>
        public abstract void Process(IUserChangedEvent evt);
    }
}