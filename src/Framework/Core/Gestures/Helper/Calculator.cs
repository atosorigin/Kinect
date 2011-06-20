using System;
using System.Windows.Media.Media3D;

namespace Kinect.Core.Gestures.Helper
{
    internal static class Calculator
    {
        internal static double CalculateAngle(double px1, double py1, double px2, double py2)
        {
            //// Negate X and Y values
            double resX = px2 - px1;
            double resY = py2 - py1;
            double angle = 0.0;
            //// Calculate the angle
            if (resX == 0.0)
            {
                if (resY == 0.0)
                    // was resY. i think 0,0 is seen as same as up... There is an error in http://www.carlosfemmer.com/post/2006/02/Calculate-Angle-between-2-points-using-C.aspx  :-)
                {
                    angle = 0.0;
                }
                else if (resY > 0.0)
                {
                    angle = Math.PI/2.0;
                }
                else
                {
                    angle = Math.PI*3.0/2.0;
                }
            }
            else if (resY == 0.0)
            {
                if (resX > 0.0)
                {
                    angle = 0.0;
                }
                else
                {
                    angle = Math.PI;
                }
            }
            else
            {
                if (resX < 0.0)
                {
                    angle = Math.Atan(resY/resX) + Math.PI;
                }
                else if (resY < 0.0)
                {
                    angle = Math.Atan(resY/resX) + (2*Math.PI);
                }
                else
                {
                    angle = Math.Atan(resY/resX);
                }
            }

            angle = angle*180/Math.PI; //// Convert to degrees
            return angle;
        }

        internal static bool WithinMargin(double left, double right, double margin)
        {
            return (Math.Abs((left - right)) < margin) || (Math.Abs((right - left)) < margin);
        }

        internal static bool WithinMargin(Point3D point1, Point3D point2, Point3D margin)
        {
            return WithinMargin(point1.X, point2.X, margin.X) &&
                   WithinMargin(point1.Y, point2.Y, margin.Y) &&
                   WithinMargin(point1.Z, point2.Z, margin.Z);
        }
    }
}