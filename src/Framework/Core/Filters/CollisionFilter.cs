using System;
using System.Collections.Generic;
using System.Linq;
using Kinect.Common;
using Kinect.Core.Eventing;
using Kinect.Core.Filters.Helper;
using Kinect.Core.Gestures.Helper;
using log4net;
using xn;
using Point3D = System.Windows.Media.Media3D.Point3D;

namespace Kinect.Core.Filters
{
    /// <summary>
    /// Filter checks for collisions of <see cref="System.Windows.Media.Media3D.Point3D"/>'s
    /// </summary>
    public class CollisionFilter : Filter<IUserChangedEvent>
    {
        private const string _name = "MarginFilter";
        private static readonly ILog _log = LogManager.GetLogger(typeof (CollisionFilter));

        /// <summary>
        /// Initializes a new instance of the <see cref="CollisionFilter"/> class.
        /// </summary>
        /// <param name="margin">The margin.</param>
        /// <param name="joints">The joints.</param>
        public CollisionFilter(Point3D margin, params SkeletonJoint[] joints)
        {
            if (joints.Length < 2)
            {
                throw new ArgumentException("Add at least two joints to check");
            }

            FilterData = new Point3D[joints.Length - 1];
            JointsToCheck = joints;
            Margin = margin;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get { return _name; }
        }

        internal Point3D Margin { get; private set; }

        internal SkeletonJoint[] JointsToCheck { get; set; } // TODO : Private Setter?

        /// <summary>
        /// Gets the filter data.
        /// </summary>
        public Point3D[] FilterData { get; private set; }

        // TODO : internal or the other public?

        /// <summary>
        /// Processes the specified evt.
        /// </summary>
        /// <param name="evt">The evt.</param>
        public override void Process(IUserChangedEvent evt)
        {
            List<Point3D> points = evt.GetPoints(JointsToCheck);
            bool succesFullCheck = false;

            if (!succesFullCheck)
            {
                succesFullCheck = true;
                //Use the first point as reference
                Point3D point = points[0];
                //Check every point with the first one
                for (int i = 1; i < points.Count; i++)
                {
                    var margin = new Point3D();
                    margin.X = Math.Abs(point.X - points[i].X);
                    margin.Y = Math.Abs(point.Y - points[i].Y);
                    margin.Z = Math.Abs(point.Z - points[i].Z);
                    FilterData[i - 1] = margin;

                    succesFullCheck = succesFullCheck && Calculator.WithinMargin(point, points[i], Margin);
                    //If one check fails, break the loop
                    if (!succesFullCheck)
                    {
                        break;
                    }
                }
            }

            if (FilterData.Count() > 0 && JointsToCheck.Length > 1)
            {
                _log.IfDebugFormat("CollisionFilter:\t{4} {2} -> {3}\t{0} on {1}", JointsToCheck[0], JointsToCheck[1],
                                   FilterData[0].GetDebugString(), succesFullCheck ? "OK" : string.Empty, evt.ID);
            }

            if (succesFullCheck)
            {
                base.Process(evt);
            }
            else
            {
                OnFilteredEvent(new CollisionFilterEventArgs((Point3D[]) FilterData.Clone()));
            }
        }
    }
}