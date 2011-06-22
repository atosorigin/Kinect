using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Kinect.Core;
using Kinect.Core.Gestures;
using log4net;
using Microsoft.Research.Kinect.Nui;

namespace Kinect.MouseControl.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (MainViewModel));
        private static readonly object _syncRoot = new object();

        public RelayCommand<KeyEventArgs> KeyPress { get; set; }
        public RelayCommand<CancelEventArgs> Closing { get; set; }
        private MyKinect _kinect;
        private User _activeUser;
        private SelfTouchGesture clapGesture;
        private bool _controlMouse = false;

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
                clapGesture = _activeUser.AddSelfTouchGesture(new Point3D(0, 0, 0), JointID.HandLeft, JointID.HandRight);
                clapGesture.SelfTouchDetected += clapGesture_SelfTouchDetected;
            }
        }

        void clapGesture_SelfTouchDetected(object sender, SelfTouchEventArgs e)
        {
            //Left button down
            WindowMessage = "Mouse down";
        }

        void gesture_SelfTouchDetected(object sender, SelfTouchEventArgs e)
        {
            WindowMessage = "Enable / Dissable mouse";
            //Enable/Disable mouse
            _controlMouse = !_controlMouse;
        }

        void _activeUser_Updated(object sender, Core.Eventing.ProcessEventArgs<IUserChangedEvent> e)
        {
            if (!_controlMouse)
            {
                return;
            }
        }

        private void _kinect_CameraDataUpdated(object sender, KinectCameraEventArgs e)
        {
            if (_kinect != null)
            {
                CameraView = e.Image;
            }
        }
    }
}
