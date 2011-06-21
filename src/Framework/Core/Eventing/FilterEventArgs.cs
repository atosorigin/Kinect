using System;

namespace Kinect.Core.Eventing
{
    /// <summary>
    /// The filtereventargs base
    /// </summary>
    public abstract class FilterEventArgs : EventArgs
    {
        /// <summary>
        /// Name of the filter
        /// </summary>
        public abstract string Name { get; }
    }
}