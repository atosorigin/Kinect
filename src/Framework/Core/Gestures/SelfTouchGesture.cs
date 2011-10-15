using System;
using Microsoft.Research.Kinect.Nui;

namespace Kinect.Core.Gestures
{
    public class SelfTouchGesture : GestureBase
    {
        private int _selfTouchCount;

        internal JointID[] Joints { get; set; }

        protected override string GestureName
        {
            get { return "SelfTouchGesture"; }
        }

        public SelfTouchGesture()
        {
            
        }

        public SelfTouchGesture(int historyCount)
        {
            HistoryCount = historyCount;
        }

        public event EventHandler<SelfTouchEventArgs> SelfTouchDetected;

        public override void Process(IUserChangedEvent evt)
        {
            _selfTouchCount++;
            if (_selfTouchCount > HistoryCount)
            {
                OnSelfTouchDetected(evt.Id, Joints);
                _selfTouchCount = 0;
            }
        }

        protected virtual void OnSelfTouchDetected(int userid, JointID[] joints)
        {
            EventHandler<SelfTouchEventArgs> handler = SelfTouchDetected;
            if (handler != null)
            {
                handler(this, new SelfTouchEventArgs(userid, joints));
            }
        }
    }
}