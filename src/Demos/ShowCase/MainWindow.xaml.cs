using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Kinect.Common;
using Kinect.Core;
using Kinect.Core.Eventing;
using Kinect.Core.Gestures;
using Microsoft.Research.Kinect.Nui;

namespace Kinect.ShowCase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly List<Point> _ballPoints = new List<Point>
                                                      {
                                                          new Point(15, -10),
                                                          //new Point(15, -5),
                                                          //new Point(15, -0),
                                                          //new Point(15, 5),
                                                          new Point(15, 10),
                                                          new Point(-15, -10),
                                                          //new Point(-15, -2.5),
                                                          //new Point(-15, 2.5),
                                                          new Point(-15, 10)
                                                      };

        private readonly List<GradientBall> _balls = new List<GradientBall>();
        private readonly Point3D _centerScreen = new Point3D(0, 0, -44.5);

        private readonly List<string> _names = new List<string>
        {
            "Trend voor Technology Services",
            "Trend voor Technology Services",
            "Trend voor Technology Services",
            "Trend voor Technology Services"
        };

        private readonly Size _screenResolution = new Size(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);

        private readonly List<string> _workshops = new List<string>
        {
            "Business Agility",
            "Big Data",
            "Cloud",
            "Mobile apps",
        };

        private MyKinect _kinect;
        private List<User> _kinectUsers;
        private Point3D _animatingBallOriginalPosition;
        private GradientBall _centerscreenBall;

        private Point3D _mouse3DPosition;

        private GradientBall _movingBall;
        private int _movingBallIndex = -1;
        private const double Sensitivity = 1.5;
        private Point _viewPortSize = new Point(36, 26);

        private readonly ChangeResolution _changeResolution;

        public MainWindow()
        {
            _changeResolution = new ChangeResolution();
            _changeResolution.ChangeScreenResolution(1024, 768);
            InitializeComponent();
            InitBalls();
            InitAnimations();
            InitKinect();
        }

        public Point3D HandPoint { get; set; }

        public Visibility HandPointVisibility { get; set; }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            if (_kinect != null)
            {
                _kinect.StopKinect();
            }
            _changeResolution.ChangeScreenResolutionBackToOriginal();
        }

        private void InitBalls()
        {
            const string imageSrc = "pack://application:,,,/Kinect.ShowCase;component/Images/foto{0}.jpg";
            for (int i = 0; i < _ballPoints.Count; i++)
            {
                _balls.Add(InitGradientBall(string.Format(imageSrc, (i + 1)), _ballPoints[i].X, _ballPoints[i].Y));
            }
        }

        private GradientBall InitGradientBall(string imageSrc, double offsetx, double offsety)
        {
            var p3D = new Point3D(offsetx, offsety, -8);
            var ball = new GradientBall { ImageSource = imageSrc, Offset = p3D };

            visualModel.Children.Add(ball);
            return ball;
        }

        private void InitAnimations()
        {
            foreach (GradientBall ball in _balls)
            {
                var animation = new DoubleAnimation(0, 360, new Duration(new TimeSpan(0, 0, 5))) { By = 0.5, RepeatBehavior = RepeatBehavior.Forever };

                var vect = new Vector3D(0, 1, 0);
                var rt3D = new AxisAngleRotation3D(vect, 0);
                var transform = new RotateTransform3D(rt3D);
                ball.TransformGroup.Children.Add(transform);
                rt3D.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
            }
        }

        private void InitKinect()
        {
            _kinectUsers = new List<User>();
            _kinect = MyKinect.Instance;
            _kinect.ChangeMaxSkeletonPositions(.5f, .5f);
            _kinect.ElevationAngleInitialPosition = 0;
            _kinect.UserCreated += KinectUserCreated;
            _kinect.UserRemoved += _kinect_UserRemoved;
            //_kinect.KinectStarted += new EventHandler<KinectEventArgs>(_kinect_KinectStarted);
            _kinect.StartKinect();
        }

        void _kinect_KinectStarted(object sender, KinectEventArgs e)
        {
            //Thread.Sleep(400);
            _kinect.SetElevationAngle(1);
        }

        private void Do3DAnimation(GradientBall ball, Point3D nextPosition)
        {
            Action animation = () =>
            {
                var speed = new Duration(new TimeSpan(0, 0, 3));
                Point3D currentPosition = ball.Offset;

                var animationX = new DoubleAnimation(currentPosition.X, nextPosition.X, speed);
                var animationY = new DoubleAnimation(currentPosition.Y, nextPosition.Y, speed);
                var animationZ = new DoubleAnimation(currentPosition.Z, nextPosition.Z, speed);

                var tt3D = new TranslateTransform3D(currentPosition.X, currentPosition.Y, currentPosition.Z);
                ball.Transform = tt3D;
                tt3D.BeginAnimation(TranslateTransform3D.OffsetXProperty, animationX);
                tt3D.BeginAnimation(TranslateTransform3D.OffsetYProperty, animationY);
                tt3D.BeginAnimation(TranslateTransform3D.OffsetZProperty, animationZ);
            };
            ball.Dispatcher.BeginInvoke(DispatcherPriority.Send, animation);
        }

        private void Move(Point position)
        {
            try
            {
                var pos = position;
                //pos = Mouse.GetPosition(viewPort);

                //update hand
                Action setHand = () =>
                {
                    Canvas.SetTop(HandImage, pos.Y);
                    Canvas.SetLeft(HandImage, pos.X);
                };
                HandImage.Dispatcher.BeginInvoke(DispatcherPriority.Send, setHand);

                _mouse3DPosition.X = ((_viewPortSize.X / (viewPort.ActualWidth / pos.X)) - (_viewPortSize.X / 2)) * -1;
                _mouse3DPosition.Y = ((_viewPortSize.Y / (viewPort.ActualHeight / pos.Y)) - (_viewPortSize.Y / 2)) * -1;

                if (_movingBall == null && _centerscreenBall == null)
                {
                    //Er is geen bal geanimeerd. We kunnen een nieuwe ball zoeken
                    int index = MouseOnBall();
                    if (index != -1)
                    {
                        GradientBall tempBall = _balls[index];
                        Action getBallPosition = delegate { _animatingBallOriginalPosition = tempBall.Offset; };
                        tempBall.Dispatcher.BeginInvoke(DispatcherPriority.Send, getBallPosition);
                        _movingBallIndex = index;
                        _movingBall = tempBall;
                    }
                }
                else if (_movingBall != null)
                {
                    Action moveBall = () =>
                    {
                        if (_movingBall != null)
                        {
                            var tt3D = new TranslateTransform3D(_mouse3DPosition.X, _mouse3DPosition.Y, -10);
                            if (_movingBall != null)
                                _movingBall.Transform = tt3D;
                        }
                    };

                    _movingBall.Dispatcher.BeginInvoke(DispatcherPriority.Send, moveBall);

                    //movingBall.Transform = tt3d;
                    if (WithinMargin(_mouse3DPosition.X, 0, Sensitivity))
                    // && WithinMargin(mouse3dPosition.Y, 0, sensitivity * 2))
                    {
                        if (_movingBallIndex != -1)
                        {
                            int index = _movingBallIndex;
                            Action del3 = () =>
                            {
                                lblNaam.Content = _names[index];
                                lblNaam.Visibility = Visibility.Visible;
                            };
                            Action del4 = () =>
                            {
                                lblWorkshop.Text = _workshops[index];
                                lblWorkshop.Visibility = Visibility.Visible;
                            };
                            lblNaam.Dispatcher.BeginInvoke(DispatcherPriority.Send, del3);
                            lblWorkshop.Dispatcher.BeginInvoke(DispatcherPriority.Send, del4);
                        }
                        //The ball is on the center of the screen. Please make it bigger
                        _centerscreenBall = _movingBall;
                        _movingBall = null;
                        _movingBallIndex = -1;
                        Do3DAnimation(_centerscreenBall, _centerScreen);
                    }
                }
            }
            catch
            {
                //do nothing
            }
        }

        private void WindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            RestoreBalls();
        }

        private void RestoreBalls()
        {
            if (_movingBall != null)
            {
                GradientBall tempball = _movingBall;
                _movingBall = null;
                Do3DAnimation(tempball, _animatingBallOriginalPosition);
            }
            if (_centerscreenBall != null)
            {
                GradientBall tempball = _centerscreenBall;
                _centerscreenBall = null;
                Do3DAnimation(tempball, _animatingBallOriginalPosition);
            }
        }

        private int MouseOnBall()
        {
            for (int i = 0; i < _ballPoints.Count; i++)
            {
                if (WithinMargin(_mouse3DPosition.X, _ballPoints[i].X, Sensitivity) &&
                    WithinMargin(_mouse3DPosition.Y, _ballPoints[i].Y, Sensitivity))
                {
                    return i;
                    //return balls[i];
                }
            }
            return -1;
            //return null;
        }

        internal static bool WithinMargin(double left, double right, double margin)
        {
            return (Math.Abs((left - right)) < margin) || (Math.Abs((right - left)) < margin);
        }

        #region Kinect functions

        private void KinectUserCreated(object sender, KinectUserEventArgs e)
        {
            User user = _kinect.GetUser(e.User.Id);
            user.Updated += KinectUserUpdated;
            SelfTouchGesture gesture = user.AddSelfTouchGesture(new Point3D(0, 0, 0), JointID.HandLeft, JointID.HandRight);
            gesture.SelfTouchDetected += GestureSelfTouchDetected;
            _kinectUsers.Add(user);

            Action del2 = () => HandImage.Visibility = Visibility.Visible;
            HandImage.Dispatcher.BeginInvoke(DispatcherPriority.Send, del2);
        }

        private void GestureSelfTouchDetected(object sender, SelfTouchEventArgs e)
        {
            Action del3 = () => lblNaam.Visibility = Visibility.Hidden;
            Action del4 = () => lblWorkshop.Visibility = Visibility.Hidden;
            lblNaam.Dispatcher.BeginInvoke(DispatcherPriority.Send, del3);
            lblWorkshop.Dispatcher.BeginInvoke(DispatcherPriority.Send, del4);

            RestoreBalls();
        }

        private void KinectUserUpdated(object sender, ProcessEventArgs<IUserChangedEvent> e)
        {
            var screenpoint = e.Event.HandRight.ToScreenPosition(new Size(640, 480), _screenResolution);
            var point = new Point(screenpoint.X, screenpoint.Y);
            Move(point);
        }

        private void _kinect_UserRemoved(object sender, KinectUserEventArgs e)
        {
        }

        #endregion

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                this.Close();
            }
            else if (e.Key == Key.Up)
            {
                _kinect.MotorUp(2);
            }
            else if (e.Key == Key.Down)
            {
                _kinect.MotorDown(2);
            }
        }
    }
}