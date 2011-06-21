using System;
using System.Windows.Media.Media3D;

namespace HiddenMarkovModel.Utils
{
    public class MotionCalculator
    {
        private readonly int precision = 30;

        public double CalculateMotion(Point3D start, Point3D end)
        {
            //Feature extraction based on: A Hidden Markov Model-Based Continuous 
            //Gesture Recognition System for Hand Motion Trajectory
            double x = Math.Atan2(end.Y - start.Y, end.X - start.X)*180/Math.PI;
            if (x != Math.Abs(x))
            {
                x += 360;
            }
            return Convert.ToInt32(x/precision);
        }
    }
}