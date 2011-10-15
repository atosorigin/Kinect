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
        private readonly DispatcherTimer _dispatchtimer;
        private const double UpdateMargin = 0.5;
        private const double PropertionModifier = 1.2;
        private readonly User _user;
        private DateTime _startDateTime;
        private Brush _brush;
        private int _calibrationCounter = 5;
        private string _calibrationText;
        private DispatcherTimer _calibrationTimer;
        private Color _color;
        private int _counter;
        private Point3D _head;
        private Point3D _ankleLeft;
        private Point3D _elbowLeft;
        private Point3D _wristLeft;
        private Point3D _footLeft;
        private Point3D _handLeft;
        private Point3D _hipLeft;
        private Point3D _kneeLeft;
        private Point3D _shoulderLeft;
        private Point3D _shoulderCenter;
        private Point3D _ankleRight;
        private Point3D _elbowRight;
        private Point3D _wristRight;
        private Point3D _footRight;
        private Point3D _handRight;
        private Point3D _hipRight;
        private Point3D _kneeRight;
        private Point3D _shoulderRight;
        private Point3D _spine;
        private Point3D _hipCenter;
        private SemaphoreGesture _semaphoreGesture;
        private TimeSpan _stopWatch;

        private Visibility _win = Visibility.Hidden;

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

        public int Id
        {
            get { return _user.Id; }
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

        public Point3D ShoulderCenter
        {
            get { return _shoulderCenter; }
            set
            {
                if (Changed(value, _head))
                {
                    _shoulderCenter = value;
                    RaisePropertyChanged("ShoulderCenter");
                }
            }
        }

        public Point3D Spine
        {
            get { return _spine; }
            set
            {
                if (Changed(value, _spine))
                {
                    _spine = value;
                    RaisePropertyChanged("Spine");
                }
            }
        }

        public Point3D HipCenter
        {
            get { return _hipCenter; }
            set
            {
                if (Changed(value, _hipCenter))
                {
                    _hipCenter = value;
                    RaisePropertyChanged("HipCenter");
                }
            }
        }

        public Point3D ShoulderLeft
        {
            get { return _shoulderLeft; }
            set
            {
                if (Changed(value, _shoulderLeft))
                {
                    _shoulderLeft = value;
                    RaisePropertyChanged("ShoulderLeft");
                }
            }
        }

        public Point3D ShoulderRight
        {
            get { return _shoulderRight; }
            set
            {
                if (Changed(value, _shoulderRight))
                {
                    _shoulderRight = value;
                    RaisePropertyChanged("ShoulderRight");
                }
            }
        }

        public Point3D ElbowLeft
        {
            get { return _elbowLeft; }
            set
            {
                if (Changed(value, _elbowLeft))
                {
                    _elbowLeft = value;
                    RaisePropertyChanged("ElbowLeft");
                }
            }
        }

        public Point3D ElbowRight
        {
            get { return _elbowRight; }
            set
            {
                if (Changed(value, _elbowRight))
                {
                    _elbowRight = value;
                    RaisePropertyChanged("ElbowRight");
                }
            }
        }

        public Point3D HandLeft
        {
            get { return _handLeft; }
            set
            {
                if (Changed(value, _handLeft))
                {
                    _handLeft = value;
                    RaisePropertyChanged("HandLeft");
                }
            }
        }

        public Point3D HandRight
        {
            get { return _handRight; }
            set
            {
                if (Changed(value, _handRight))
                {
                    _handRight = value;
                    RaisePropertyChanged("HandRight");
                }
            }
        }

        public Point3D WristLeft
        {
            get { return _wristLeft; }
            set
            {
                if (Changed(value, _wristLeft))
                {
                    _wristLeft = value;
                    RaisePropertyChanged("WristLeft");
                }
            }
        }

        public Point3D WristRight
        {
            get { return _wristRight; }
            set
            {
                if (Changed(value, _wristRight))
                {
                    _wristRight = value;
                    RaisePropertyChanged("WristRight");
                }
            }
        }

        public Point3D HipLeft
        {
            get { return _hipLeft; }
            set
            {
                if (Changed(value, _hipLeft))
                {
                    _hipLeft = value;
                    RaisePropertyChanged("HipLeft");
                }
            }
        }

        public Point3D HipRight
        {
            get { return _hipRight; }
            set
            {
                if (Changed(value, _hipRight))
                {
                    _hipRight = value;
                    RaisePropertyChanged("HipRight");
                }
            }
        }

        public Point3D KneeLeft
        {
            get { return _kneeLeft; }
            set
            {
                if (Changed(value, _kneeLeft))
                {
                    _kneeLeft = value;
                    RaisePropertyChanged("KneeLeft");
                }
            }
        }

        public Point3D KneeRight
        {
            get { return _kneeRight; }
            set
            {
                if (Changed(value, _kneeRight))
                {
                    _kneeRight = value;
                    RaisePropertyChanged("KneeRight");
                }
            }
        }

        public Point3D AnkleLeft
        {
            get { return _ankleLeft; }
            set
            {
                if (Changed(value, _ankleLeft))
                {
                    _ankleLeft = value;
                    RaisePropertyChanged("AnkleLeft");
                }
            }
        }

        public Point3D AnkleRight
        {
            get { return _ankleRight; }
            set
            {
                if (Changed(value, _ankleRight))
                {
                    _ankleRight = value;
                    RaisePropertyChanged("AnkleRight");
                }
            }
        }

        public Point3D FootLeft
        {
            get { return _footLeft; }
            set
            {
                if (Changed(value, _footLeft))
                {
                    _footLeft = value;
                    RaisePropertyChanged("FootLeft");
                }
            }
        }

        public Point3D FootRight
        {
            get { return _footRight; }
            set
            {
                if (Changed(value, _footRight))
                {
                    _footRight = value;
                    RaisePropertyChanged("FootRight");
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
                        var point = (Point3D) propValue;
                        var newValue = new Point3D(point.X * PropertionModifier, point.Y * PropertionModifier, point.Z * PropertionModifier);

                        prop.SetValue(this, newValue, null);
                    }
                }
            });
        }

        internal void AddSemaphoreTracking()
        {
            Semaphore = new SemaphoreViewModel();
            Semaphore.PropertyChanged += SemaphorePropertyChanged;
            _semaphoreGesture = _user.AddSemaphoreTouchGesture();
            _semaphoreGesture.SemafoorDetected += _semaphoreGesture_SemafoorDetected;
        }

        private void SemaphorePropertyChanged(object sender, PropertyChangedEventArgs e)
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
                    FilterHelper.CalculateCorrection(new List<Point3D> { _user.HandLeft, _user.ShoulderRight }));
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
                writer.WriteLine("ShoulderCenter:\t\t{0}\t{1}\t{2}", ShoulderCenter.X, ShoulderCenter.Y, ShoulderCenter.Z);
                writer.WriteLine("ShoulderLeft:\t\t{0}\t{1}\t{2}", ShoulderLeft.X, ShoulderLeft.Y, ShoulderLeft.Z);
                writer.WriteLine("ShoulderRight:\t\t{0}\t{1}\t{2}", ShoulderRight.X, ShoulderRight.Y, ShoulderRight.Z);
                writer.WriteLine("Spine:\t\t{0}\t{1}\t{2}", Spine.X, Spine.Y, Spine.Z);
                writer.WriteLine("HipCenter:\t\t{0}\t{1}\t{2}", HipCenter.X, HipCenter.Y, HipCenter.Z);
                writer.WriteLine("ElbowLeft:\t\t{0}\t{1}\t{2}", ElbowLeft.X, ElbowLeft.Y, ElbowLeft.Z);
                writer.WriteLine("ElbowRight:\t\t{0}\t{1}\t{2}", ElbowRight.X, ElbowRight.Y, ElbowRight.Z);
                writer.WriteLine("HandLeft:\t\t{0}\t{1}\t{2}", HandLeft.X, HandLeft.Y, HandLeft.Z);
                writer.WriteLine("HandRight:\t\t{0}\t{1}\t{2}", HandRight.X, HandRight.Y, HandRight.Z);
                writer.WriteLine("WristLeft:\t\t{0}\t{1}\t{2}", WristLeft.X, WristLeft.Y, WristLeft.Z);
                writer.WriteLine("WristRight:\t\t{0}\t{1}\t{2}", WristRight.X, WristRight.Y, WristRight.Z);
                writer.WriteLine("HipLeft:\t\t{0}\t{1}\t{2}", HipLeft.X, HipLeft.Y, HipLeft.Z);
                writer.WriteLine("HipRight:\t\t{0}\t{1}\t{2}", HipRight.X, HipRight.Y, HipRight.Z);
                writer.WriteLine("KneeLeft:\t\t{0}\t{1}\t{2}", KneeLeft.X, KneeLeft.Y, KneeLeft.Z);
                writer.WriteLine("KneeRight:\t\t{0}\t{1}\t{2}", KneeRight.X, KneeRight.Y, KneeRight.Z);
                writer.WriteLine("FootLeft:\t\t{0}\t{1}\t{2}", FootLeft.X, FootLeft.Y, FootLeft.Z);
                writer.WriteLine("FootRight:\t\t{0}\t{1}\t{2}", FootRight.X, FootRight.Y, FootRight.Z);
            }
        }
    }
}