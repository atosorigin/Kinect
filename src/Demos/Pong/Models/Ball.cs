using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel;

namespace Kinect.Pong.Models
{
    public class Ball : INotifyPropertyChanged
    {
        private Point _position;
        public Point Position
        {
            get
            {
                return _position;
            }
            private set
            {
                if (_position != value)
                {
                    _position = value;
                    OnPropertyChanged("Position");
                }
            }
        }

        public int Radius { get; private set; }
        public double XVelocity { get; private set; }
        public double YVelocity { get; private set; }

        public double Angle { get; private set; }
        public double Speed { get; private set; }

        private System.Drawing.Rectangle Boundry
        {
            get
            {
                return _game.Boundry;
            }
        }

        private PongGame _game
        {
            get
            {
                return PongGame.Instance;
            }
        }
        private ObservableCollection<Paddle> _paddles
        {
            get
            {
                return this._game.Paddles;
            }
        }

        public event EventHandler<ScoreEventArgs> Scored;

        public void Move()
        {
            DetermineBallDirection();
            DetermineBallCollision();
            DeterminePaddleCollision();
            DetermineScore();

            Position = new Point(Position.X + XVelocity, Position.Y + YVelocity);
        }

        private void DetermineScore()
        {
            if (Position.X + Radius >= Boundry.Width)
            {
                OnScored(Paddle.Side.Right, this);
            }
            if (Position.X <= 0 + this.Radius)
            {
                OnScored(Paddle.Side.Left, this);
            }
        }

        private void DetermineBallCollision()
        {
            var minx = Position.X - (this.Radius / 2);
            var maxx = Position.X + (this.Radius / 2);
            var minY = Position.Y - (this.Radius / 2);
            var maxY = Position.Y + (this.Radius / 2);

            foreach (var ball in PongGame.Instance.Balls)
            {
                //Jezelf niet controleren
                if (ball == this)
                {
                    continue;
                }

                //De ballen raken elkaar.
                if (ball.Position.X >= minx && ball.Position.X <= maxx &&
                    ball.Position.Y >= minY && ball.Position.Y <= maxY)
                {
                    //TODO: Bereken hier de collision hoek
                    //Bereken daarna de juiste X en Y velocity
                    XVelocity = -XVelocity;
                }
            }   
        }

        private void DeterminePaddleCollision()
        {
            foreach (var paddle in _paddles)
            {
                if (paddle.PaddleSide == Paddle.Side.Left)
                {
                    if ((paddle.Position.X + (this.Radius / 2) + paddle.Width) > Position.X + (this.Radius / 2) &&
                        Position.Y + (this.Radius / 2) > paddle.Position.Y + (this.Radius / 2) &&
                        Position.Y + (this.Radius / 2) < paddle.Position.Y + (this.Radius / 2) + paddle.Height)
                    {
                        XVelocity = -XVelocity;
                    }
                }
                else if (paddle.PaddleSide == Paddle.Side.Right)
                {
                    if (Position.X > paddle.Position.X - paddle.Width &&
                        Position.Y > paddle.Position.Y &&
                        Position.Y < paddle.Position.Y + paddle.Height)
                    {
                        XVelocity = -XVelocity;
                    }
                }

            }
        }

        private void DetermineBallDirection()
        {
            if ((Position.X < Boundry.X) || (Position.X > Boundry.Width))
            {
                XVelocity = -XVelocity;
            }
            if ((Position.Y < Boundry.Y) || (Position.Y + Radius > Boundry.Height))
            {
                YVelocity = -YVelocity;
            }
        }

        protected virtual void OnScored(Paddle.Side _side, Ball ball)
        {
            var handler = this.Scored;

            if (handler != null)
            {
                handler(this, new ScoreEventArgs(_side, ball));
            }
        }

        public Ball(int radius, double angle, double speed)
        {
            Radius = radius;
            Angle = angle;
            Speed = speed;

            //Spawn at middle point
            Position = new Point(Boundry.Width / 2 - (Radius / 2), Boundry.Height / 2 - (Radius / 2));
            //Aanliggend
            XVelocity = (Math.Cos(Angle * (Math.PI / 180)) * Speed);
            //Overstaand
            YVelocity = (Math.Sin(Angle * (Math.PI / 180)) * Speed);

            if (XVelocity > -0.5 && XVelocity < 0.5)
            {
                XVelocity = XVelocity < 0 ? XVelocity - 0.5 : XVelocity + 0.5;
            }

            if (YVelocity > -0.5 && YVelocity < 0.5)
            {
                YVelocity = YVelocity < 0 ? YVelocity - 0.5 : YVelocity + 0.5;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
