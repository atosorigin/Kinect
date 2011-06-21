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
                    return evt.LeftAnkle;
                case JointID.ElbowLeft:
                    return evt.LeftElbow;
                    //case JointID.R: return evt.LeftFingertip;
                case JointID.FootLeft:
                    return evt.LeftFoot;
                case JointID.HandLeft:
                    return evt.LeftHand;
                case JointID.KneeLeft:
                    return evt.LeftKnee;
                case JointID.ShoulderLeft:
                    return evt.LeftShoulder;
                case JointID.HipLeft:
                    return evt.LeftHip;
                case JointID.ShoulderCenter:
                    return evt.Neck;
                case JointID.AnkleRight:
                    return evt.RightAnkle;
                case JointID.ElbowRight:
                    return evt.RightElbow;
                    //case JointID.RightFingertip: return evt.RightFingertip; 
                case JointID.FootRight:
                    return evt.RightFoot;
                case JointID.HandRight:
                    return evt.RightHand;
                case JointID.KneeRight:
                    return evt.RightKnee;
                case JointID.HipRight:
                    return evt.RightHip;
                case JointID.ShoulderRight:
                    return evt.RightShoulder;
                case JointID.Spine:
                    return evt.Torso;
                case JointID.HipCenter:
                    return evt.Waist;
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
                    user.LeftAnkle = newPoint;
                    break;
                case JointID.ElbowLeft:
                    user.LeftElbow = newPoint;
                    break;
                    //case JointID.LeftFingertip: user.LeftFingertip = newPoint; break;
                case JointID.FootLeft:
                    user.LeftFoot = newPoint;
                    break;
                case JointID.HandLeft:
                    user.LeftHand = newPoint;
                    break;
                case JointID.KneeLeft:
                    user.LeftKnee = newPoint;
                    break;
                case JointID.ShoulderLeft:
                    user.LeftShoulder = newPoint;
                    break;
                case JointID.HipLeft:
                    user.LeftHip = newPoint;
                    break;
                case JointID.ShoulderCenter:
                    user.Neck = newPoint;
                    break;
                case JointID.AnkleRight:
                    user.RightAnkle = newPoint;
                    break;
                case JointID.ElbowRight:
                    user.RightElbow = newPoint;
                    break;
                    //case JointID.RightFingertip: user.RightFingertip = newPoint; break;
                case JointID.FootRight:
                    user.RightFoot = newPoint;
                    break;
                case JointID.HandRight:
                    user.RightHand = newPoint;
                    break;
                case JointID.KneeRight:
                    user.RightKnee = newPoint;
                    break;
                case JointID.HipRight:
                    user.RightHip = newPoint;
                    break;
                case JointID.ShoulderRight:
                    user.RightShoulder = newPoint;
                    break;
                case JointID.Spine:
                    user.Torso = newPoint;
                    break;
                case JointID.HipCenter:
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