using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Kinect.Plugins.Common
{
    internal class SinglePointEventArgs : EventArgs
    {
        internal Point3D Point { get; private set; }
        internal int UserID { get; private set; }

        internal SinglePointEventArgs(int userid, Point3D point)
        {
            Point = point;
            UserID = userid;
        }
    }
}
