using System.Collections.Generic;
using xn;
using Point3D = System.Windows.Media.Media3D.Point3D;

namespace Kinect.Core.Filters.Helper
{
    public static class FilterHelper
    {
        public static Point3D GetPoint(this IUserChangedEvent evt, SkeletonJoint joint)
        {
            switch (joint)
            {
                case SkeletonJoint.Head:
                    return evt.Head;
                case SkeletonJoint.LeftAnkle:
                    return evt.LeftAnkle;
                case SkeletonJoint.LeftElbow:
                    return evt.LeftElbow;
                case SkeletonJoint.LeftFingertip:
                    return evt.LeftFingertip;
                case SkeletonJoint.LeftFoot:
                    return evt.LeftFoot;
                case SkeletonJoint.LeftHand:
                    return evt.LeftHand;
                case SkeletonJoint.LeftKnee:
                    return evt.LeftKnee;
                case SkeletonJoint.LeftShoulder:
                    return evt.LeftShoulder;
                case SkeletonJoint.LeftHip:
                    return evt.LeftHip;
                case SkeletonJoint.Neck:
                    return evt.Neck;
                case SkeletonJoint.RightAnkle:
                    return evt.RightAnkle;
                case SkeletonJoint.RightElbow:
                    return evt.RightElbow;
                case SkeletonJoint.RightFingertip:
                    return evt.RightFingertip;
                case SkeletonJoint.RightFoot:
                    return evt.RightFoot;
                case SkeletonJoint.RightHand:
                    return evt.RightHand;
                case SkeletonJoint.RightKnee:
                    return evt.RightKnee;
                case SkeletonJoint.RightHip:
                    return evt.RightHip;
                case SkeletonJoint.RightShoulder:
                    return evt.RightShoulder;
                case SkeletonJoint.Torso:
                    return evt.Torso;
                case SkeletonJoint.Waist:
                    return evt.Waist;
            }

            return default(Point3D);
        }

        public static List<Point3D> GetPoints(this IUserChangedEvent evt, params SkeletonJoint[] joints)
        {
            var points = new List<Point3D>();
            foreach (SkeletonJoint joint in joints)
            {
                points.Add(GetPoint(evt, joint));
            }

            return points;
        }

        internal static void SetPoint(this User user, SkeletonJoint joint, Point3D newPoint)
        {
            switch (joint)
            {
                case SkeletonJoint.Head:
                    user.Head = newPoint;
                    break;
                case SkeletonJoint.LeftAnkle:
                    user.LeftAnkle = newPoint;
                    break;
                case SkeletonJoint.LeftElbow:
                    user.LeftElbow = newPoint;
                    break;
                case SkeletonJoint.LeftFingertip:
                    user.LeftFingertip = newPoint;
                    break;
                case SkeletonJoint.LeftFoot:
                    user.LeftFoot = newPoint;
                    break;
                case SkeletonJoint.LeftHand:
                    user.LeftHand = newPoint;
                    break;
                case SkeletonJoint.LeftKnee:
                    user.LeftKnee = newPoint;
                    break;
                case SkeletonJoint.LeftShoulder:
                    user.LeftShoulder = newPoint;
                    break;
                case SkeletonJoint.LeftHip:
                    user.LeftHip = newPoint;
                    break;
                case SkeletonJoint.Neck:
                    user.Neck = newPoint;
                    break;
                case SkeletonJoint.RightAnkle:
                    user.RightAnkle = newPoint;
                    break;
                case SkeletonJoint.RightElbow:
                    user.RightElbow = newPoint;
                    break;
                case SkeletonJoint.RightFingertip:
                    user.RightFingertip = newPoint;
                    break;
                case SkeletonJoint.RightFoot:
                    user.RightFoot = newPoint;
                    break;
                case SkeletonJoint.RightHand:
                    user.RightHand = newPoint;
                    break;
                case SkeletonJoint.RightKnee:
                    user.RightKnee = newPoint;
                    break;
                case SkeletonJoint.RightHip:
                    user.RightHip = newPoint;
                    break;
                case SkeletonJoint.RightShoulder:
                    user.RightShoulder = newPoint;
                    break;
                case SkeletonJoint.Torso:
                    user.Torso = newPoint;
                    break;
                case SkeletonJoint.Waist:
                    user.Waist = newPoint;
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