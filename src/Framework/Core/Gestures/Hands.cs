using System;
using xn;

namespace Kinect.Core.Gestures
{
    public class Hands
    {
        public Point3D Left { get; set; }
        public Point3D Right { get; set; }

        public Hands(Point3D left, Point3D right)
        {
            this.Left = left;
            this.Right = right;
        }

        public bool DetectClap()
        {
            return this.DetectClap(Left, Right);
        }

        public bool DetectClap(Point3D left, Point3D right)
        {
            return (this.WithinMargin(left.X, right.X, ClapGesture.MarginX) &&
                this.WithinMargin(left.Y, right.Y, ClapGesture.MarginY) &&
                this.WithinMargin(left.Z, right.Z, ClapGesture.MarginZ));
        }

        private bool WithinMargin(float left, float right, float margin)
        {
            return (Math.Abs((left - right)) < margin) || (Math.Abs((right - left)) < margin);
        }

        public override string ToString()
        {
            return string.Format("Left: x={0} y={1} z={2} -- Right: x={3} y={4} z={5}  -- Clap: {6}", Left.X, Left.Y, Left.Z, Right.X, Right.Y, Right.Z, DetectClap());
        }
    }
}
