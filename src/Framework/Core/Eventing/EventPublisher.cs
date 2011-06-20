using System.Collections.Generic;
using System.Linq;

namespace Kinect.Core.Eventing
{
    /// <summary>
    /// Eventpublisher baseclass. The EventPublisher is always the first pipeline component
    /// </summary>
    /// <typeparam name="T">Event to publish</typeparam>
    public abstract class EventPublisher<T> : IEventPublisher<T>
    {
        //TODO: Set back to private after implementing the factory
        internal List<IPipeline<T>> _pipelines = new List<IPipeline<T>>();

        #region IEventPublisher<T> Members

        /// <summary>
        /// Registers a new pipeline component to the eventpublisher
        /// </summary>
        /// <param name="pipeline">The pipeline to add</param>
        public void AttachPipeline(IPipeline<T> pipeline)
        {
            if (pipeline != null)
            {
                _pipelines.Add(pipeline);
            }
        }

        /// <summary>
        /// Unregisters a existing pipeline component from the eventpublisher
        /// </summary>
        /// <param name="pipeline">The pipline to remove</param>
        public void DetachPipeline(IPipeline<T> pipeline)
        {
            if (pipeline != null)
            {
                _pipelines.Remove(pipeline);
            }
        }

        #endregion

        /// <summary>
        /// Process the event to all pipelines
        /// </summary>
        /// <param name="evt">The event to publish</param>
        protected void PublishEvent(T evt)
        {
            _pipelines.AsParallel().ForAll(p => p.Process(evt));
        }
    }
}