using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinect.WPF.nPong.Models
{
    public class ScoreEventArgs : EventArgs
    {
        public Models.Paddle.Side Side { get; private set; }
        public Ball Ball { get; private set; }

        public ScoreEventArgs(Models.Paddle.Side side, Ball ball)
        {
            Side = side;
            Ball = ball;
        }
    }
}
