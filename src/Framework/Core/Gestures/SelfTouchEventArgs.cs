using System;
using System.Text;
using xn;

namespace Kinect.Core.Gestures
{
    public class SelfTouchEventArgs : GestureEventArgs
    {
        public SkeletonJoint[] Joints { get; private set; }

        public SelfTouchEventArgs(uint userid, params SkeletonJoint[] joints) : base(userid)
        {
            this.Joints = joints;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var joint in this.Joints)
            {
                builder.Append(joint);
                builder.Append("+");
            }

            return builder.Length > 0 ? builder.ToString().Substring(0, builder.Length - 1) : "Unknown joints";
        }
    }
}
