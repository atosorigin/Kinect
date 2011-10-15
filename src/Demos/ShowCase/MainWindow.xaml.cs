using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
    public partial class MainWindow : Window
    {
        private readonly List<Point> ballPoints = new List<Point>
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

        private readonly List<GradientBall> balls = new List<GradientBall>();
        private readonly Point3D centerScreen = new Point3D(0, 0, -44.5);

        private readonly List<string> names = new List<string>
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

        private readonly Size screenResolution = new Size(1024, 768);

        private readonly List<string> workshops = new List<string>
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
        private Point3D animatingBallOriginalPosition;
        private GradientBall centerscreenBall;

        private Point3D mouse3dPosition;

        private GradientBall movingBall;
        private int movingBallIndex = -1;
        private double sensitivity = 1.5;
        private Point viewPortSize = new Point(36, 26);

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

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_kinect != null)
            {
                _kinect.StopKinect();
            }
        }

        private void InitBalls()
        {
            string imageSrc = "Images/foto{0}.jpg";
            for (int i = 0; i < ballPoints.Count; i++)
            {
                balls.Add(InitGradientBall(string.Format(imageSrc, (i + 1)), ballPoints[i].X, ballPoints[i].Y));
            }
        }

        private GradientBall InitGradientBall(string imageSrc, double offsetx, double offsety)
        {
            var p3D = new Point3D(offsetx, offsety, -10);
            var ball = new GradientBall();
            ball.ImageSource = imageSrc;
            ball.Offset = p3D;

            visualModel.Children.Add(ball);
            return ball;
        }

        private void InitAnimations()
        {
            foreach (GradientBall ball in balls)
            {
                var animation = new DoubleAnimation(0, 360, new Duration(new TimeSpan(0, 0, 3)));
                animation.By = 0.5;
                animation.RepeatBehavior = RepeatBehavior.Forever;

                var vect = new Vector3D(0, 1, 0);
                var rt = new RotateTransform();
                var rt3d = new AxisAngleRotation3D(vect, 0);
                var transform = new RotateTransform3D(rt3d);
                ball.TransformGroup.Children.Add(transform);
                rt3d.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
            }
        }

        private void InitKinect()
        {
            _kinectUsers = new List<User>();
            _kinect = MyKinect.Instance;

            _kinect.UserCreated += _kinect_UserCreated;
            _kinect.UserRemoved += _kinect_UserRemoved;
            _kinect.StartKinect();
        }

        private void Do3DAnimation(GradientBall ball, Point3D nextPosition)
        {
            SimpleDelegate animation = delegate
                                           {
                                               var speed = new Duration(new TimeSpan(0, 0, 3));
                                               Point3D currentPosition;
                                               currentPosition = ball.Offset;

                                               var animationX = new DoubleAnimation(currentPosition.X, nextPosition.X,
                                                                                    speed);
                                               var animationY = new DoubleAnimation(currentPosition.Y, nextPosition.Y,
                                                                                    speed);
                                               var animationZ = new DoubleAnimation(currentPosition.Z, nextPosition.Z,
                                                                                    speed);


                                               var tt3d = new TranslateTransform3D(currentPosition.X, currentPosition.Y,
                                                                                   currentPosition.Z);
                                               ball.Transform = tt3d;
                                               tt3d.BeginAnimation(TranslateTransform3D.OffsetXProperty, animationX);
                                               tt3d.BeginAnimation(TranslateTransform3D.OffsetYProperty, animationY);
                                               tt3d.BeginAnimation(TranslateTransform3D.OffsetZProperty, animationZ);
                                           };
            ball.Dispatcher.BeginInvoke(DispatcherPriority.Send, animation);
        }

        private void Move(Point pos)
        {
            //Point pos = Mouse.GetPosition(viewPort);

            //update hand
            Action setHand = delegate
                                 {
                                     Canvas.SetTop(HandImage, pos.Y);
                                     Canvas.SetLeft(HandImage, pos.X);
                                 };
            HandImage.Dispatcher.BeginInvoke(DispatcherPriority.Send, setHand);

            mouse3dPosition.X = ((viewPortSize.X/(viewPort.ActualWidth/pos.X)) - (viewPortSize.X/2))*-1;
            mouse3dPosition.Y = ((viewPortSize.Y/(viewPort.ActualHeight/pos.Y)) - (viewPortSize.Y/2))*-1;

            if (movingBall == null && centerscreenBall == null)
            {
                //Er is geen bal geanimeerd. We kunnen een nieuwe ball zoeken
                int index = MouseOnBall();
                if (index != -1)
                {
                    GradientBall tempBall = balls[index];
                    Action getBallPosition = delegate { animatingBallOriginalPosition = tempBall.Offset; };
                    tempBall.Dispatcher.BeginInvoke(DispatcherPriority.Send, getBallPosition);
                    movingBallIndex = index;
                    movingBall = tempBall;
                }
            }
            else if (movingBall != null)
            {
                SimpleDelegate moveBall = delegate
                                              {
                                                  if (movingBall != null)
                                                  {
                                                      var tt3d = new TranslateTransform3D(mouse3dPosition.X,
                                                                                          mouse3dPosition.Y, -10);
                                                      movingBall.Transform = tt3d;
                                                  }
                                              };

                movingBall.Dispatcher.BeginInvoke(DispatcherPriority.Send, moveBall);

                //movingBall.Transform = tt3d;
                if (WithinMargin(mouse3dPosition.X, 0, sensitivity))
                    // && WithinMargin(mouse3dPosition.Y, 0, sensitivity * 2))
                {
                    if (movingBallIndex != -1)
                    {
                        int index = movingBallIndex;
                        SimpleDelegate del3 = delegate
                                                  {
                                                      lblNaam.Content = names[index];
                                                      lblNaam.Visibility = Visibility.Visible;
                                                  };
                        lblNaam.Dispatcher.BeginInvoke(DispatcherPriority.Send, del3);
                        SimpleDelegate del4 = delegate
                                                  {
                                                      lblWorkshop.Text = workshops[index];
                                                      lblWorkshop.Visibility = Visibility.Visible;
                                                  };
                        lblWorkshop.Dispatcher.BeginInvoke(DispatcherPriority.Send, del4);
                    }
                    //The ball is on the center of the screen. Please make it bigger
                    centerscreenBall = movingBall;
                    movingBall = null;
                    movingBallIndex = -1;
                    Do3DAnimation(centerscreenBall, centerScreen);
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Zet de ballen terug
            if (movingBall != null)
            {
                GradientBall tempball = movingBall;
                movingBall = null;
                Do3DAnimation(tempball, animatingBallOriginalPosition);
            }
            if (centerscreenBall != null)
            {
                GradientBall tempball = centerscreenBall;
                centerscreenBall = null;
                Do3DAnimation(tempball, animatingBallOriginalPosition);
            }
        }

        private int MouseOnBall()
        {
            for (int i = 0; i < ballPoints.Count; i++)
            {
                if (WithinMargin(mouse3dPosition.X, ballPoints[i].X, sensitivity) &&
                    WithinMargin(mouse3dPosition.Y, ballPoints[i].Y, sensitivity))
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

        private void _kinect_UserCreated(object sender, KinectUserEventArgs e)
        {
            User user = _kinect.GetUser(e.User.Id);
            user.Updated += _kinectUser_Updated;
            SelfTouchGesture gesture = user.AddSelfTouchGesture(new Point3D(0, 0, 0), JointID.HandLeft,
                                                                JointID.HandRight);
            gesture.SelfTouchDetected += gesture_SelfTouchDetected;
            _kinectUsers.Add(user);

            SimpleDelegate del2 = delegate { HandImage.Visibility = Visibility.Visible; };
            HandImage.Dispatcher.BeginInvoke(DispatcherPriority.Send, del2);
        }

        private void gesture_SelfTouchDetected(object sender, SelfTouchEventArgs e)
        {
            SimpleDelegate del3 = delegate { lblNaam.Visibility = Visibility.Hidden; };
            lblNaam.Dispatcher.BeginInvoke(DispatcherPriority.Send, del3);
            SimpleDelegate del4 = delegate { lblWorkshop.Visibility = Visibility.Hidden; };
            lblWorkshop.Dispatcher.BeginInvoke(DispatcherPriority.Send, del4);

            //Zet de ballen terug
            if (movingBall != null)
            {
                GradientBall tempball = movingBall;
                movingBall = null;
                Do3DAnimation(tempball, animatingBallOriginalPosition);
            }
            if (centerscreenBall != null)
            {
                GradientBall tempball = centerscreenBall;
                centerscreenBall = null;
                Do3DAnimation(tempball, animatingBallOriginalPosition);
            }
        }

        private void _kinectUser_Updated(object sender, ProcessEventArgs<IUserChangedEvent> e)
        {
            Point3D screenpoint = e.Event.HandRight.ToScreenPosition(new Size(640, 480), screenResolution,
                                                                     new Point(213, 160), new Size(213, 160));
            //var screenpoint = e.Event.RightHand.ToScreenPosition(new Size(640, 480), new Size(1650, 1050));
            var point = new Point(screenpoint.X, screenpoint.Y);
            Move(point);
            //New point
            //e.Event.LeftHand
        }

        private void _kinect_UserRemoved(object sender, KinectUserEventArgs e)
        {
        }

        private delegate void SimpleDelegate();

        #endregion
    }
}