using Kinect.Core.Gestures.Model;

namespace Kinect.Core
{
    public class KinectSemaphoreGestureEventArgs : KinectGestureEventArgs
    {
        public Semaphore Semafoor { get; private set; }

        public KinectSemaphoreGestureEventArgs(IUserChangedEvent userEvent, Semaphore semafoor)
            : base(userEvent)
        {
            this.Semafoor = semafoor;
        }
    }
}
