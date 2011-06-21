using Kinect.Core.Eventing;

namespace Kinect.Core.Filters
{
    /// <summary>
    /// EventArgs if frames are filtered out
    /// </summary>
    public class FramesFilterEventArgs : FilterEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FramesFilterEventArgs"/> class.
        /// </summary>
        /// <param name="currentFps">The current FPS.</param>
        /// <param name="fpsToFilter">The FPS to filter.</param>
        /// <param name="currentFrame">The current frame.</param>
        public FramesFilterEventArgs(int currentFps, int fpsToFilter, int currentFrame)
        {
            CurrentFps = currentFps;
            FpsToFilter = fpsToFilter;
            CurrentFrame = currentFrame;
        }

        /// <summary>
        /// Name of the filter
        /// </summary>
        public override string Name
        {
            get { return "FPSFilterEventArgs"; }
        }

        /// <summary>
        /// Gets the current FPS.
        /// </summary>
        public int CurrentFps { get; private set; }

        /// <summary>
        /// Gets the FPS to filter.
        /// </summary>
        public int FpsToFilter { get; private set; }

        /// <summary>
        /// Gets the current frame.
        /// </summary>
        public int CurrentFrame { get; private set; }
    }
}