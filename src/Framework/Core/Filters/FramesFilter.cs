using Kinect.Core.Eventing;

namespace Kinect.Core.Filters
{
    /// <summary>
    /// Filter for filtering some frames
    /// </summary>
    public class FramesFilter : Filter<IUserChangedEvent>
    {
        private readonly int _FpxToFilter = 30;
        private int _currentFps = 30;
        private int _currentFrame;
        private int _filter = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="FramesFilter"/> class.
        /// </summary>
        /// <param name="FPSToFilter">The FPS to filter.</param>
        public FramesFilter(int FPSToFilter)
        {
            _FpxToFilter = FPSToFilter;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get { return "FramesFilter"; }
        }

        /// <summary>
        /// Processes the specified evt.
        /// </summary>
        /// <param name="evt">The evt.</param>
        public override void Process(IUserChangedEvent evt)
        {
            _currentFrame++;

            OnFilteringEvent(new FramesFilterEventArgs(_currentFps, _FpxToFilter, _currentFrame));

            _filter = _currentFps/_FpxToFilter;

            if (_filter < 1)
            {
                _filter = 1;
            }

            if (_currentFrame%_filter == 0)
            {
                base.Process(evt);
            }

            OnFilteredEvent(new FramesFilterEventArgs(_currentFps, _FpxToFilter, _currentFrame));
        }
    }
}