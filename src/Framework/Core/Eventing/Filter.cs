using System;
using System.Collections.Generic;
using System.Linq;

namespace Kinect.Core.Eventing
{
    /// <summary>
    /// Filter pipeline component baseclass.
    /// </summary>
    /// <typeparam name="T">The event which gonna be filtered</typeparam>
    public abstract class Filter<T> : IFilter<T>
    {
        //TODO: Set back to private after implementing the factory
        internal List<IPipeline<T>> _pipelines = new List<IPipeline<T>>();

        #region IFilter<T> Members

        /// <summary>
        /// Event will notify if the event is processing
        /// </summary>
        public event EventHandler<ProcessEventArgs<T>> ProcessingEvent;

        /// <summary>
        /// Event will notify if the event is processed
        /// </summary>
        public event EventHandler<ProcessEventArgs<T>> ProcessedEvent;

        /// <summary>
        /// Event will notify if the event is filtering
        /// </summary>
        public event EventHandler<FilterEventArgs> Filtering;

        /// <summary>
        /// Event will notify if the event is filtered
        /// </summary>
        public event EventHandler<FilterEventArgs> Filtered;

        /// <summary>
        /// Gets the filter name
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Registers a pipeline component to the filter
        /// </summary>
        /// <param name="pipeline">The pipeline to add</param>
        public virtual void AttachPipeline(IPipeline<T> pipeline)
        {
            if (pipeline != null)
            {
                _pipelines.Add(pipeline);
            }
        }

        /// <summary>
        /// Unregisters a pipeline component from the filter
        /// </summary>
        /// <param name="pipeline">The pipeline to remove</param>
        public virtual void DetachPipeline(IPipeline<T> pipeline)
        {
            if (pipeline != null)
            {
                _pipelines.Remove(pipeline);
            }
        }

        /// <summary>
        /// Processes the event
        /// </summary>
        /// <param name="evt">The event to process</param>
        void IPipeline<T>.Process(T evt)
        {
            OnProcessingEvent(evt);
            Process(evt);
            OnProcessedEvent(evt);
        }

        #endregion

        /// <summary>
        /// Processes the event
        /// </summary>
        /// <param name="evt">The event to process</param>
        public virtual void Process(T evt)
        {
            _pipelines.AsParallel().ForAll(p => p.Process(evt));
        }

        /// <summary>
        /// Triggers the ProcessingEvent event
        /// </summary>
        /// <param name="evt">The event being processed</param>
        protected virtual void OnProcessingEvent(T evt)
        {
            EventHandler<ProcessEventArgs<T>> handler = ProcessingEvent;
            if (handler != null)
            {
                handler(this, new ProcessEventArgs<T>(evt));
            }
        }

        /// <summary>
        /// Triggers the ProcessedEvent event
        /// </summary>
        /// <param name="evt">The processed event</param>
        protected virtual void OnProcessedEvent(T evt)
        {
            EventHandler<ProcessEventArgs<T>> handler = ProcessedEvent;
            if (handler != null)
            {
                handler(this, new ProcessEventArgs<T>(evt));
            }
        }

        /// <summary>
        /// Triggers the FilteringEvent event
        /// </summary>
        /// <param name="filterEventArgs">The event being filtered</param>
        protected virtual void OnFilteringEvent(FilterEventArgs filterEventArgs)
        {
            EventHandler<FilterEventArgs> handler = Filtering;
            if (handler != null)
            {
                handler(this, filterEventArgs);
            }
        }

        /// <summary>
        /// Triggers the FilteredEvent event
        /// </summary>
        /// <param name="filterEventArgs">The filtered event</param>
        protected virtual void OnFilteredEvent(FilterEventArgs filterEventArgs)
        {
            EventHandler<FilterEventArgs> handler = Filtered;
            if (handler != null)
            {
                handler(this, filterEventArgs);
            }
        }
    }
}