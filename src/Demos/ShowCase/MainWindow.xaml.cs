using System;
using System.Collections.Generic;
using System.ComponentModel;
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

//using Kinect.Core.Gestures;

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
                                                          new Point(15, -5),
                                                          new Point(15, -0),
                                                          new Point(15, 5),
                                                          new Point(15, 10),
                                                          new Point(-15, -8),
                                                          new Point(-15, -2.5),
                                                          new Point(-15, 2.5),
                                                          new Point(-15, 8)
                                                      };

        private readonly List<GradientBall> _balls = new List<GradientBall>();
        private readonly Point3D _centerScreen = new Point3D(0, 0, -44.5);

        private readonly List<string> _names = new List<string>
                                                  {
                                                      "Chris Dekkers en Mischa van Oijen",
                                                      "Dirk Jan Aalbers",
                                                      "Remco Seesink",
                                                      "Ronald Katoen, Gerben Jacobs,\r\nSofia Skoblikov en Tamar van der Riet",
                                                      "Harold Janssen",
                                                      "Hester Heringa",
                                                      "Elly Borsboom",
                                                      "Christiaan Zandt",
                                                      "Tjeerd Hans Terpstra en Jan Willem Groenenberg"
                                                  };

        private readonly Size _screenResolution = new Size(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);

        private readonly List<string> _workshops = new List<string>
                                                      {
                                                          "Any time any place any device",
                                                          "Creatief vergaderen en brainstormen",
                                                          "Getting Things Done",
                                                          "Het Nieuwe Werken volgens Generatie Y",
                                                          "HNW = Het Normale Werken",
                                                          "Lijfelijk organiseren",
                                                          "Onbetreden paden:\r\nlaat de energie voor je werken 2.0",
                                                          "SAMENwerken 2.0 - Geweldloze communicatie",
                                                          "Yammer, wat moet ik ermee?"
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

        public MainWindow()
        {
            InitializeComponent();
            InitBalls();
            InitAnimations();
            InitKinect();
        }

        //Kinect properties

        public Point3D HandPoint { get; set; }

        public Visibility HandPointVisibility { get; set; }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            if (_kinect != null)
            {
                _kinect.StopKinect();
            }
        }

        private void InitBalls()
        {
            const string imageSrc = "Images/foto{0}.jpg";
            for (int i = 0; i < _ballPoints.Count; i++)
            {
                _balls.Add(InitGradientBall(string.Format(imageSrc, (i + 1)), _ballPoints[i].X, _ballPoints[i].Y));
            }
        }

        private GradientBall InitGradientBall(string imageSrc, double offsetx, double offsety)
        {
            var p3D = new Point3D(offsetx, offsety, -10);
            var ball = new GradientBall {ImageSource = imageSrc, Offset = p3D};

            visualModel.Children.Add(ball);
            return ball;
        }

        private void InitAnimations()
        {
            foreach (GradientBall ball in _balls)
            {
                var animation = new DoubleAnimation(0, 360, new Duration(new TimeSpan(0, 0, 3)))
                                    {By = 0.5, RepeatBehavior = RepeatBehavior.Forever};

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

            _kinect.UserCreated += KinectUserCreated;
            _kinect.UserRemoved += _kinect_UserRemoved;
            _kinect.StartKinect();
        }

        private void Do3DAnimation(GradientBall ball, Point3D nextPosition)
        {
            SimpleDelegate animation = delegate
                                           {
                                               var speed = new Duration(new TimeSpan(0, 0, 3));
                                               Point3D currentPosition = ball.Offset;

                                               var animationX = new DoubleAnimation(currentPosition.X, nextPosition.X,
                                                                                    speed);
                                               var animationY = new DoubleAnimation(currentPosition.Y, nextPosition.Y,
                                                                                    speed);
                                               var animationZ = new DoubleAnimation(currentPosition.Z, nextPosition.Z,
                                                                                    speed);


                                               var tt3D = new TranslateTransform3D(currentPosition.X, currentPosition.Y,
                                                                                   currentPosition.Z);
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
                Action setHand = delegate
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
                    SimpleDelegate moveBall = delegate
                    {
                        if (_movingBall != null)
                        {
                            var tt3D = new TranslateTransform3D(_mouse3DPosition.X, _mouse3DPosition.Y, -10);
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
                            SimpleDelegate del3 = delegate
                            {
                                lblNaam.Content = _names[index];
                                lblNaam.Visibility = Visibility.Visible;
                            };
                            lblNaam.Dispatcher.BeginInvoke(DispatcherPriority.Send, del3);
                            SimpleDelegate del4 = delegate
                            {
                                lblWorkshop.Text = _workshops[index];
                                lblWorkshop.Visibility = Visibility.Visible;
                            };
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
            //Zet de ballen terug
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
            SelfTouchGesture gesture = user.AddSelfTouchGesture(new Point3D(0, 0, 0), JointID.HandLeft,
                                                                JointID.HandRight);
            gesture.SelfTouchDetected += GestureSelfTouchDetected;
            _kinectUsers.Add(user);

            SimpleDelegate del2 = delegate { HandImage.Visibility = Visibility.Visible; };
            HandImage.Dispatcher.BeginInvoke(DispatcherPriority.Send, del2);
        }

        private void GestureSelfTouchDetected(object sender, SelfTouchEventArgs e)
        {
            SimpleDelegate del3 = delegate { lblNaam.Visibility = Visibility.Hidden; };
            lblNaam.Dispatcher.BeginInvoke(DispatcherPriority.Send, del3);
            SimpleDelegate del4 = delegate { lblWorkshop.Visibility = Visibility.Hidden; };
            lblWorkshop.Dispatcher.BeginInvoke(DispatcherPriority.Send, del4);

            //Zet de ballen terug
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

        private void KinectUserUpdated(object sender, ProcessEventArgs<IUserChangedEvent> e)
        {
            var screenpoint = e.Event.HandRight.ToScreenPosition(new Size(640, 480), _screenResolution);
            var point = new Point(screenpoint.X, screenpoint.Y);
            Move(point);
        }

        private void _kinect_UserRemoved(object sender, KinectUserEventArgs e)
        {
        }

        private delegate void SimpleDelegate();

        #endregion
    }
}