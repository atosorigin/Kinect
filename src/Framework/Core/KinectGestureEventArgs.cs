namespace Kinect.Core
{
    public class KinectGestureEventArgs : KinectUserEventArgs
    {
        public KinectGestureEventArgs(IUserChangedEvent user)
            : base(user)
        {
        }
    }
}