using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Kinect.Common;
using System.Windows.Media;
using Kinect.Common;
using Kinect.Core;
using System.Windows.Media.Media3D;
using GalaSoft.MvvmLight.Threading;
using System.Windows;
using GalaSoft.MvvmLight;
using Kinect.Core.Gestures;
using System.Diagnostics;
using System.Windows.Threading;
using Kinect.Core.Filters.Helper;

namespace Kinect.WPF.ViewModels
{
    public class UserViewModel : ViewModelBase
    {
        private readonly double _updateMargin = 0.5;
        private static object _syncRoot = new object();
        private User _user;
        private DispatcherTimer _dispatchtimer;
        private int _calibrationCounter = 5;
        private DispatcherTimer _calibrationTimer;
        private int _counter = 0;

        public uint ID
        {
            get { return _user.ID; }
        }

        public Visibility _win = Visibility.Hidden;
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

        private TimeSpan _stopWatch;
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

        private string _calibrationText;
        public string CalibrationText
        {
            get
            {
                return _calibrationText;
            }
            set
            {
                _calibrationText = value;
                RaisePropertyChanged("CalibrationText");
            }
        }

        public SemaphoreViewModel Semaphore { get; private set; }

        private SemaphoreGesture _semaphoreGesture;

        private Color _color;
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

        private Brush _brush;
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
        private Point3D _head;
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

        private Point3D _neck;
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

        private Point3D _torso;
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

        private Point3D _waist;
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

        private Point3D _leftShoulder;
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

        private Point3D _rightShoulder;
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

        private Point3D _leftElbow;
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

        private Point3D _rightElbow;
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

        private Point3D _leftHand;
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

        private Point3D _rightHand;
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

        private Point3D _leftFingertip;
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

        private Point3D _rightFingertip;
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

        private Point3D _leftHip;
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

        private Point3D _rightHip;
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

        private Point3D _leftKnee;
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

        private Point3D _rightKnee;
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

        private Point3D _leftAnkle;
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

        private Point3D _rightAnkle;
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

        private Point3D _leftFoot;
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

        private Point3D _rightFoot;
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
            if (Math.Abs(newValue.X - oldValue.X) >= _updateMargin ||
                Math.Abs(newValue.Y - oldValue.Y) >= _updateMargin ||
                Math.Abs(newValue.Z - oldValue.Z) >= _updateMargin
                )
            {
                return true;
            }
            return false;
        }

        public UserViewModel(uint id)
            : this(new User(id))
        {
        }

        public UserViewModel(User user)
        {
            _user = user;
            _user.Updated += _user_Updated;
            _dispatchtimer = new DispatcherTimer();
            _dispatchtimer.Interval = TimeSpan.FromMilliseconds(1);
            _dispatchtimer.Tick += _dispatchtimer_Tick;
        }

        private void _dispatchtimer_Tick(object sender, EventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                StopWatch = DateTime.Now - _StartDateTime;
                //StopWatch += TimeSpan.FromMilliseconds(1);
            });
        }

        private void _user_Updated(object sender, Core.Eventing.ProcessEventArgs<IUserChangedEvent> e)
        {
            var eventProperties = e.Event.GetType().GetProperties();
            var thisType = this.GetType();

            eventProperties.AsParallel().ForAll(pi =>
            {
                var prop = thisType.GetProperty(pi.Name);
                if (prop != null && prop.CanWrite)
                {
                    var propValue = pi.GetValue(e.Event, null);
                    object newValue = propValue;
                    if (pi.PropertyType == typeof(xn.Point3D))
                    {
                        newValue = ((xn.Point3D)propValue).GetMediaPoint3D();
                    }
                    prop.SetValue(this, newValue, null);
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

        private DateTime _StartDateTime;

        public void SetStopwatch(bool isRunning)
        {
            if (isRunning && !_dispatchtimer.IsEnabled)
            {
                _StartDateTime = DateTime.Now;

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
                AddSelfTouchGesture(FilterHelper.CalculateCorrection(new List<Point3D>() { _user.LeftHand, _user.RightShoulder }));
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
            _user.AddSelfTouchGesture(correction, xn.SkeletonJoint.LeftHand, xn.SkeletonJoint.RightShoulder).SelfTouchDetected += SelfTouch;
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
            var detected = e.Semafoor;
            Semaphore.SemaphoreDetected(detected);
        }

        public void LogToFile()
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter("c:/temp/user.log"))
            {
                writer.WriteLine("Head:\t\t{0}\t{1}\t{2}", this.Head.X, this.Head.Y, this.Head.Z);
                writer.WriteLine("LeftAnkle:\t\t{0}\t{1}\t{2}", this.Neck.X, this.Neck.Y, this.Neck.Z);
                writer.WriteLine("LeftShoulder:\t\t{0}\t{1}\t{2}", this.LeftShoulder.X, this.LeftShoulder.Y, this.LeftShoulder.Z);
                writer.WriteLine("RightShoulder:\t\t{0}\t{1}\t{2}", this.RightShoulder.X, this.RightShoulder.Y, this.RightShoulder.Z);
                writer.WriteLine("Torso:\t\t{0}\t{1}\t{2}", this.Torso.X, this.Torso.Y, this.Torso.Z);
                writer.WriteLine("Waist:\t\t{0}\t{1}\t{2}", this.Waist.X, this.Waist.Y, this.Waist.Z);
                writer.WriteLine("LeftElbow:\t\t{0}\t{1}\t{2}", this.LeftElbow.X, this.LeftElbow.Y, this.LeftElbow.Z);
                writer.WriteLine("RightElbow:\t\t{0}\t{1}\t{2}", this.RightElbow.X, this.RightElbow.Y, this.RightElbow.Z);
                writer.WriteLine("LeftHand:\t\t{0}\t{1}\t{2}", this.LeftHand.X, this.LeftHand.Y, this.LeftHand.Z);
                writer.WriteLine("RightHand:\t\t{0}\t{1}\t{2}", this.RightHand.X, this.RightHand.Y, this.RightHand.Z);
                writer.WriteLine("LeftFingertip:\t\t{0}\t{1}\t{2}", this.LeftFingertip.X, this.LeftFingertip.Y, this.LeftFingertip.Z);
                writer.WriteLine("RightFingertip:\t\t{0}\t{1}\t{2}", this.RightFingertip.X, this.RightFingertip.Y, this.RightFingertip.Z);
                writer.WriteLine("LeftHip:\t\t{0}\t{1}\t{2}", this.LeftHip.X, this.LeftHip.Y, this.LeftHip.Z);
                writer.WriteLine("RightHip:\t\t{0}\t{1}\t{2}", this.RightHip.X, this.RightHip.Y, this.RightHip.Z);
                writer.WriteLine("LeftKnee:\t\t{0}\t{1}\t{2}", this.LeftKnee.X, this.LeftKnee.Y, this.LeftKnee.Z);
                writer.WriteLine("RightKnee:\t\t{0}\t{1}\t{2}", this.RightKnee.X, this.RightKnee.Y, this.RightKnee.Z);
                writer.WriteLine("LeftFoot:\t\t{0}\t{1}\t{2}", this.LeftFoot.X, this.LeftFoot.Y, this.LeftFoot.Z);
                writer.WriteLine("RightFoot:\t\t{0}\t{1}\t{2}", this.RightFoot.X, this.RightFoot.Y, this.RightFoot.Z);
            }
        }
    }
}
