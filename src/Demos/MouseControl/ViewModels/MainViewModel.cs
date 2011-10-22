using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Kinect.Common;
using Kinect.Core;
using Kinect.Core.Eventing;
using Kinect.Core.Filters;
using Kinect.Core.Gestures;
using log4net;
using Microsoft.Research.Kinect.Nui;

namespace Kinect.MouseControl.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private enum ControlMode { MouseControl, AngryBirds };
        private ControlMode _currentMode = ControlMode.AngryBirds;

        private long _mouseDownCounter, _mouseUpCounter;
        private static readonly ILog Log = LogManager.GetLogger(typeof (MainViewModel));
        private static readonly object SyncRoot = new object();
        private const int MouseButtonsIntervalInMilliseconds = 100;
        private const int SwitchModeEventInterval = 2000;
        private const int NoiseFilter = 3;

        public RelayCommand<KeyEventArgs> KeyPress { get; set; }
        public RelayCommand<CancelEventArgs> Closing { get; set; }
        public RelayCommand<EventArgs> StartGame { get; set; }

        private readonly Size _screenResolution = new Size(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
        private readonly MyKinect _kinect;
        private User _activeUser;
        private bool _controlMouse = false;
        private bool _mouseDown = false;
        private Process _game;

        private DateTime _mouseDownHit = DateTime.Now;
        private DateTime _mouseUpHit = DateTime.Now;
        private DateTime _mouseClickHit = DateTime.Now;
        private DateTime _switchModeHit = DateTime.Now;

        //Filters and gestures
        private CollisionFilter _lefthandRighthandCollision, _lefthandHeadCollision, _righthandHeadCollision,_righthandLeftShoulderCollision, _righthandRightHipCollision;
        private SelfTouchGesture _lefthandRighthandGesture, _lefthandHeadGesture, _righthandHeadGesture, _righthandLeftShoulderGesture, _righthandRightHipGesture;
        

        private string _windowMessage;
        public string WindowMessage
        {
            get { return _windowMessage; }
            set
            {
                lock (SyncRoot)
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
                lock (SyncRoot)
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
            SetUpKinect();
        }

        private void SetCommands()
        {
            KeyPress = new RelayCommand<KeyEventArgs>(e =>
                {
                    Log.DebugFormat("Key pressed: {0}", e.Key);
                    if (e.Key == Key.Q)
                    {
                        CloseKinect();
                        Application.Current.MainWindow.Close();
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
                    else if (e.Key == Key.M)
                    {
                        ToggleMouseControl();
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
                    if (_game != null && !_game.HasExited)
                    {
                        _game.Kill();
                    }
                    Application.Current.Shutdown();
                });

            StartGame = new RelayCommand<EventArgs>(e =>
            {
                _game = new Process
                            {
                                StartInfo = {FileName = ConfigurationManager.AppSettings["GameUri"]},
                                EnableRaisingEvents = true
                            };
                var overlay = new AtosOverlay();
                overlay.BringIntoView(new Rect(10, 10, 100, 100));
                overlay.Activate();
                overlay.Show();
                _game.Exited += (s, ea) =>
                {
                    if (_controlMouse) ToggleMouseControl();
                };

                _game.Start();

                if (!_controlMouse)
                {
                    ToggleMouseControl();
                }
                    
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
            _kinect.ChangeMaxSkeletonPositions(.6f, .6f);
            _kinect.ElevationAngleInitialPosition = 15;
            _kinect.CameraDataUpdated += _kinect_CameraDataUpdated;
            //_kinect.PropertyChanged += _kinect_PropertyChanged;
            _kinect.UserCreated += _kinect_UserCreated;
            _kinect.UserRemoved += _kinect_UserRemoved;
            _kinect.StartKinect();
        }

        private void _kinect_UserRemoved(object sender, KinectUserEventArgs e)
        {
            lock (SyncRoot)
            {
                if (_activeUser != null && _activeUser.Id == e.User.Id)
                {
                    _controlMouse = false;
                    _activeUser = null;
                }
            }
        }

        private void _kinect_UserCreated(object sender, KinectUserEventArgs e)
        {
            WindowMessage = "User found";
            lock (SyncRoot)
            {
                if (_activeUser != null)
                {
                    return;
                }
                _activeUser = _kinect.GetUser(e.User.Id);
                _activeUser.Updated += _activeUser_Updated;

                var framesFilter = new FramesFilter(15);

                //Initialize filters
                _lefthandRighthandCollision = new CollisionFilter(new Point3D(100, 50, 130), JointID.HandLeft, JointID.HandRight);
                _lefthandHeadCollision = new CollisionFilter(new Point3D(150, 30, 500), JointID.HandLeft, JointID.Head);
                _righthandHeadCollision = new CollisionFilter(new Point3D(125, 40, 150), JointID.HandRight, JointID.Head);
                _righthandLeftShoulderCollision = new CollisionFilter(new Point3D(50, 50, 300), JointID.HandRight, JointID.ShoulderLeft);
                _righthandRightHipCollision = new CollisionFilter(new Point3D(80, 30, 200), JointID.HandRight, JointID.HipRight);

                //Initialize gestures
                _lefthandRighthandGesture = new SelfTouchGesture(1);
                _lefthandHeadGesture = new SelfTouchGesture(1);
                _righthandHeadGesture = new SelfTouchGesture(1);
                _righthandLeftShoulderGesture = new SelfTouchGesture(1);
                _righthandRightHipGesture = new SelfTouchGesture(1);

                //Attach filters and gestures
                _activeUser.AttachPipeline(framesFilter);
                framesFilter.AttachPipeline(_lefthandRighthandCollision);
                framesFilter.AttachPipeline(_lefthandHeadCollision);
                framesFilter.AttachPipeline(_righthandHeadCollision);
                framesFilter.AttachPipeline(_righthandLeftShoulderCollision);
                framesFilter.AttachPipeline(_righthandRightHipCollision);
                _lefthandRighthandCollision.AttachPipeline(_lefthandRighthandGesture);
                _lefthandHeadCollision.AttachPipeline(_lefthandHeadGesture);
                _righthandHeadCollision.AttachPipeline(_righthandHeadGesture);
                _righthandLeftShoulderCollision.AttachPipeline(_righthandLeftShoulderGesture);
                _righthandRightHipCollision.AttachPipeline(_righthandRightHipGesture);

                _righthandLeftShoulderGesture.SelfTouchDetected += SwitchMode;

                //Debug info
                //_righthandLeftShoulderCollision.Filtered += (s, args) => ShowDebugInfo(args, "Filter info: ");

                SwitchMode(null, null);
            }
        }

        void SwitchMode(object sender, GestureEventArgs e)
        {
            if (!CheckEventInterval(ref _switchModeHit, SwitchModeEventInterval)) return;

            switch (_currentMode)
            {
                case ControlMode.MouseControl: _currentMode = ControlMode.AngryBirds; break;
                case ControlMode.AngryBirds: _currentMode = ControlMode.MouseControl; break;
                default: return;
            }

            WindowMessage = "Switching mode to " + _currentMode;

            UnBindAllGestures();
            switch (_currentMode)
            {
                case ControlMode.MouseControl:
                    _righthandRightHipCollision.Filtered += FireMouseUp;
                    _righthandRightHipGesture.SelfTouchDetected += FireMouseDown;
                    _righthandHeadGesture.SelfTouchDetected += FireMouseClick;
                break;
                case ControlMode.AngryBirds:
                    _lefthandRighthandCollision.Filtered += FireMouseUp;
                    _lefthandRighthandGesture.SelfTouchDetected += FireMouseDown;
                    _lefthandHeadGesture.SelfTouchDetected += FireMouseClick;
                break;
            }
        }

        private void UnBindAllGestures()
        {
            _righthandRightHipCollision.Filtered -= FireMouseUp;
            _righthandRightHipGesture.SelfTouchDetected -= FireMouseDown;
            _righthandHeadGesture.SelfTouchDetected -= FireMouseClick;

            _lefthandRighthandCollision.Filtered -= FireMouseUp;
            _lefthandRighthandGesture.SelfTouchDetected -= FireMouseDown;
            _lefthandHeadGesture.SelfTouchDetected -= FireMouseClick;
        }

        void FireMouseUp(object sender, Core.Eventing.FilterEventArgs e)
        {
            //Hands are not on each other
            lock (SyncRoot)
            {
                //ShowDebugInfo(e, "Mouse up ");

                _mouseUpCounter++;
                //only act after 3 hits
                if (_mouseUpCounter < NoiseFilter) return;
                _mouseDownCounter = 0;
                //Prevent int overflow
                if (_mouseUpCounter > int.MaxValue) _mouseUpCounter = NoiseFilter;

                //check the time interval
                if (!CheckEventInterval(ref _mouseUpHit, MouseButtonsIntervalInMilliseconds) || !_mouseDown) return;
                _mouseDown = false;
                WindowMessage = "Mouse up";

                if (!_controlMouse) return;
                MouseSimulator.MouseUp(System.Windows.Input.MouseButton.Left);
            }
        }

        void FireMouseDown(object sender, SelfTouchEventArgs e)
        {
            lock (SyncRoot)
            {
                _mouseDownCounter++;
                //only act after 3 hits
                if (_mouseDownCounter < NoiseFilter) return;
                _mouseUpCounter = 0;
                //Prevent int overflow
                if (_mouseDownCounter > int.MaxValue) _mouseDownCounter = NoiseFilter;
                
                //Check the time interval
                if (!CheckEventInterval(ref _mouseDownHit, MouseButtonsIntervalInMilliseconds) || _mouseDown) return;
                _mouseDown = true;
                WindowMessage = "Mouse down";

                if (!_controlMouse) return;
                MouseSimulator.MouseDown(System.Windows.Input.MouseButton.Left);
            }
        }

        void FireMouseClick(object sender, SelfTouchEventArgs e)
        {
            lock (SyncRoot)
            {
                //Left button down
                if (!CheckEventInterval(ref _mouseClickHit, MouseButtonsIntervalInMilliseconds)) return;
                WindowMessage = "Mouse click";
                if (!_controlMouse) return;
                MouseSimulator.MouseDown(System.Windows.Input.MouseButton.Left);
                MouseSimulator.MouseUp(System.Windows.Input.MouseButton.Left);
                _mouseDown = false;
            }
        }

        void ShowDebugInfo(FilterEventArgs args, string message)
        {
            var eventArgs = args as CollisionFilterEventArgs;
            if (eventArgs != null && eventArgs.MarginData.Length > 0)
            {
                WindowMessage = message + eventArgs.MarginData[0].GetDebugString();
            }
        }

        void ToggleMouseControl()
        {
            lock (SyncRoot)
            {
                //Enable/Disable mouse
                _controlMouse = !_controlMouse;
                WindowMessage = "MouseControl : " + _controlMouse.ToString();
            }
        }

        void _activeUser_Updated(object sender, Core.Eventing.ProcessEventArgs<IUserChangedEvent> e)
        {
            //WindowMessage = e.Event.Head.Z.ToString();
            if (!_controlMouse)
            {
                return;
            }
            lock (SyncRoot)
            {
                Point3D point;
                switch(_currentMode)
                {
                    case ControlMode.MouseControl:
                        point = e.Event.HandLeft.ToScreenPosition(new Size(640, 480), _screenResolution);
                        break;
                    case ControlMode.AngryBirds:
                        point = e.Event.Spine.ToScreenPosition(new Size(640, 480), _screenResolution);
                        break;
                    default: return;
                }
                MouseSimulator.Position = new Point(point.X, point.Y);
            }
        }

        private void _kinect_CameraDataUpdated(object sender, KinectCameraEventArgs e)
        {
            if (_kinect != null)
            {
                CameraView = e.Image;
            }
        }

        private bool CheckEventInterval(ref DateTime lastHit, int interval)
        {
            if ((DateTime.Now - lastHit).TotalMilliseconds > interval)
            {
                lastHit = DateTime.Now;
                return true;
            }
            return false;
        }
    }
}
