using System;

namespace Kinect.Pong.Models
{
    public class ScoreEventArgs : EventArgs
    {
        public ScoreEventArgs(Paddle.Side side, Ball ball)
        {
            Side = side;
            Ball = ball;
        }

        public Paddle.Side Side { get; private set; }
        public Ball Ball { get; private set; }
    }
}