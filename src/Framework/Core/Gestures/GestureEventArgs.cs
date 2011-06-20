using System;

namespace Kinect.Core.Gestures
{
    /// <summary>
    /// GestureEventArgs
    /// </summary>
    public class GestureEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GestureEventArgs"/> class.
        /// </summary>
        /// <param name="userid">The userid.</param>
        public GestureEventArgs(uint userid)
        {
            UserID = userid;
        }

        /// <summary>
        /// Gets the user ID.
        /// </summary>
        public uint UserID { get; private set; }
    }
}