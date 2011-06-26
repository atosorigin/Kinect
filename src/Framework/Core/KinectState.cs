using System;

namespace Kinect.Core
{
    /// <summary>
    /// Kinect states
    /// </summary>
    [Flags]
    public enum KinectState
    {
        /// <summary>
        /// Kinect context is open
        /// </summary>
        ContextOpen,

        /// <summary>
        /// Kinect is initializing
        /// </summary>
        Initializing,

        /// <summary>
        /// Kinect has failed
        /// </summary>
        Failed,

        /// <summary>
        /// Kinect is stopped
        /// </summary>
        Stopped,

        /// <summary>
        /// Kinect is running
        /// </summary>
        Running
    }
}