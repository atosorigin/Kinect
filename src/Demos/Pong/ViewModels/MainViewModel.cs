using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Kinect.Core;
using Kinect.Core.Gestures;
using Kinect.Pong.Models;

namespace Kinect.Pong.ViewModels
{
    internal class MainViewModel : ResourcesViewModelBase
    {
        private static readonly object _syncRoot = new object();
        private readonly ObservableCollection<User> _players;
        private double _cameraSize;
        private ImageSource _cameraView;
        private string _debugInformation;
        private int _fps;
        private CameraView _imageType = Core.CameraView.Color;
        private MyKinect _kinect;
        private User _player;

        private PongGame _pongGame;

        public PongGame PongGame
        {
            get { return _pongGame; }
            set
            {
                if (_pongGame != value)
                {
                    _pongGame = value;
                    RaisePropertyChanged("PongGame");
                }
            }
        }

        public RelayCommand<KeyEventArgs> KeyPress { get; set; }
        public RelayCommand<CancelEventArgs> Closing { get; set; }
        public RelayCommand<SizeChangedEventArgs> SizeChanged { get; set; }

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

        public int Fps
        {
            get
            {
                if (_kinect != null)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() => { _fps = _kinect.Fps; });
                }
                return _fps;
            }
        }

        public string DebugInformation
        {
            get { return _debugInformation; }
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
            PongGame.Scored += PongGame_Scored;
            PongGame.Start();
        }

        private void PongGame_Scored(object sender, ScoreEventArgs e)
        {
            RaisePropertyChanged("PongGame");
        }

        private void SetupKinect()
        {
            _kinect = MyKinect.Instance;
            _kinect.UserCreated += _kinect_UserCreated;
            _kinect.CameraDataUpdated += _kinect_CameraDataUpdated;
            _kinect.StartKinect();
        }

        private void _kinect_UserCreated(object sender, KinectUserEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                                      {
                                                          lock (_syncRoot)
                                                          {
                                                              User kuser = _kinect.GetUser(e.User.Id);
                                                              if (kuser != null)
                                                              {
                                                                  _player = kuser;
                                                                  AccelerationGesture AccelerationGesture =
                                                                      _player.AddAccelerationGesture();
                                                                  AccelerationGesture.AccelerationCalculated +=
                                                                      AccelerationGesture_AccelerationCalculated;
                                                                  if (_players.Count%2 == 0)
                                                                  {
                                                                      PongGame.Paddles.Add(new Paddle(
                                                                                               Paddle.Side.Right, false,
                                                                                               kuser.Id));
                                                                  }
                                                                  else
                                                                  {
                                                                      PongGame.Paddles.Add(new Paddle(Paddle.Side.Left,
                                                                                                      false, kuser.Id));
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

        private void AccelerationGesture_AccelerationCalculated(object sender, AccelerationEventArgs e)
        {
            PongGame.Paddles.First(paddle => paddle.KinectUserID == e.UserID).SetDirection(4*e.DeltaY);
        }

        private void _kinect_CameraDataUpdated(object sender, KinectEventArgs e)
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
                                                                      case Core.CameraView.None:
                                                                          _imageType = Core.CameraView.ColoredDepth;
                                                                          DebugInformation = "Colored Depth";
                                                                          break;
                                                                      case Core.CameraView.ColoredDepth:
                                                                          _imageType = Core.CameraView.Color;
                                                                          DebugInformation = "Color";
                                                                          break;
                                                                      case Core.CameraView.Color:
                                                                          _imageType = Core.CameraView.Depth;
                                                                          DebugInformation = "Depth";
                                                                          break;
                                                                      case Core.CameraView.Depth:
                                                                          _imageType = Core.CameraView.None;
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
                                                                Application.Current.Shutdown();
                                                            });

            SizeChanged =
                new RelayCommand<SizeChangedEventArgs>(
                    e =>
                        {
                            PongGame.Boundry = new Rectangle(0, 0, Convert.ToInt32(e.NewSize.Width),
                                                             Convert.ToInt32(e.NewSize.Height));
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