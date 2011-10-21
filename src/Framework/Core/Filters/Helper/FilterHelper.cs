using System.Collections.Generic;
using System.Windows.Media.Media3D;
using Microsoft.Research.Kinect.Nui;

namespace Kinect.Core.Filters.Helper
{
    public static class FilterHelper
    {
        public static Point3D GetPoint(this IUserChangedEvent evt, JointID joint)
        {
            switch (joint)
            {
                case JointID.Head:
                    return evt.Head;
                case JointID.AnkleLeft:
                    return evt.AnkleLeft;
                case JointID.ElbowLeft:
                    return evt.ElbowLeft;
                case JointID.FootLeft:
                    return evt.FootLeft;
                case JointID.HandLeft:
                    return evt.HandLeft;
                case JointID.KneeLeft:
                    return evt.KneeLeft;
                case JointID.ShoulderLeft:
                    return evt.ShoulderLeft;
                case JointID.HipLeft:
                    return evt.HipLeft;
                case JointID.ShoulderCenter:
                    return evt.ShoulderCenter;
                case JointID.AnkleRight:
                    return evt.AnkleRight;
                case JointID.ElbowRight:
                    return evt.ElbowRight;
                case JointID.FootRight:
                    return evt.FootRight;
                case JointID.HandRight:
                    return evt.HandRight;
                case JointID.KneeRight:
                    return evt.KneeRight;
                case JointID.HipRight:
                    return evt.HipRight;
                case JointID.ShoulderRight:
                    return evt.ShoulderRight;
                case JointID.Spine:
                    return evt.Spine;
                case JointID.HipCenter:
                    return evt.HipCenter;
            }

            return default(Point3D);
        }

        public static List<Point3D> GetPoints(this IUserChangedEvent evt, params JointID[] joints)
        {
            var points = new List<Point3D>();
            foreach (JointID joint in joints)
            {
                points.Add(GetPoint(evt, joint));
            }

            return points;
        }

        internal static void SetPoint(this User user, JointID joint, Point3D newPoint)
        {
            switch (joint)
            {
                case JointID.Head:
                    user.Head = newPoint;
                    break;
                case JointID.AnkleLeft:
                    user.AnkleLeft = newPoint;
                    break;
                case JointID.ElbowLeft:
                    user.ElbowLeft = newPoint;
                    break;
                case JointID.FootLeft:
                    user.FootLeft = newPoint;
                    break;
                case JointID.HandLeft:
                    user.HandLeft = newPoint;
                    break;
                case JointID.KneeLeft:
                    user.KneeLeft = newPoint;
                    break;
                case JointID.ShoulderLeft:
                    user.ShoulderLeft = newPoint;
                    break;
                case JointID.HipLeft:
                    user.HipLeft = newPoint;
                    break;
                case JointID.ShoulderCenter:
                    user.ShoulderCenter = newPoint;
                    break;
                case JointID.AnkleRight:
                    user.AnkleRight = newPoint;
                    break;
                case JointID.ElbowRight:
                    user.ElbowRight = newPoint;
                    break;
                case JointID.FootRight:
                    user.FootRight = newPoint;
                    break;
                case JointID.HandRight:
                    user.HandRight = newPoint;
                    break;
                case JointID.KneeRight:
                    user.KneeRight = newPoint;
                    break;
                case JointID.HipRight:
                    user.HipRight = newPoint;
                    break;
                case JointID.ShoulderRight:
                    user.ShoulderRight = newPoint;
                    break;
                case JointID.Spine:
                    user.Spine = newPoint;
                    break;
                case JointID.HipCenter:
                    user.HipCenter = newPoint;
                    break;
            }
        }

        public static Point3D CalculateCorrection(IList<Point3D> points)
        {
            var correction = new Point3D();
            if (points.Count > 1)
            {
                correction.X = points[0].X - points[1].X;
                correction.Y = points[0].Y - points[1].Y;
                correction.Z = points[0].Z - points[1].Z;
            }
            return correction;
        }
    }
}