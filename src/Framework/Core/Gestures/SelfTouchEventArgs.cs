using System.Text;
using Microsoft.Research.Kinect.Nui;

namespace Kinect.Core.Gestures
{
    public class SelfTouchEventArgs : GestureEventArgs
    {
        public SelfTouchEventArgs(int userid, params JointID[] joints)
            : base(userid)
        {
            Joints = joints;
        }

        public JointID[] Joints { get; private set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (JointID joint in Joints)
            {
                builder.Append(joint);
                builder.Append("+");
            }

            return builder.Length > 0 ? builder.ToString().Substring(0, builder.Length - 1) : "Unknown joints";
        }
    }
}