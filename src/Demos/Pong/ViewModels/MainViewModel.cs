using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Kinect.Core;
using Kinect.Core.Gestures;
using Kinect.WPF.nPong.Models;
using System.Drawing;
using System.Windows.Media.Media3D;
using System.Windows;



namespace Kinect.WPF.nPong.ViewModels
{
    class MainViewModel : ResourcesViewModelBase
    {
        private MyKinect _kinect;
        private User _player;

        private ObservableCollection<User> _players;

        private static object _syncRoot = new object();

        private PongGame _pongGame;
        public PongGame PongGame
        {
            get
            {
                return _pongGame;
            }
            set
            {
                if (_pongGame != value)
                {
                    _pongGame = value;
                    RaisePropertyChanged("PongGame");
                }
            }
        }

        private CameraView _imageType = Kinect.Core.CameraView.Color;
        public RelayCommand<KeyEventArgs> KeyPress { get; set; }
        public RelayCommand<CancelEventArgs> Closing { get; set; }
        public RelayCommand<SizeChangedEventArgs> SizeChanged { get; set; }

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
                        RaisePropertyChanged("CameraView");
                    }
                }
            }
        }

        private double _cameraSize;
        public double CameraSize
        {
            get { return _cameraSize; }
            set
            {
                if (value != _cameraSize)
                {
                    _cameraSize = value;
                    RaisePropertyChanged("CameraSize");
                }
            }
        }

        private int _fps = 0;
        public int FPS
        {
            get
            {
                if (_kinect != null)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        _fps = _kinect.FPS;
                    });
                }
                return _fps;
            }
        }

        private string _debugInformation;
        public string DebugInformation
        {
            get
            {
                return _debugInformation;
            }
            set
            {
                if (value != _debugInformation)
                {
                    _debugInformation = value;
                    RaisePropertyChanged("DebugInformation");
                }
            }
        }

        #region Functions

        public MainViewModel()
        {
            SetCommands();
            _players = new ObservableCollection<User>();
            PongGame = PongGame.Instance;
            PongGame.Boundry = new Rectangle(0, 0, 0, 0);
            PongGame.Scored += new EventHandler<ScoreEventArgs>(PongGame_Scored);
            PongGame.Start();
        }

        void PongGame_Scored(object sender, ScoreEventArgs e)
        {
            RaisePropertyChanged("PongGame");
        }

        private void SetupKinect()
        {
            _kinect = Kinect.Core.MyKinect.Instance;
            _kinect.UserCreated += _kinect_UserCreated;
            _kinect.NewUser += _kinect_NewUser;
            _kinect.CameraDataUpdated += _kinect_CameraDataUpdated;
            _kinect.StartKinect();
        }

        void _kinect_UserCreated(object sender, KinectUserEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                lock (_syncRoot)
                {
                    var kuser = _kinect.GetUser(e.User.ID);
                    if (kuser != null)
                    {
                        _player = kuser;
                        var AccelerationGesture = _player.AddAccelerationGesture();
                        AccelerationGesture.AccelerationCalculated += AccelerationGesture_AccelerationCalculated;
                        if (_players.Count % 2 == 0)
                        {
                            PongGame.Paddles.Add(new Paddle(Paddle.Side.Right, false, (int)kuser.ID));
                        }
                        else
                        {
                            PongGame.Paddles.Add(new Paddle(Paddle.Side.Left, false, (int)kuser.ID));
                        }
                        _players.Add(_player);
                        if (PongGame.Paddles.Count == 2)
                        {
                            PongGame.AddBall();
                        }
                    }
                    DebugInformation = "User Created";

                }
            });
        }

        void AccelerationGesture_AccelerationCalculated(object sender, AccelerationEventArgs e)
        {
            PongGame.Paddles.First(paddle => paddle.KinectUserID == e.UserID).SetDirection(4 * e.DeltaY);
        }

        void _kinect_NewUser(object sender, KinectEventArgs e)
        {
            DebugInformation = "New user in field of camera";
        }

        void _kinect_CameraDataUpdated(object sender, KinectEventArgs e)
        {
            SetCameraView();
        }

        private void UpdateCameraView(CameraView view)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (_kinect != null)
                {
                    CameraView = _kinect.GetCameraView(view);
                }
            });
        }

        private void SetCommands()
        {
            KeyPress = new RelayCommand<KeyEventArgs>(e =>
            {
                if (e.Key == Key.S)
                {
                    DebugInformation = "Kinect starting...";
                    SetupKinect();
                }
                else if (e.Key == Key.Q)
                {
                    CloseKinect();
                }
                else if (e.Key == Key.Y)
                {
                    PongGame.AddPaddle(Paddle.Side.Left, true, -1);
                }
                else if (e.Key == Key.U)
                {
                    PongGame.AddPaddle(Paddle.Side.Right, true, -1);
                }
                else if (e.Key == Key.I)
                {
                    PongGame.Reset();
                }
                else if (e.Key == Key.K)
                {
                    CloseKinect();
                }
                else if (e.Key == Key.B)
                {
                    PongGame.AddBall();
                }
                else if (e.Key == Key.P)
                {
                    PongGame.AddBall();
                    PongGame.Start();
                }
                else if (e.Key == Key.C)
                {
                    SetCameraView();
                    switch (_imageType)
                    {
                        case Kinect.Core.CameraView.None:
                            _imageType = Kinect.Core.CameraView.ColoredDepth;
                            DebugInformation = "Colored Depth";
                            break;
                        case Kinect.Core.CameraView.ColoredDepth:
                            _imageType = Kinect.Core.CameraView.Color;
                            DebugInformation = "Color";
                            break;
                        case Kinect.Core.CameraView.Color:
                            _imageType = Kinect.Core.CameraView.Depth;
                            DebugInformation = "Depth";
                            break;
                        case Kinect.Core.CameraView.Depth:
                            _imageType = Kinect.Core.CameraView.None;
                            DebugInformation = "";
                            break;
                        default:
                            break;
                    }
                }
            });

            Closing = new RelayCommand<CancelEventArgs>(e =>
            {
                CloseKinect();
                App.Current.Shutdown();
            });

            SizeChanged = new RelayCommand<SizeChangedEventArgs>(e =>
            {
                PongGame.Boundry = new Rectangle(0, 0, Convert.ToInt32(e.NewSize.Width), Convert.ToInt32(e.NewSize.Height));
            });

        }

        private void CloseKinect()
        {
            if (_kinect != null)
            {
                _kinect.CameraDataUpdated -= _kinect_CameraDataUpdated;
                _kinect.StopKinect();
                _kinect = null;
                CameraView = null;
            }
        }

        private void SetCameraView()
        {
            if (_kinect != null)
            {
                UpdateCameraView(_imageType);
            }
        }
        #endregion
    }
}
