using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Kinect.Common;
using Kinect.Core;
using Kinect.Core.Filters;
using Kinect.Core.Gestures;
using log4net;
using Microsoft.Research.Kinect.Nui;

namespace Kinect.MouseControl.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (MainViewModel));
        private static readonly object _syncRoot = new object();
        private const int EventIntervalInMilliseconds = 750;

        public RelayCommand<KeyEventArgs> KeyPress { get; set; }
        public RelayCommand<CancelEventArgs> Closing { get; set; }
        private MyKinect _kinect;
        private User _activeUser;
        private bool _controlMouse = false;
        private bool _mouseDown = false;

        private DateTime _clapHit = DateTime.Now;
        private DateTime _clapFilter = DateTime.Now;
        private DateTime _headHit = DateTime.Now;
        

        private string _windowMessage;
        public string WindowMessage
        {
            get { return _windowMessage; }
            set
            {
                lock (_syncRoot)
                {
                    if (value != _windowMessage)
                    {
                        _windowMessage = value;
                        //TODO: Change to RaisePropertyChanged(() => WindowMessage)
                        //when MvvMLight V4 is released
                        RaisePropertyChanged("WindowMessage");
                    }
                }
            }
        }

        private ImageSource _cameraView;
        public ImageSource CameraView
        {
            get { return _cameraView; }
            set
            {
                lock (_syncRoot)
                {
                    if (value != _cameraView)
                    {
                        _cameraView = value;
                        //TODO: Change to RaisePropertyChanged(() => CameraView)
                        //when MvvMLight V4 is released
                        RaisePropertyChanged("CameraView");
                    }
                }
            }
        }

        private Visibility _cameraVisibility;
        public Visibility CameraVisibility
        {
            get { return _cameraVisibility; }
            set
            {
                if (value != _cameraVisibility)
                {
                    _cameraVisibility = value;
                    RaisePropertyChanged("CameraVisibility");
                }
            }
        }

        public MainViewModel()
        {
            _kinect = MyKinect.Instance;
            SetCommands();
            WindowMessage = "Application started";
        }

        private void SetCommands()
        {
            KeyPress = new RelayCommand<KeyEventArgs>(e =>
                {
                    _log.DebugFormat("Key pressed: {0}", e.Key);
                    if (e.Key == Key.S)
                    {
                        SetUpKinect();
                    }
                    else if (e.Key == Key.Q)
                    {
                        CloseKinect();
                    }
                    else if (e.Key == Key.C)
                    {
                        switch (_kinect.CameraViewType)
                        {
                            case Core.CameraView.Depth:
                                _kinect.CameraViewType =
                                    Core.CameraView.ColoredDepth;
                                break;
                            case Core.CameraView.ColoredDepth:
                                _kinect.CameraViewType = Core.CameraView.Color;
                                break;
                            case Core.CameraView.Color:
                                _kinect.CameraViewType = Core.CameraView.None;
                                break;
                            case Core.CameraView.None:
                                _kinect.CameraViewType = Core.CameraView.Depth;
                                break;
                            default:
                                break;
                        }
                        SetCameraView();
                    }
                    else if (e.Key == Key.Up)
                    {
                        _kinect.MotorUp(2);
                    }
                    else if (e.Key == Key.Down)
                    {
                        _kinect.MotorDown(2);
                    }
                });

            Closing = new RelayCommand<CancelEventArgs>(e =>
                {
                    CloseKinect();
                    Application.Current.Shutdown();
                });
        }

        private void SetCameraView()
        {
            if (_kinect != null)
            {
                switch (_kinect.CameraViewType)
                {
                    case Core.CameraView.Color:
                        CameraVisibility = Visibility.Visible;
                        break;
                    case Core.CameraView.Depth:
                        CameraVisibility = Visibility.Visible;
                        break;
                    case Core.CameraView.ColoredDepth:
                        CameraVisibility = Visibility.Visible;
                        break;
                    case Core.CameraView.None:
                        CameraVisibility = Visibility.Collapsed;
                        break;
                    default:
                        break;
                }
            }
        }

        private void CloseKinect()
        {
            _kinect.StopKinect();
        }

        private void SetUpKinect()
        {
            _kinect.CameraDataUpdated += _kinect_CameraDataUpdated;
            //_kinect.PropertyChanged += _kinect_PropertyChanged;
            _kinect.UserCreated += _kinect_UserCreated;
            _kinect.UserRemoved += _kinect_UserRemoved;
            _kinect.StartKinect();
        }

        private void _kinect_UserRemoved(object sender, KinectUserEventArgs e)
        {
            lock (_syncRoot)
            {
                if (_activeUser != null && _activeUser.ID == e.User.ID)
                {
                    _controlMouse = false;
                    _activeUser = null;
                }
            }
        }

        private void _kinect_UserCreated(object sender, KinectUserEventArgs e)
        {
            WindowMessage = "User found";
            lock (_syncRoot)
            {
                if (_activeUser != null)
                {
                    return;
                }
                _activeUser = _kinect.GetUser(e.User.ID);
                _activeUser.Updated += _activeUser_Updated;
                _activeUser.AddSelfTouchGesture(new Point3D(0, 0, 0), JointID.HandRight, JointID.Head).SelfTouchDetected += gesture_SelfTouchDetected;
                
                var framesFilter  = new FramesFilter(6);
                var clapFilter = new CollisionFilter(new Point3D(50,50,300), JointID.HandLeft, JointID.HandRight);
                clapFilter.Filtered += clapFilter_Filtered;
                var clapGesture = new SelfTouchGesture(1);
                clapGesture.SelfTouchDetected += clapGesture_SelfTouchDetected;
                _activeUser.AttachPipeline(framesFilter);
                framesFilter.AttachPipeline(clapFilter);
                clapFilter.AttachPipeline(clapGesture);

            }
        }

        void clapFilter_Filtered(object sender, Core.Eventing.FilterEventArgs e)
        {
            //Hands are not on each other
            lock (_syncRoot)
            {
                //First check the time interval to ignore most of the messages
                if (CheckEventInterval(ref _clapFilter) && _mouseDown)
                {
                    WindowMessage = "Mouse up";
                    _mouseDown = false;
                    MouseSimulator.MouseUp(System.Windows.Input.MouseButton.Left);
                }
            }
        }

        void clapGesture_SelfTouchDetected(object sender, SelfTouchEventArgs e)
        {
            lock (_syncRoot)
            {            
                //Left button down
                if (CheckEventInterval(ref _clapHit))
                {
                    WindowMessage = "Mouse down";
                    _mouseDown = true;
                    MouseSimulator.MouseDown(System.Windows.Input.MouseButton.Left);
                }
            }
        }

        void gesture_SelfTouchDetected(object sender, SelfTouchEventArgs e)
        {
            lock (_syncRoot)
            {
                if (CheckEventInterval(ref _headHit))
                {
                    WindowMessage = "Enable / Dissable mouse";
                    //Enable/Disable mouse
                    _controlMouse = !_controlMouse;
                }
            }
        }

        void _activeUser_Updated(object sender, Core.Eventing.ProcessEventArgs<IUserChangedEvent> e)
        {
            //WindowMessage = e.Event.Head.Z.ToString();
            if (!_controlMouse)
            {
                return;
            }
            var screen = new Size(System.Windows.SystemParameters.PrimaryScreenWidth,
                                  System.Windows.SystemParameters.PrimaryScreenHeight);

            var point = e.Event.RightHand.ToScreenPosition(new Size(320, 240), screen,new Point(50,50),new Size(160,120));
            MouseSimulator.Position = new Point(point.X,point.Y);
        }

        private void _kinect_CameraDataUpdated(object sender, KinectCameraEventArgs e)
        {
            if (_kinect != null)
            {
                CameraView = e.Image;
            }
        }

        private bool CheckEventInterval(ref DateTime lastHit)
        {
            if ((DateTime.Now - lastHit).TotalMilliseconds > EventIntervalInMilliseconds)
            {
                lastHit = DateTime.Now;
                return true;
            }
            return false;
        }
    }
}
