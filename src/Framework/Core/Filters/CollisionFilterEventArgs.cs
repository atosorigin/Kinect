using Kinect.Core.Eventing;
using System.Windows.Media.Media3D;

namespace Kinect.Core.Filters
{
    public class CollisionFilterEventArgs : FilterEventArgs
    {
        public Point3D[] MarginData { get; private set; }

        private const string _name = "MarginFilterEventArgs";

        public override string Name
        {
            get { return _name; }
        }

        public CollisionFilterEventArgs(Point3D[] marginData)
        {
            this.MarginData = marginData;
        }
    }
}
