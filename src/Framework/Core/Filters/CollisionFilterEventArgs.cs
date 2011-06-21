using System.Windows.Media.Media3D;
using Kinect.Core.Eventing;

namespace Kinect.Core.Filters
{
    public class CollisionFilterEventArgs : FilterEventArgs
    {
        private const string _name = "MarginFilterEventArgs";

        public CollisionFilterEventArgs(Point3D[] marginData)
        {
            MarginData = marginData;
        }

        public Point3D[] MarginData { get; private set; }

        public override string Name
        {
            get { return _name; }
        }
    }
}