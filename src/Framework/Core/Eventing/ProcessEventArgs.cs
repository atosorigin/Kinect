using System;

namespace Kinect.Core.Eventing
{
    /// <summary>
    /// The processEventArgs of the event being processed
    /// </summary>
    /// <typeparam name="T">The event type being processed</typeparam>
    public class ProcessEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="evt">The evt.</param>
        public ProcessEventArgs(T evt)
        {
            Event = evt;
        }

        /// <summary>
        /// Gets the event being processed
        /// </summary>
        public T Event { get; private set; }
    }
}