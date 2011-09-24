using Kinect.Core;
using Kinect.Core.Eventing;

namespace Kinect.Workshop.Gestures
{
    public class MyFilter : Filter<IUserChangedEvent>
    {
        /// <summary>
        /// A variable to count the frames we are filtering
        /// </summary>
        private int _framesCount;

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
            OnFilteredEvent(new MyFilterEventArgs(this, "Filtering of Frame is started!"));
            var filtered = true;
            
            //TODO: implement logic to to filter several frames.

            if (!filtered)
            {
                base.Process(evt);
            }
            else
            {
                OnFilteredEvent(new MyFilterEventArgs(this, "This Frame is filtered!"));
            }
        }
    }
}