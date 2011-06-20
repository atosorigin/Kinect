using Kinect.Core;
using System.Windows.Media;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Kinect.GestureDetection.ViewModels
{
    public class MainViewModel : ResourcesViewModelBase
    {
        private MyKinect _kinect;

        private static object _syncRoot = new object();
        public ObservableCollection<TrackingViewModel> Users { get; set; }

        private CameraView _imageType = Kinect.Core.CameraView.Color;
        public RelayCommand<KeyEventArgs> KeyPress { get; set; }
        public RelayCommand<CancelEventArgs> Closing { get; set; }

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

        #region functions

        public MainViewModel()
        {
            SetCommands();
            Users = new ObservableCollection<TrackingViewModel>();
        }

        private void SetupKinect()
        {
            _kinect = Kinect.Core.MyKinect.Instance;
            _kinect.UserCreated += _kinect_UserCreated;
            //_kinect.UserRemoved += _kinect_UserRemoved;
            //_kinect.UserCalibrating += _kinect_UserCalibrating;
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
                        Users.Add(new TrackingViewModel(kuser));
                        DebugInformation = "User Created";
                    }
                }
            });
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
                else if (e.Key == Key.T)
                {
   

                }
                else if (e.Key == Key.C)
                {
                    SetCameraView();
                    switch (_imageType)
                    {
                        case Kinect.Core.CameraView.Depth:
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
