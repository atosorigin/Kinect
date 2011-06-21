using System;

namespace Kinect.Core
{
    public class KinectUserEventArgs : EventArgs
    {
        public KinectUserEventArgs(IUserChangedEvent user)
        {
            User = user;
        }

        public IUserChangedEvent User { get; private set; }
    }
}