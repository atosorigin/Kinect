using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Common.ColorHelpers;
using Common;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Kinect.Common.ColorHelpers;
using Kinect.Common.Models;
using Kinect.Core;
using System.Windows;
using Kinect.Semaphore;
using Kinect.WPF.Models;
using Kinect.Core.Gestures;
using log4net;

namespace Kinect.WPF.ViewModels
{
    public class MainViewModel : ResourcesViewModelBase
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(MainViewModel));
        
        private static object _syncRoot = new object();

        private enum CameraImageSize { Large, Small }

        private CameraImageSize _imageSize = CameraImageSize.Large;

        private MyKinect _kinect;

        private ColorGenerator _generator = new ColorGenerator();

        private CameraView _imageType = Kinect.Core.CameraView.Color;

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

        private System.Windows.Visibility _debugInformation;
        public System.Windows.Visibility DebugInformation
        {
            get { return _debugInformation; }
            set
            {
                if (_debugInformation != value)
                {
                    _debugInformation = value;
                    RaisePropertyChanged("DebugInformation");
                }
            }
        }

        private string _imageSource;
        public string ImageSource
        {
            get { return _imageSource; }
            set
            {
                if (_imageSource != value)
                {
                    _imageSource = value;
                    RaisePropertyChanged("ImageSource");
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

        private string _marginData;
        public string MarginData
        {
            get
            {
                return _marginData;
            }
            set
            {
                if (value != null && !value.Equals(_marginData))
                {
                    _marginData = value;
                    RaisePropertyChanged("MarginData");
                }
                
            }
        }

        public string FramesPerSecond { get; set; }

        public string UserString { get; set; }

        public ObservableCollection<Message> Messages { get; set; }

        public Message LastMessage
        {
            get { return Messages.LastOrDefault(); }
        }

        public ObservableCollection<UserViewModel> Users { get; set; }
        //private UserViewModel _user;
        //public UserViewModel User
        //{
        //    get { return _user; }
        //    set { _user = value;
        //            RaisePropertyChanged("User");
        //    }
        //}

        public RelayCommand<KeyEventArgs> KeyPress { get; set; }

        public RelayCommand<CancelEventArgs> Closing { get; set; }

        public RelayCommand<DataTransferEventArgs> SourceUpdated { get; set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            LoadValuesFromResource<AppResources>();
            if (IsInDesignMode)
            {
                //Code runs in Blend --> create design time data.
                Messages = new ObservableCollection<Message>();
                Messages.Add(new Message { ImageUrl = "", Value = "Connecting to Kinect..." });
                Messages.Add(new Message { ImageUrl = "", Value = "Searching for user..." });
                Messages.Add(new Message { ImageUrl = "", Value = "User 1 found..." });
                Messages.Add(new Message { ImageUrl = "", Value = "User 1 is in calibration pose..." });
                Messages.Add(new Message { ImageUrl = "", Value = "Calibrating user 1..." });
                Messages.Add(new Message { ImageUrl = "", Value = "User 2 found..." });
                Messages.Add(new Message { ImageUrl = "", Value = "User 2 is in calibration pose..." });
                Messages.Add(new Message { ImageUrl = "", Value = "Calibrating user 2..." });
                Messages.Add(new Message { ImageUrl = "", Value = "Lost connection to user 1..." });
                Messages.Add(new Message { ImageUrl = "", Value = "User 2 waved at Kinect..." });
                Messages.Add(new Message { ImageUrl = "", Value = "User 1 found..." });
                Messages.Add(new Message { ImageUrl = "", Value = "User 1 is in calibration pose..." });
                Messages.Add(new Message { ImageUrl = "", Value = "Calibrating user 1..." });
                Messages.Add(new Message { ImageUrl = "", Value = "User 1 waved at Kinect..." });

                CreateUsers();
            }
            else
            {
                SetCommands();
                ResizeCameraImage();
                Users = new ObservableCollection<UserViewModel>();
                Trace.Listeners.Add(App.TraceListener);

                //TODO: Tracelistener van het type collection maken zodat hij direct gebind kan worden
                Messages = new ObservableCollection<Message>();
                App.TraceListener.CollectionChanged += TraceListener_CollectionChanged;
            }
            MarginData = "X:10,Y:20,Z;30";
        }

        private void TraceListener_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (Message item in e.NewItems)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        Messages.Add(item);
                    });
                }
                RaisePropertyChanged("LastMessage");
            }
        }

        private void SetUpKinect()
        {
            _kinect = Kinect.Core.MyKinect.Instance;
            _kinect.CameraDataUpdated += _kinect_CameraDataUpdated;
            _kinect.PropertyChanged += _kinect_PropertyChanged;
            _kinect.UserCreated += _kinect_UserCreated;
            _kinect.UserRemoved += _kinect_UserRemoved;
            _kinect.UserCalibrating += _kinect_UserCalibrating;
            _kinect.CalibrationFailed += _kinect_CalibrationFailed;
            _kinect.NewUser += _kinect_NewUser;
            _kinect.StartKinect();
        }

        void _kinect_UserCalibrating(object sender, KinectUserEventArgs e)
        {
            if (Users.Count < 1)
            //if(User == null)
            {
                ImageSource = "/Kinect.WPF;component/Images/Step2.png";
            }
        }

        void _kinect_CalibrationFailed(object sender, KinectEventArgs e)
        {
            ImageSource = "/Kinect.WPF;component/Images/Step2Failed.png";
        }

        void _kinect_NewUser(object sender, KinectEventArgs e)
        {
            if (Users.Count < 1)
            //if(User == null)
            {
                ImageSource = "/Kinect.WPF;component/Images/Step1.png";
            }
        }

        private void _kinect_UserRemoved(object sender, KinectUserEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                lock (_syncRoot)
                {
                    if (e.User != null)
                    {
                        var user = Users.SingleOrDefault(ku => ku != null && ku.ID == e.User.ID);
                        if (user != null)
                        {
                            Users.Remove(user);
                        }
                        //if(User.ID == e.User.ID)
                        //{
                        //    User = null;
                        //}
                    }
                }
            });
        }

        private void _kinect_UserCreated(object sender, KinectUserEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                lock (_syncRoot)
                {
                    var kuser = _kinect.GetUser(e.User.ID);
                    if (kuser != null)
                    {
                        var user = CreateUser(kuser);
                        user.AddSemaphoreTracking();
                        
                        Users.Add(user);
                        //User = user;
                        ImageSource = null;
                    }
                }
            });
        }

        private void _kinect_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FPS")
            {
                RaisePropertyChanged(e.PropertyName);
            }
        }

        private void CloseKinect()
        {
            if (_kinect != null)
            {
                _kinect.CameraDataUpdated -= _kinect_CameraDataUpdated;
                _kinect.StopKinect();
                _kinect = null;
                Messages.Clear();
                Users.Clear();
                //User = null;
                CameraView = null;
            }
        }

        private void _kinect_CameraDataUpdated(object sender, KinectEventArgs e)
        {
            SetCameraView();
        }

        private void SetCameraView()
        {
            if (_kinect != null)
            {
                UpdateCameraView(_imageType);
                switch (_imageType)
                {
                    case Kinect.Core.CameraView.Color:
                        CameraVisibility = Visibility.Visible;
                        break;
                    case Kinect.Core.CameraView.Depth:
                        CameraVisibility = Visibility.Visible;
                        break;
                    case Kinect.Core.CameraView.ColoredDepth:
                        CameraVisibility = Visibility.Visible;
                        break;
                    case Kinect.Core.CameraView.None:
                        CameraVisibility = Visibility.Collapsed;
                        break;
                    default:
                        break;
                }
            }
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

        private void ToggleDebugInformation()
        {
            if (DebugInformation == Visibility.Collapsed)
            {
                DebugInformation = Visibility.Visible;
            }
            else
            {
                DebugInformation = Visibility.Collapsed;
            }
        }

        private void ResizeCameraImage()
        {
            if (_imageSize == CameraImageSize.Large)
            {
                _imageSize = CameraImageSize.Small;
                CameraSize = 300;
            }
            else if (_imageSize == CameraImageSize.Small)
            {
                _imageSize = CameraImageSize.Large;
                CameraSize = 150;
            }
        }

        private void SetCommands()
        {
            KeyPress = new RelayCommand<KeyEventArgs>(e =>
            {
                _log.DebugFormat("Key pressed: {0}",e.Key);
                if (e.Key == Key.S)
                {
                    SetUpKinect();
                }
                else if (e.Key == Key.D)
                {
                    ToggleDebugInformation();
                }
                else if (e.Key == Key.Q)
                {
                    CloseKinect();
                    SemaphoreGames.Instance.ResetGameCounter();
                }
                else if (e.Key == Key.C)
                {
                    SetCameraView();
                    switch (_imageType)
                    {
                        case Kinect.Core.CameraView.Depth:
                            _imageType = Kinect.Core.CameraView.ColoredDepth;
                            break;
                        case Kinect.Core.CameraView.ColoredDepth:
                            _imageType = Kinect.Core.CameraView.Color;
                            break;
                        case Kinect.Core.CameraView.Color:
                            _imageType = Kinect.Core.CameraView.None;
                            break;
                        case Kinect.Core.CameraView.None:
                            _imageType = Kinect.Core.CameraView.Depth;
                            break;
                        default:
                            break;
                    }
                }
                else if (e.Key == Key.Z)
                {
                    ResizeCameraImage();
                }
                //else if (e.Key == Key.Up)
                //{
                //    _kinect.MotorUp(100);
                //}
                //else if (e.Key == Key.Down)
                //{
                //    _kinect.MotorDown(100);
                //}
            });

            Closing = new RelayCommand<CancelEventArgs>(e =>
            {
                CloseKinect();
                App.Current.Shutdown();
            });

            SourceUpdated = new RelayCommand<DataTransferEventArgs>(e =>
            {
                if (e.TargetObject is ListView)
                {
                    var lv = e.TargetObject as ListView;
                    lv.ScrollIntoView(lv.Items[lv.Items.Count - 1]);
                }
            });
        }

        private void CreateUsers()
        {
            //Users = new ObservableCollection<UserViewModel>();
            
            ColorGenerator generator = new ColorGenerator();
            Random rand = new Random((int)DateTime.Now.Ticks);
            //User = CreateUser(generator, rand, 1);
            for (uint i = 1; i < 6; i++)
            {
                Users.Add(CreateUser(generator, rand, i));
            }
        }

        private UserViewModel CreateUser(User user)
        {
            return CreateUser(_generator, user);
        }

        private UserViewModel CreateUser(ColorGenerator generator, User user)
        {
            var r = 1f / 255 * user.Color.R;
            var g = 1f / 255 * user.Color.G;
            var b = 1f / 255 * user.Color.B;

            var point = GetPoint3DCoordinates(user.ID, user.Torso.X, user.Torso.Y, user.Torso.Z);

            return new UserViewModel(user)
            {
                Color = Color.FromArgb((byte)0.85f,(byte)r,(byte)g,(byte)b),
                Brush = new RadialGradientBrush(Color.FromScRgb(0.85f, r, g, b), Color.FromScRgb(0.85f, r / 2, g / 2, b / 2)),
                Torso = point
            };
        }

        private UserViewModel CreateUser(ColorGenerator generator, Random rand, uint i)
        {
            string color = generator.NextColorString();
            var r = 1f / 255 * int.Parse(color.Substring(0, 2), NumberStyles.HexNumber);
            var g = 1f / 255 * int.Parse(color.Substring(2, 2), NumberStyles.HexNumber);
            var b = 1f / 255 * int.Parse(color.Substring(4, 2), NumberStyles.HexNumber);

            var point = GetPoint3DCoordinates(i, rand.NextDouble(), rand.NextDouble(), rand.NextDouble());

            return new UserViewModel(i)
            {
                Brush = new RadialGradientBrush(Color.FromScRgb(0.85f, r, g, b), Color.FromScRgb(0.85f, r / 2, g / 2, b / 2)),
                Head = new Point3D(point.X, point.Y - 100, point.Z),
                Neck = new Point3D(point.X, point.Y - 60, point.Z),
                LeftShoulder = new Point3D(point.X - 35, point.Y - 60, point.Z),
                RightShoulder = new Point3D(point.X + 35, point.Y - 60, point.Z),
                LeftElbow = new Point3D(point.X - 55, point.Y, point.Z),
                RightElbow = new Point3D(point.X + 55, point.Y, point.Z),
                LeftHand = new Point3D(point.X - 50, point.Y + 65, point.Z),
                RightHand = new Point3D(point.X + 50, point.Y + 65, point.Z),
                Torso = point,
                LeftHip = new Point3D(point.X - 30, point.Y + 50, point.Z),
                RightHip = new Point3D(point.X + 30, point.Y + 50, point.Z),
                LeftKnee = new Point3D(point.X - 35, point.Y + 130, point.Z),
                RightKnee = new Point3D(point.X + 35, point.Y + 130, point.Z),
                LeftFoot = new Point3D(point.X - 40, point.Y + 210, point.Z),
                RightFoot = new Point3D(point.X + 40, point.Y + 210, point.Z),
            };
        }

        private static Point3D GetPoint3DCoordinates(uint i, double x, double y, double z)
        {
            Point3D point = new Point3D();
            point.X = (i * (x + 25)) / x;
            point.Y = (i * (y + 25)) / y;
            point.Z = i * z;
            return point;
        }
    }
}
