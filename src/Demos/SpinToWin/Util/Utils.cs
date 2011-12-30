using System;
using System.Windows;

namespace Kinect.SpinToWin.Util
{
    public static class Utils
    {

        /// <summary>
        /// Converts a coordinate from the polar coordinate system to the cartesian coordinate system.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Point ComputeCartesianCoordinate(double angle, double radius)
        {
            // convert to radians
            var angleRad = (Math.PI / 180.0) * (angle - 90);

            var x = radius * Math.Cos(angleRad);
            var y = radius * Math.Sin(angleRad);

            return new Point(x, y);
        }
    }
}
