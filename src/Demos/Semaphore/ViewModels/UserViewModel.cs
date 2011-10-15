using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using Kinect.Common;
using Kinect.Core;
using Kinect.Core.Eventing;
using Kinect.Core.Filters.Helper;
using Kinect.Core.Gestures;
using Microsoft.Research.Kinect.Nui;

namespace Kinect.Semaphore.ViewModels
{
    public class UserViewModel : ViewModelBase
    {
        private static object _syncRoot = new object();
        private readonly DispatcherTimer _dispatchtimer;
        private const double UpdateMargin = 0.5;
        private const double PropertionModifier = 1.5;
        private readonly User _user;
        private DateTime _startDateTime;
        private Brush _brush;
        private int _calibrationCounter = 5;
        private string _calibrationText;
        private DispatcherTimer _calibrationTimer;
        private Color _color;
        private int _counter;
        private Point3D _head;
        private Point3D _leftAnkle;
        private Point3D _leftElbow;
        private Point3D _leftFingertip;
        private Point3D _leftFoot;
        private Point3D _leftHand;
        private Point3D _leftHip;
        private Point3D _leftKnee;
        private Point3D _leftShoulder;
        private Point3D _neck;
        private Point3D _rightAnkle;
        private Point3D _rightElbow;
        private Point3D _rightFingertip;
        private Point3D _rightFoot;
        private Point3D _rightHand;
        private Point3D _rightHip;
        private Point3D _rightKnee;
        private Point3D _rightShoulder;
        private SemaphoreGesture _semaphoreGesture;
        private TimeSpan _stopWatch;
        private Point3D _torso;
        private Point3D _waist;

        public Visibility _win = Visibility.Hidden;

        public UserViewModel(int id)
            : this(new User(id))
        {
        }

        public UserViewModel(User user)
        {
            _user = user;
            _user.Updated += _user_Updated;
            _dispatchtimer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(1)};
            _dispatchtimer.Tick += _dispatchtimer_Tick;
        }

        public int ID
        {
            get { return _user.ID; }
        }

        public Visibility Win
        {
            get { return _win; }
            set
            {
                if (value != _win)
                {
                    _win = value;
                    RaisePropertyChanged("Win");
                }
            }
        }

        public TimeSpan StopWatch
        {
            get { return _stopWatch; }
            set
            {
                if (value != _stopWatch)
                {
                    _stopWatch = value;
                    RaisePropertyChanged("StopWatch");
                }
            }
        }

        public string CalibrationText
        {
            get { return _calibrationText; }
            set
            {
                _calibrationText = value;
                RaisePropertyChanged("CalibrationText");
            }
        }

        public SemaphoreViewModel Semaphore { get; private set; }

        public Color Color
        {
            get { return _color; }
            set
            {
                if (value != _color)
                {
                    _color = value;
                    RaisePropertyChanged("Color");
                }
            }
        }

        public Brush Brush
        {
            get { return _brush; }
            set
            {
                if (value != _brush)
                {
                    _brush = value;
                    RaisePropertyChanged("Brush");
                }
            }
        }

        public double SemafoorX
        {
            get { return _user.Head.X - 75; }
        }

        public double SemafoorY
        {
            get { return _user.Head.Y - 200; }
        }

        //private setters are for Expression Designer

        public Point3D Head
        {
            get { return _head; }
            set
            {
                if (Changed(value, _head))
                {
                    _head = value;
                    RaisePropertyChanged("Head");
                    RaisePropertyChanged("SemafoorX");
                    RaisePropertyChanged("SemafoorY");
                }
            }
        }

        public Point3D Neck
        {
            get { return _neck; }
            set
            {
                if (Changed(value, _head))
                {
                    _neck = value;
                    RaisePropertyChanged("Neck");
                }
            }
        }

        public Point3D Torso
        {
            get { return _torso; }
            set
            {
                if (Changed(value, _torso))
                {
                    _torso = value;
                    RaisePropertyChanged("Torso");
                }
            }
        }

        public Point3D Waist
        {
            get { return _waist; }
            set
            {
                if (Changed(value, _waist))
                {
                    _waist = value;
                    RaisePropertyChanged("Waist");
                }
            }
        }

        public Point3D LeftShoulder
        {
            get { return _leftShoulder; }
            set
            {
                if (Changed(value, _leftShoulder))
                {
                    _leftShoulder = value;
                    RaisePropertyChanged("LeftShoulder");
                }
            }
        }

        public Point3D RightShoulder
        {
            get { return _rightShoulder; }
            set
            {
                if (Changed(value, _rightShoulder))
                {
                    _rightShoulder = value;
                    RaisePropertyChanged("RightShoulder");
                }
            }
        }

        public Point3D LeftElbow
        {
            get { return _leftElbow; }
            set
            {
                if (Changed(value, _leftElbow))
                {
                    _leftElbow = value;
                    RaisePropertyChanged("LeftElbow");
                }
            }
        }

        public Point3D RightElbow
        {
            get { return _rightElbow; }
            set
            {
                if (Changed(value, _rightElbow))
                {
                    _rightElbow = value;
                    RaisePropertyChanged("RightElbow");
                }
            }
        }

        public Point3D LeftHand
        {
            get { return _leftHand; }
            set
            {
                if (Changed(value, _leftHand))
                {
                    _leftHand = value;
                    RaisePropertyChanged("LeftHand");
                }
            }
        }

        public Point3D RightHand
        {
            get { return _rightHand; }
            set
            {
                if (Changed(value, _rightHand))
                {
                    _rightHand = value;
                    RaisePropertyChanged("RightHand");
                }
            }
        }

        public Point3D LeftFingertip
        {
            get { return _leftFingertip; }
            set
            {
                if (Changed(value, _leftFingertip))
                {
                    _leftFingertip = value;
                    RaisePropertyChanged("LeftFingertip");
                }
            }
        }

        public Point3D RightFingertip
        {
            get { return _rightFingertip; }
            set
            {
                if (Changed(value, _rightFingertip))
                {
                    _rightFingertip = value;
                    RaisePropertyChanged("RightFingertip");
                }
            }
        }

        public Point3D LeftHip
        {
            get { return _leftHip; }
            set
            {
                if (Changed(value, _leftHip))
                {
                    _leftHip = value;
                    RaisePropertyChanged("LeftHip");
                }
            }
        }

        public Point3D RightHip
        {
            get { return _rightHip; }
            set
            {
                if (Changed(value, _rightHip))
                {
                    _rightHip = value;
                    RaisePropertyChanged("RightHip");
                }
            }
        }

        public Point3D LeftKnee
        {
            get { return _leftKnee; }
            set
            {
                if (Changed(value, _leftKnee))
                {
                    _leftKnee = value;
                    RaisePropertyChanged("LeftKnee");
                }
            }
        }

        public Point3D RightKnee
        {
            get { return _rightKnee; }
            set
            {
                if (Changed(value, _rightKnee))
                {
                    _rightKnee = value;
                    RaisePropertyChanged("RightKnee");
                }
            }
        }

        public Point3D LeftAnkle
        {
            get { return _leftAnkle; }
            set
            {
                if (Changed(value, _leftAnkle))
                {
                    _leftAnkle = value;
                    RaisePropertyChanged("LeftAnkle");
                }
            }
        }

        public Point3D RightAnkle
        {
            get { return _rightAnkle; }
            set
            {
                if (Changed(value, _rightAnkle))
                {
                    _rightAnkle = value;
                    RaisePropertyChanged("RightAnkle");
                }
            }
        }

        public Point3D LeftFoot
        {
            get { return _leftFoot; }
            set
            {
                if (Changed(value, _leftFoot))
                {
                    _leftFoot = value;
                    RaisePropertyChanged("LeftFoot");
                }
            }
        }

        public Point3D RightFoot
        {
            get { return _rightFoot; }
            set
            {
                if (Changed(value, _rightFoot))
                {
                    _rightFoot = value;
                    RaisePropertyChanged("RightFoot");
                }
            }
        }

        private bool Changed(Point3D newValue, Point3D oldValue)
        {
            if (Math.Abs(newValue.X - oldValue.X) >= UpdateMargin ||
                Math.Abs(newValue.Y - oldValue.Y) >= UpdateMargin ||
                Math.Abs(newValue.Z - oldValue.Z) >= UpdateMargin
                )
            {
                return true;
            }
            return false;
        }

        private void _dispatchtimer_Tick(object sender, EventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                                      {
                                                          StopWatch = DateTime.Now - _startDateTime;
                                                          //StopWatch += TimeSpan.FromMilliseconds(1);
                                                      });
        }

        private void _user_Updated(object sender, ProcessEventArgs<IUserChangedEvent> e)
        {
            PropertyInfo[] eventProperties = e.Event.GetType().GetProperties();
            Type thisType = GetType();

            eventProperties.AsParallel().ForAll(pi =>
                                                    {
                                                        PropertyInfo prop = thisType.GetProperty(pi.Name);
                                                        if (prop != null && prop.CanWrite)
                                                        {
                                                            if (pi.PropertyType == typeof(Point3D))
                                                            {
                                                                var propValue = pi.GetValue(e.Event, null);
                                                                var point = ((Point3D)propValue);
                                                                var newValue = new Point3D(point.X * PropertionModifier, point.Y * PropertionModifier, point.Z * PropertionModifier);

                                                                prop.SetValue(this, newValue, null);
                                                            }
                                                        }
                                                    });
        }

        internal void AddSemaphoreTracking()
        {
            Semaphore = new SemaphoreViewModel();
            Semaphore.PropertyChanged += Semaphore_PropertyChanged;
            _semaphoreGesture = _user.AddSemaphoreTouchGesture();
            _semaphoreGesture.SemafoorDetected += _semaphoreGesture_SemafoorDetected;
        }

        private void Semaphore_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsRunning")
            {
                SetStopwatch(Semaphore.IsRunning);
            }
        }

        public void SetStopwatch(bool isRunning)
        {
            if (isRunning && !_dispatchtimer.IsEnabled)
            {
                _startDateTime = DateTime.Now;

                //StopWatch = new TimeSpan(0);
                _dispatchtimer.Start();
            }
            else if (!isRunning && _dispatchtimer.IsEnabled)
            {
                _dispatchtimer.Stop();
            }
        }

        internal void AddSelfTouchGesture()
        {
            _calibrationTimer = new DispatcherTimer();
            _calibrationTimer.Interval = new TimeSpan(0, 0, 1);
            _calibrationTimer.Tick += CountdownTimerStep;
            _calibrationTimer.Start();
        }

        private void CountdownTimerStep(object sender, EventArgs e)
        {
            _calibrationCounter--;
            if (_calibrationCounter > 0)
            {
                CalibrationText = _calibrationCounter.ToString();
            }
            else if (_calibrationCounter == 0)
            {
                AddSelfTouchGesture(
                    FilterHelper.CalculateCorrection(new List<Point3D> { _user.LeftHand, _user.RightShoulder }));
                CalibrationText = "Saving";
            }
            else
            {
                CalibrationText = string.Empty;
                _calibrationTimer.Stop();
            }
        }

        private void AddSelfTouchGesture(Point3D correction)
        {
            _user.AddSelfTouchGesture(correction, JointID.HandLeft, JointID.ShoulderRight).SelfTouchDetected +=
                SelfTouch;
            Trace.Write(string.Format("Correction {0}", correction.GetDebugString()));
        }

        private void SelfTouch(object sender, SelfTouchEventArgs e)
        {
            Trace.Write(string.Format("{0} LeftHand on RightShoulder", _counter++));
        }

        internal void RemoveSemaphoreTracking()
        {
            _semaphoreGesture.SemafoorDetected -= _semaphoreGesture_SemafoorDetected;
            //TODO Remove Semaphore gesture logic
            //_user.RemoveSemaphoreTouchGesture();
            _semaphoreGesture = null;
            Semaphore = null;
        }

        private void _semaphoreGesture_SemafoorDetected(object sender, KinectSemaphoreGestureEventArgs e)
        {
            Core.Gestures.Model.Semaphore detected = e.Semafoor;
            Semaphore.SemaphoreDetected(detected);
        }

        public void LogToFile()
        {
            using (var writer = new StreamWriter("c:/temp/user.log"))
            {
                writer.WriteLine("Head:\t\t{0}\t{1}\t{2}", Head.X, Head.Y, Head.Z);
                writer.WriteLine("LeftAnkle:\t\t{0}\t{1}\t{2}", Neck.X, Neck.Y, Neck.Z);
                writer.WriteLine("LeftShoulder:\t\t{0}\t{1}\t{2}", LeftShoulder.X, LeftShoulder.Y, LeftShoulder.Z);
                writer.WriteLine("RightShoulder:\t\t{0}\t{1}\t{2}", RightShoulder.X, RightShoulder.Y, RightShoulder.Z);
                writer.WriteLine("Torso:\t\t{0}\t{1}\t{2}", Torso.X, Torso.Y, Torso.Z);
                writer.WriteLine("Waist:\t\t{0}\t{1}\t{2}", Waist.X, Waist.Y, Waist.Z);
                writer.WriteLine("LeftElbow:\t\t{0}\t{1}\t{2}", LeftElbow.X, LeftElbow.Y, LeftElbow.Z);
                writer.WriteLine("RightElbow:\t\t{0}\t{1}\t{2}", RightElbow.X, RightElbow.Y, RightElbow.Z);
                writer.WriteLine("LeftHand:\t\t{0}\t{1}\t{2}", LeftHand.X, LeftHand.Y, LeftHand.Z);
                writer.WriteLine("RightHand:\t\t{0}\t{1}\t{2}", RightHand.X, RightHand.Y, RightHand.Z);
                writer.WriteLine("LeftFingertip:\t\t{0}\t{1}\t{2}", LeftFingertip.X, LeftFingertip.Y, LeftFingertip.Z);
                writer.WriteLine("RightFingertip:\t\t{0}\t{1}\t{2}", RightFingertip.X, RightFingertip.Y,
                                 RightFingertip.Z);
                writer.WriteLine("LeftHip:\t\t{0}\t{1}\t{2}", LeftHip.X, LeftHip.Y, LeftHip.Z);
                writer.WriteLine("RightHip:\t\t{0}\t{1}\t{2}", RightHip.X, RightHip.Y, RightHip.Z);
                writer.WriteLine("LeftKnee:\t\t{0}\t{1}\t{2}", LeftKnee.X, LeftKnee.Y, LeftKnee.Z);
                writer.WriteLine("RightKnee:\t\t{0}\t{1}\t{2}", RightKnee.X, RightKnee.Y, RightKnee.Z);
                writer.WriteLine("LeftFoot:\t\t{0}\t{1}\t{2}", LeftFoot.X, LeftFoot.Y, LeftFoot.Z);
                writer.WriteLine("RightFoot:\t\t{0}\t{1}\t{2}", RightFoot.X, RightFoot.Y, RightFoot.Z);
            }
        }
    }
}