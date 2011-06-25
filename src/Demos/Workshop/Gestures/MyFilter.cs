using System;
using Kinect.Core;
using Kinect.Core.Eventing;

namespace Kinect.Workshop.Gestures
{
    public class MyFilter : Filter<IUserChangedEvent>
    {
        /// <summary>
        /// Gets the filter name
        /// </summary>
        public override string Name
        {
            get { return "FramesPerSecondFilter"; }
        }

        /// <summary>
        /// Processes the specified evt.
        /// </summary>
        /// <param name="evt">The evt.</param>
        public override void Process(IUserChangedEvent evt)
        {
            base.Process(evt);
        }
    }
}