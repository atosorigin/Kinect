using System;
using xn;

namespace Kinect.Core.Gestures
{
    public class SelfTouchGesture : GestureBase
    {
        private int _selfTouchCount = 0;

        public SelfTouchGesture()
            : base()
        {
        }

        public event EventHandler<SelfTouchEventArgs> SelfTouchDetected;

        internal SkeletonJoint[] Joints { get; set; }

        protected override string GestureName
        {
            get { return "SelfTouchGesture"; }
        }

        public override void Process(IUserChangedEvent evt)
        {
            this._selfTouchCount++;
            if (this._selfTouchCount > this.HistoryCount)
            {
                this.OnSelfTouchDetected(evt.ID,this.Joints);
                this._selfTouchCount = 0;
            }
        }

        protected virtual void OnSelfTouchDetected(uint userid, SkeletonJoint[] joints)
        {
            var handler = this.SelfTouchDetected;
            if (handler != null)
            {
                handler(this, new SelfTouchEventArgs(userid, joints));
            }
        }
    }
}
