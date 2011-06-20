using System.Text;
using xn;

namespace Kinect.Core.Gestures
{
    public class SelfTouchEventArgs : GestureEventArgs
    {
        public SelfTouchEventArgs(uint userid, params SkeletonJoint[] joints) : base(userid)
        {
            Joints = joints;
        }

        public SkeletonJoint[] Joints { get; private set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (SkeletonJoint joint in Joints)
            {
                builder.Append(joint);
                builder.Append("+");
            }

            return builder.Length > 0 ? builder.ToString().Substring(0, builder.Length - 1) : "Unknown joints";
        }
    }
}