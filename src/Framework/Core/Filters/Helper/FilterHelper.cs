using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Kinect.Core.Filters.Helper
{
    public static class FilterHelper
    {
        public static Point3D GetPoint(this IUserChangedEvent evt, xn.SkeletonJoint joint)
        {
            switch (joint)
            {
                case xn.SkeletonJoint.Head: return evt.Head; 
                case xn.SkeletonJoint.LeftAnkle: return evt.LeftAnkle; 
                case xn.SkeletonJoint.LeftElbow: return evt.LeftElbow; 
                case xn.SkeletonJoint.LeftFingertip: return evt.LeftFingertip;
                case xn.SkeletonJoint.LeftFoot: return evt.LeftFoot; 
                case xn.SkeletonJoint.LeftHand: return evt.LeftHand; 
                case xn.SkeletonJoint.LeftKnee: return evt.LeftKnee; 
                case xn.SkeletonJoint.LeftShoulder: return evt.LeftShoulder; 
                case xn.SkeletonJoint.LeftHip: return evt.LeftHip; 
                case xn.SkeletonJoint.Neck: return evt.Neck; 
                case xn.SkeletonJoint.RightAnkle: return evt.RightAnkle; 
                case xn.SkeletonJoint.RightElbow: return evt.RightElbow; 
                case xn.SkeletonJoint.RightFingertip: return evt.RightFingertip; 
                case xn.SkeletonJoint.RightFoot: return evt.RightFoot; 
                case xn.SkeletonJoint.RightHand: return evt.RightHand; 
                case xn.SkeletonJoint.RightKnee: return evt.RightKnee; 
                case xn.SkeletonJoint.RightHip: return evt.RightHip; 
                case xn.SkeletonJoint.RightShoulder: return evt.RightShoulder; 
                case xn.SkeletonJoint.Torso: return evt.Torso; 
                case xn.SkeletonJoint.Waist: return evt.Waist; 
            }

            return default(Point3D);
        }

        public static List<Point3D> GetPoints(this IUserChangedEvent evt, params xn.SkeletonJoint[] joints)
        {
            List<Point3D> points = new List<Point3D>();
            foreach (var joint in joints)
            {
                points.Add(GetPoint(evt, joint));
            }

            return points;
        }

        internal static void SetPoint(this User user, xn.SkeletonJoint joint, Point3D newPoint)
        {
            switch (joint)
            {
                case xn.SkeletonJoint.Head: user.Head = newPoint; break;
                case xn.SkeletonJoint.LeftAnkle: user.LeftAnkle = newPoint; break;
                case xn.SkeletonJoint.LeftElbow: user.LeftElbow = newPoint; break;
                case xn.SkeletonJoint.LeftFingertip: user.LeftFingertip = newPoint; break;
                case xn.SkeletonJoint.LeftFoot: user.LeftFoot = newPoint; break;
                case xn.SkeletonJoint.LeftHand: user.LeftHand = newPoint; break;
                case xn.SkeletonJoint.LeftKnee: user.LeftKnee = newPoint; break;
                case xn.SkeletonJoint.LeftShoulder: user.LeftShoulder = newPoint; break;
                case xn.SkeletonJoint.LeftHip: user.LeftHip = newPoint; break;
                case xn.SkeletonJoint.Neck: user.Neck = newPoint; break;
                case xn.SkeletonJoint.RightAnkle: user.RightAnkle = newPoint; break;
                case xn.SkeletonJoint.RightElbow: user.RightElbow = newPoint; break;
                case xn.SkeletonJoint.RightFingertip: user.RightFingertip = newPoint; break;
                case xn.SkeletonJoint.RightFoot: user.RightFoot = newPoint; break;
                case xn.SkeletonJoint.RightHand: user.RightHand = newPoint; break;
                case xn.SkeletonJoint.RightKnee: user.RightKnee = newPoint; break;
                case xn.SkeletonJoint.RightHip: user.RightHip = newPoint; break;
                case xn.SkeletonJoint.RightShoulder: user.RightShoulder = newPoint; break;
                case xn.SkeletonJoint.Torso: user.Torso = newPoint; break;
                case xn.SkeletonJoint.Waist: user.Waist = newPoint; break;
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
