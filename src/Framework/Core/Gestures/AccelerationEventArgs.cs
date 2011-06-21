namespace Kinect.Core.Gestures
{
    /// <summary>
    /// Eventargs for the AccelerationGesture
    /// </summary>
    public class AccelerationEventArgs : GestureEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccelerationEventArgs"/> class.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="deltaY">The delta Y.</param>
        public AccelerationEventArgs(int userId, double deltaY)
            : base(userId)
        {
            DeltaY = deltaY;
        }

        /// <summary>
        /// Gets the delta Y.
        /// </summary>
        public double DeltaY { get; private set; }
    }
}