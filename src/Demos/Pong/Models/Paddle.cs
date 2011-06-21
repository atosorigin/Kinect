using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace Kinect.Pong.Models
{
    public class Paddle : INotifyPropertyChanged
    {
        #region Side enum

        public enum Side
        {
            Left,
            Right
        } ;

        #endregion

        private Brush _brush;
        private Point _position;

        public Paddle(Side side, bool isComputerControlled, int kinectUserId)
        {
            _speed = 2;
            PaddleSide = side;
            KinectUserID = kinectUserId;
            IsComputerControlled = isComputerControlled;
            Height = 100;
            Width = 20;
            PositionPaddle(side);
        }

        public Side PaddleSide { get; private set; }
        public bool IsComputerControlled { get; private set; }
        public int Height { get; private set; }
        public int KinectUserID { get; private set; }
        public int Width { get; private set; }
        public int Speed { get; private set; }

        private Rectangle Boundry
        {
            get { return _game.Boundry; }
        }


        public Point Position
        {
            get { return _position; }
            private set
            {
                if (_position != value)
                {
                    _position = value;
                    OnPropertyChanged("Position");
                }
            }
        }

        private double _yVelocity { get; set; }
        private int _speed { get; set; }

        private PongGame _game
        {
            get { return PongGame.Instance; }
        }

        private ObservableCollection<Ball> _balls
        {
            get { return _game.Balls; }
        }

        public Brush Brush
        {
            get { return _brush; }
            set
            {
                if (_brush != value)
                {
                    _brush = value;
                    OnPropertyChanged("Brush");
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void PositionPaddle(Side _side)
        {
            if (_side == Side.Left)
            {
                Brush = new SolidColorBrush(Color.FromScRgb(1f, 1, 1, 1));
                Position = new Point(Boundry.X + (Boundry.Width/100), (Boundry.Height/2) - (Height/2));
            }
            else if (_side == Side.Right)
            {
                Brush = new SolidColorBrush(Color.FromScRgb(1f, 1, 1, 1));
                Position = new Point(Boundry.Width - (Boundry.Width/100) - Width, (Boundry.Height/2) - (Height/2));
            }
        }

        private void ComputerTracking()
        {
            Ball ballToTrack = DetermineBallToTrack();
            DetermineMovement(ballToTrack);
        }

        private void DetermineMovement(Ball ballToTrack)
        {
            //Move paddle towards ball
            if (ballToTrack != null)
            {
                if (ballToTrack.Position.Y > Position.Y + Height/2)
                {
                    SetDirection(_speed);
                }
                if (ballToTrack.Position.Y < Position.Y + Height/2)
                {
                    SetDirection(-_speed);
                }
            }
                //Don't move when not! tracking a ball
            else
            {
                _yVelocity = 0;
            }
        }

        private Ball DetermineBallToTrack()
        {
            Ball ballToTrack = null;
            if (PaddleSide == Side.Right)
            {
                foreach (Ball ball in _balls)
                {
                    //Lock on first ball;
                    if (ball.XVelocity > 0)
                    {
                        if (ballToTrack == null)
                        {
                            ballToTrack = ball;
                        }
                    }
                    //See if other balls are better matches
                    if (ball.XVelocity > 0 && ball.Position.X > ballToTrack.Position.X)
                    {
                        ballToTrack = ball;
                    }
                }
            }
            else if (PaddleSide == Side.Left)
            {
                foreach (Ball ball in _balls)
                {
                    //Lock on first ball;
                    if (ball.XVelocity < 0)
                    {
                        if (ballToTrack == null)
                        {
                            ballToTrack = ball;
                        }
                    }
                    //See if other balls are better matches
                    if (ball.XVelocity < 0 && ball.Position.X < ballToTrack.Position.X)
                    {
                        ballToTrack = ball;
                    }
                }
            }
            return ballToTrack;
        }

        public void Move()
        {
            if (IsComputerControlled) ComputerTracking();
            if ((Position.Y + Height + _yVelocity) < Boundry.Height && Position.Y + _yVelocity > 0)
            {
                Position = new Point(Position.X, Position.Y + _yVelocity);
            }
        }

        public void SetDirection(double speed)
        {
            _yVelocity = speed;
        }


        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}