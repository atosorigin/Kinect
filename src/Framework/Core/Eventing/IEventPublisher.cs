namespace Kinect.Core.Eventing
{
    /// <summary>
    /// Interface for the eventpublisher pipeline component
    /// </summary>
    public interface IEventPublisher : IEventPublisher<object>
    {
    }

    /// <summary>
    /// Interface for the eventpublisher pipeline component
    /// </summary>
    /// <typeparam name="T">The event type to be published</typeparam>
    public interface IEventPublisher<T>
    {
        /// <summary>
        /// Attaches the pipeline.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        void AttachPipeline(IPipeline<T> pipeline);

        /// <summary>
        /// Detaches the pipeline.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        void DetachPipeline(IPipeline<T> pipeline);
    }
}