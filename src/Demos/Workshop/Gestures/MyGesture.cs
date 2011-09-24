using System;
using Kinect.Core;
using Kinect.Core.Gestures;

namespace Kinect.Workshop.Gestures
{
    public class MyGesture : GestureBase
    {
        /// <summary>
        /// The event which needs to get fired when the gesture is detected
        /// </summary>
        public event EventHandler GestureDetected;
        
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
            var handsAreTogether = false;

            //TODO: Check if hands are together

            if (handsAreTogether)
            {
                OnGestureDetected();
            }

            OnProcessedEvent(evt);
        }

        /// <summary>
        /// Thread-safe invoke of the GestureDetected event
        /// </summary>
        private void OnGestureDetected()
        {
            var handler = GestureDetected;
            if (handler != null)
            {
                handler(this,new EventArgs());
            }
        }
    }
}