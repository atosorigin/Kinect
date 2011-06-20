using Kinect.Core.Gestures.Model;

namespace Kinect.Core
{
    public class KinectSemaphoreGestureEventArgs : KinectGestureEventArgs
    {
        public KinectSemaphoreGestureEventArgs(IUserChangedEvent userEvent, Semaphore semafoor)
            : base(userEvent)
        {
            Semafoor = semafoor;
        }

        public Semaphore Semafoor { get; private set; }
    }
}