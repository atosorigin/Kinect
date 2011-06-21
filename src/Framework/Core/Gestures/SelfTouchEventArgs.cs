using System.Text;
using Microsoft.Research.Kinect.Nui;

namespace Kinect.Core.Gestures
{
    public class SelfTouchEventArgs : GestureEventArgs
    {
        public JointID[] Joints { get; private set; }

        public SelfTouchEventArgs(int userid, params JointID[] joints)
            : base(userid)
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
