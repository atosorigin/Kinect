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
        private long _mouseDownCounter, _mouseUpCounter;
        private static readonly ILog Log = LogManager.GetLogger(typeof (MainViewModel));
        private static readonly object SyncRoot = new object();
        private const int EventIntervalInMilliseconds = 500;

        public RelayCommand<KeyEventArgs> KeyPress { get; set; }
        public RelayCommand<CancelEventArgs> Closing { get; set; }
        public RelayCommand<EventArgs> StartGame { get; set; }

        private readonly Size _screenResolution = new Size(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
        private readonly Size _handResolution = new Size(640, 480);
        private Point3D _mouseDownPoint = new Point3D(0, 0, -1); 
        private readonly MyKinect _kinect;
        private User _activeUser;
        private bool _controlMouse = false;
        private bool _mouseDown = false;
        private Process _game;

        private DateTime _clapHit = DateTime.Now;
        private DateTime _clapFilter = DateTime.Now;
        private DateTime _headHit = DateTime.Now;
        

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
                        toggleMouseControl(null, null);
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

                _game.Exited += (s, ea) =>
                {
                    if (_controlMouse) toggleMouseControl(null, null);
                };

                _game.Start();

                if (!_controlMouse)
                {
                    toggleMouseControl(null, null);
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
            _kinect.ElevationAngleInitialPosition = 5;
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
                //var mouseUpAndDownFilter = new CollisionFilter(new Point3D(100, 30, 20), JointID.HandRight, JointID.HipRight);
                var mouseUpAndDownFilter = new CollisionFilter(new Point3D(80, 30, 200), JointID.HandRight, JointID.HipRight);
                var mouseUpAndDownGesture = new SelfTouchGesture(1);

                _activeUser.AttachPipeline(framesFilter);
                framesFilter.AttachPipeline(mouseUpAndDownFilter);
                mouseUpAndDownFilter.AttachPipeline(mouseUpAndDownGesture);

                mouseUpAndDownFilter.Filtered += FireMouseUp;
                mouseUpAndDownGesture.SelfTouchDetected += FireMouseDown;

                //var mouseUpAndDownFilter = new CollisionFilter(new Point3D(100, 30, 20), JointID.HandRight, JointID.HipRight);
                var clickCollisionFilter = new CollisionFilter(new Point3D(125, 40, 150), JointID.HandRight, JointID.Head);
                var clickGesture = new SelfTouchGesture(1);

                _activeUser.AttachPipeline(clickCollisionFilter);
                clickCollisionFilter.AttachPipeline(clickGesture);

                //clickCollisionFilter.Filtered += (s, args) => ShowDebugInfo(args, "Mouse click: ");
                clickGesture.SelfTouchDetected += FireMouseClick;

                //_activeUser.AddSelfTouchGesture(new Point3D(70, 20, 20), JointID.HandRight, JointID.Head).SelfTouchDetected += FireMouseClick;
                //_activeUser.AddSelfTouchGesture(new Point3D(20, 20, 20), JointID.HandRight, JointID.HandLeft).SelfTouchDetected += FireMouseClick;
            }
        }

        void FireMouseUp(object sender, Core.Eventing.FilterEventArgs e)
        {
            //Hands are not on each other
            lock (SyncRoot)
            {
                //ShowDebugInfo(e, "Mouse up ");

                _mouseUpCounter++;
                //only act after 3 hits
                if (_mouseUpCounter < 3) return;
                _mouseDownCounter = 0;
                //Prevent int overflow
                if (_mouseUpCounter > int.MaxValue) _mouseUpCounter = 3;

                //check the time interval
                if (!CheckEventInterval(ref _clapFilter) || !_mouseDown) return;
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
                if (_mouseDownCounter < 3) return;
                _mouseUpCounter = 0;
                //Prevent int overflow
                if (_mouseDownCounter > int.MaxValue) _mouseDownCounter = 3;
                
                //Check the time interval
                if (!CheckEventInterval(ref _clapHit) || _mouseDown) return;
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
                if (!CheckEventInterval(ref _clapHit)) return;
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

        void toggleMouseControl(object sender, SelfTouchEventArgs e)
        {
            lock (SyncRoot)
            {
                if (CheckEventInterval(ref _headHit))
                {
                    //WindowMessage = "Enable / Dissable mouse";
                    //Enable/Disable mouse
                    _controlMouse = !_controlMouse;
                    WindowMessage = "MouseControl : " + _controlMouse.ToString();
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
            lock (SyncRoot)
            {

                var point = e.Event.HandLeft.ToScreenPosition(new Size(640, 480), _screenResolution);
                MouseSimulator.Position = new Point(point.X, point.Y);
                //var point = e.Event.HandLeft.ToScreenPosition(new Size(640, 480), _handResolution);

                //If initial
                //if (_mouseDownPoint.Z == -1)
                //{
                //    _mouseDownPoint = point;
                //}

                //var diff = new Point3D(point.X - _mouseDownPoint.X, point.Y - _mouseDownPoint.Y, 0);
                //_mouseDownPoint = new Point3D(diff.X + point.X, diff.Y + point.Y, 0);
                //MouseSimulator.Position = new Point(_mouseDownPoint.X, _mouseDownPoint.Y);
            }
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
