using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Kinect.Core;

namespace Kinect.GestureDetection.ViewModels
{
    internal class MainViewModel : ResourcesViewModelBase
    {
        private static readonly object _syncRoot = new object();
        private ImageSource _cameraView;
        private string _debugInformation;
        private int _fps;

        private CameraView _imageType = Core.CameraView.Color;
        private MyKinect _kinect;
        public ObservableCollection<TrackingViewModel> Users { get; set; }
        public RelayCommand<KeyEventArgs> KeyPress { get; set; }
        public RelayCommand<CancelEventArgs> Closing { get; set; }

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

        #region functions

        public MainViewModel()
        {
            SetCommands();
            Users = new ObservableCollection<TrackingViewModel>();
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
                                                                  Users.Add(new TrackingViewModel(kuser));
                                                                  DebugInformation = "User Created";
                                                              }
                                                          }
                                                      });
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
                                                              else if (e.Key == Key.T)
                                                              {
                                                              }
                                                              else if (e.Key == Key.C)
                                                              {
                                                                  SetCameraView();
                                                                  switch (_imageType)
                                                                  {
                                                                      case Core.CameraView.Depth:
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