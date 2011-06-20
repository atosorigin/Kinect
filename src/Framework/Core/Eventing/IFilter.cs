using System;

namespace Kinect.Core.Eventing
{
    /// <summary>
    /// Interface for the filter pipeline component
    /// </summary>
    /// <typeparam name="T">The event type to be processed</typeparam>
    public interface IFilter<T> : IEventPublisher<T>, IPipeline<T>
    {
        /// <summary>
        /// Gets the name of the filter
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Event will notify if event is filtering
        /// </summary>
        event EventHandler<FilterEventArgs> Filtering;

        /// <summary>
        /// Event will notify if event is filtered
        /// </summary>
        event EventHandler<FilterEventArgs> Filtered;
    }
}