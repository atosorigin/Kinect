using System;

namespace Kinect.Core
{
    /// <summary>
    /// KinectMessageEventArgs
    /// </summary>
    public class KinectMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }
    }
}