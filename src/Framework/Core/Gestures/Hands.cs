using System;
using System.Windows.Media.Media3D;

namespace Kinect.Core.Gestures
{
    public class Hands
    {
        public Hands(Point3D left, Point3D right)
        {
            Left = left;
            Right = right;
        }

        public Point3D Left { get; set; }
        public Point3D Right { get; set; }

        public bool DetectClap()
        {
            return DetectClap(Left, Right);
        }

        public bool DetectClap(Point3D left, Point3D right)
        {
            return (WithinMargin(left.X, right.X, ClapGesture.MarginX) &&
                    WithinMargin(left.Y, right.Y, ClapGesture.MarginY) &&
                    WithinMargin(left.Z, right.Z, ClapGesture.MarginZ));
        }

        private bool WithinMargin(double left, double right, double margin)
        {
            return (Math.Abs((left - right)) < margin) || (Math.Abs((right - left)) < margin);
        }

        public override string ToString()
        {
            return string.Format("Left: x={0} y={1} z={2} -- Right: x={3} y={4} z={5}  -- Clap: {6}", Left.X, Left.Y,
                                 Left.Z, Right.X, Right.Y, Right.Z, DetectClap());
        }
    }
}