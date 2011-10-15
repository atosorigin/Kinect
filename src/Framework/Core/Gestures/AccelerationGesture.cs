using System;

namespace Kinect.Core.Gestures
{
    /// <summary>
    /// Gestere recognises your right hand as sort of gashandle
    /// </summary>
    public class AccelerationGesture : GestureBase
    {
        /// <summary>
        /// Gets the name of the gesture.
        /// </summary>
        /// <value>
        /// The name of the gesture.
        /// </value>
        protected override string GestureName
        {
            get { return "AccelerationGesture"; }
        }

        /// <summary>
        /// Occurs when [acceleration calculated].
        /// </summary>
        public event EventHandler<AccelerationEventArgs> AccelerationCalculated;

        /// <summary>
        /// Processes the specified evt.
        /// </summary>
        /// <param name="evt">The evt.</param>
        public override void Process(IUserChangedEvent evt)
        {
            if (evt != null)
            {
                double predictedarmlength = evt.ShoulderRight.Y - evt.HipRight.Y -
                                            ((evt.HipRight.Y - evt.KneeRight.Y)/2);
                double maxposition = evt.ShoulderRight.Y + predictedarmlength;
                double minposition = evt.ShoulderRight.Y - predictedarmlength;
                double position = evt.HandRight.Y - evt.ShoulderRight.Y;
                double ratio = (maxposition - minposition)/2;
                double normalPosition = position/ratio;

                //double predictedarmlength = evt.RightHip.Y - evt.RightShoulder.Y - ((evt.RightKnee.Y - evt.RightHip.Y) / 2);
                //double maxposition = evt.RightShoulder.Y - predictedarmlength;
                //double minposition = evt.RightShoulder.Y + predictedarmlength;
                //double position = evt.RightHand.Y + evt.RightShoulder.Y;
                //double ratio = (maxposition + minposition) / 2;
                //double normalPosition = position / ratio;

                OnAccelerationCalculated(evt.Id, -normalPosition);
            }
        }

        /// <summary>
        /// Called when [acceleration calculated].
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="deltaY">The delta Y.</param>
        protected virtual void OnAccelerationCalculated(int userId, double deltaY)
        {
            EventHandler<AccelerationEventArgs> handler = AccelerationCalculated;
            if (handler != null)
            {
                handler(this, new AccelerationEventArgs(userId, deltaY));
            }
        }
    }
}