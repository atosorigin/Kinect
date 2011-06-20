using System;
using xn;

namespace Kinect.Core.Gestures
{
    public class SelfTouchGesture : GestureBase
    {
        private int _selfTouchCount;

        internal SkeletonJoint[] Joints { get; set; }

        protected override string GestureName
        {
            get { return "SelfTouchGesture"; }
        }

        public event EventHandler<SelfTouchEventArgs> SelfTouchDetected;

        public override void Process(IUserChangedEvent evt)
        {
            _selfTouchCount++;
            if (_selfTouchCount > HistoryCount)
            {
                OnSelfTouchDetected(evt.ID, Joints);
                _selfTouchCount = 0;
            }
        }

        protected virtual void OnSelfTouchDetected(uint userid, SkeletonJoint[] joints)
        {
            EventHandler<SelfTouchEventArgs> handler = SelfTouchDetected;
            if (handler != null)
            {
                handler(this, new SelfTouchEventArgs(userid, joints));
            }
        }
    }
}