using System;
using System.Windows.Media.Media3D;

namespace Kinect.Plugins.Common
{
    internal class SinglePointEventArgs : EventArgs
    {
        internal SinglePointEventArgs(int userid, Point3D point)
        {
            Point = point;
            UserID = userid;
        }

        internal Point3D Point { get; private set; }
        internal int UserID { get; private set; }
    }
}