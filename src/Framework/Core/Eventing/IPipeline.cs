using System;

namespace Kinect.Core.Eventing
{
    /// <summary>
    /// Interface for the pipeline component
    /// </summary>
    public interface IPipeline : IPipeline<object>
    {
    }

    /// <summary>
    /// Interface for pipeline component
    /// </summary>
    /// <typeparam name="T">The event type to be processed</typeparam>
    public interface IPipeline<T>
    {
        /// <summary>
        /// Event will notify if the event is processing
        /// </summary>
        event EventHandler<ProcessEventArgs<T>> ProcessingEvent;

        /// <summary>
        /// Event will notify if the event is being processed
        /// </summary>
        event EventHandler<ProcessEventArgs<T>> ProcessedEvent;

        /// <summary>
        /// Process the event
        /// </summary>
        /// <param name="evt">Event to be processed</param>
        void Process(T evt);
    }
}