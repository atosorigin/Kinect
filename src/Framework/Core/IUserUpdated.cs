using System;
using Kinect.Core.Eventing;

namespace Kinect.Core
{
    /// <summary>
    /// Interface for updated user
    /// </summary>
    public interface IUserUpdated
    {
        /// <summary>
        /// Event will notify if user is updated
        /// </summary>
        event EventHandler<ProcessEventArgs<IUserChangedEvent>> Updated;
    }
}