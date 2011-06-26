using System;
using Kinect.Core;
using Kinect.Core.Gestures;

namespace Kinect.Workshop.Gestures
{
    public class MyGesture : GestureBase
    {
        /// <summary>
        /// Gets the name of the gesture.
        /// </summary>
        /// <value>
        /// The name of the gesture.
        /// </value>
        protected override string GestureName
        {
            get { return "HandAreTogetherGesture"; }
        }

        /// <summary>
        /// Processes the specified evt.
        /// </summary>
        /// <param name="evt">The evt.</param>
        public override void Process(IUserChangedEvent evt)
        {
            OnProcessingEvent(evt);
            bool handAreTogether = false;
            //TODO: Check if hands are together

            if (handAreTogether)
            {
                OnProcessedEvent(evt);
            }
        }
    }
}