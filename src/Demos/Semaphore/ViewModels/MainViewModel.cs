using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Kinect.Common.ColorHelpers;
using Kinect.Common.Models;
using Kinect.Core;
using Kinect.Semaphore.Models;
using log4net;

namespace Kinect.Semaphore.ViewModels
{
    public class MainViewModel : ResourcesViewModelBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (MainViewModel));

        private static readonly object SyncRoot = new object();

        private readonly ColorGenerator _generator = new ColorGenerator();
        private double _cameraSize;

        private ImageSource _cameraView;

        private Visibility _cameraVisibility;
        private Visibility _debugInformation;
        private int _fps;
        private CameraImageSize _imageSize = CameraImageSize.Large;
        private string _imageSource;
        private CameraView _imageType = Core.CameraView.Color;
        private MyKinect _kinect;
        private string _marginData;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            LoadValuesFromResource<AppResources>();
            if (IsInDesignMode)
            {
                //Code runs in Blend --> create design time data.
                Messages = new ObservableCollection<Message>
                {
                    new Message {ImageUrl = "", Value = "Connecting to Kinect..."},
                    new Message {ImageUrl = "", Value = "Searching for user..."},
                    new Message {ImageUrl = "", Value = "User 1 found..."},
                    new Message {ImageUrl = "", Value = "User 1 is in calibration pose..."},
                    new Message {ImageUrl = "", Value = "Calibrating user 1..."},
                    new Message {ImageUrl = "", Value = "User 2 found..."},
                    new Message {ImageUrl = "", Value = "User 2 is in calibration pose..."},
                    new Message {ImageUrl = "", Value = "Calibrating user 2..."},
                    new Message {ImageUrl = "", Value = "Lost connection to user 1..."},
                    new Message {ImageUrl = "", Value = "User 2 waved at Kinect..."},
                    new Message {ImageUrl = "", Value = "User 1 found..."},
                    new Message {ImageUrl = "", Value = "User 1 is in calibration pose..."},
                    new Message {ImageUrl = "", Value = "Calibrating user 1..."},
                    new Message {ImageUrl = "", Value = "User 1 waved at Kinect..."}
                };

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
                App.TraceListener.CollectionChanged += TraceListenerCollectionChanged;
            }
            MarginData = "X:10,Y:20,Z;30";
        }

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

        public Visibility DebugInformation
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

        public string MarginData
        {
            get { return _marginData; }
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

        private void TraceListenerCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Message item in e.NewItems)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() => { Messages.Add(item); });
                }
                RaisePropertyChanged("LastMessage");
            }
        }

        private void SetUpKinect()
        {
            _kinect = MyKinect.Instance;
            _kinect.CameraDataUpdated += KinectCameraDataUpdated;
            _kinect.PropertyChanged += KinectPropertyChanged;
            _kinect.UserCreated += KinectUserCreated;
            _kinect.UserRemoved += KinectUserRemoved;
            _kinect.StartKinect();
        }

        private void KinectUserRemoved(object sender, KinectUserEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                                      {
                                                          lock (SyncRoot)
                                                          {
                                                              if (e.User != null)
                                                              {
                                                                  UserViewModel user =
                                                                      Users.SingleOrDefault(
                                                                          ku => ku != null && ku.Id == e.User.Id);
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

        private void KinectUserCreated(object sender, KinectUserEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                                      {
                                                          lock (SyncRoot)
                                                          {
                                                              User kuser = _kinect.GetUser(e.User.Id);
                                                              if (kuser != null)
                                                              {
                                                                  UserViewModel user = CreateUser(kuser);
                                                                  user.AddSemaphoreTracking();

                                                                  Users.Add(user);
                                                                  //User = user;
                                                                  ImageSource = null;
                                                              }
                                                          }
                                                      });
        }

        private void KinectPropertyChanged(object sender, PropertyChangedEventArgs e)
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
                _kinect.CameraDataUpdated -= KinectCameraDataUpdated;
                _kinect.StopKinect();
                _kinect = null;
                Messages.Clear();
                Users.Clear();
                //User = null;
                CameraView = null;
            }
        }

        private void KinectCameraDataUpdated(object sender, KinectCameraEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (_kinect != null)
                {
                    CameraView = e.Image;
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

        private void ToggleDebugInformation()
        {
            DebugInformation = DebugInformation == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
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
                                                              Log.DebugFormat("Key pressed: {0}", e.Key);
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
                                                              else if (e.Key == Key.Z)
                                                              {
                                                                  ResizeCameraImage();
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

            SourceUpdated = new RelayCommand<DataTransferEventArgs>(e =>
                                                                        {
                                                                            if (e.TargetObject is ListView)
                                                                            {
                                                                                var lv = e.TargetObject as ListView;
                                                                                lv.ScrollIntoView(
                                                                                    lv.Items[lv.Items.Count - 1]);
                                                                            }
                                                                        });
        }

        private void CreateUsers()
        {
            //Users = new ObservableCollection<UserViewModel>();

            var generator = new ColorGenerator();
            var rand = new Random((int) DateTime.Now.Ticks);
            //User = CreateUser(generator, rand, 1);
            for (int i = 1; i < 6; i++)
            {
                Users.Add(CreateUser(generator, rand, i));
            }
        }

        private UserViewModel CreateUser(User user)
        {
            return CreateUser(_generator, user);
        }

        private static UserViewModel CreateUser(ColorGenerator generator, User user)
        {
            float r = 1f/255*user.Color.R;
            float g = 1f/255*user.Color.G;
            float b = 1f/255*user.Color.B;

            Point3D point = GetPoint3DCoordinates(user.Id, user.Spine.X, user.Spine.Y, user.Spine.Z);

            return new UserViewModel(user)
                       {
                           Color = Color.FromArgb((byte) 0.85f, (byte) r, (byte) g, (byte) b),
                           Brush =
                               new RadialGradientBrush(Color.FromScRgb(0.85f, r, g, b),
                                                       Color.FromScRgb(0.85f, r/2, g/2, b/2)),
                           Spine = point
                       };
        }

        private static UserViewModel CreateUser(ColorGenerator generator, Random rand, int i)
        {
            string color = generator.NextColorString();
            float r = 1f/255*int.Parse(color.Substring(0, 2), NumberStyles.HexNumber);
            float g = 1f/255*int.Parse(color.Substring(2, 2), NumberStyles.HexNumber);
            float b = 1f/255*int.Parse(color.Substring(4, 2), NumberStyles.HexNumber);

            Point3D point = GetPoint3DCoordinates(i, rand.NextDouble(), rand.NextDouble(), rand.NextDouble());

            return new UserViewModel(i)
                       {
                           Brush =
                               new RadialGradientBrush(Color.FromScRgb(0.85f, r, g, b),
                                                       Color.FromScRgb(0.85f, r/2, g/2, b/2)),
                           Head = new Point3D(point.X, point.Y - 100, point.Z),
                           ShoulderCenter = new Point3D(point.X, point.Y - 60, point.Z),
                           ShoulderLeft = new Point3D(point.X - 35, point.Y - 60, point.Z),
                           ShoulderRight = new Point3D(point.X + 35, point.Y - 60, point.Z),
                           ElbowLeft = new Point3D(point.X - 55, point.Y, point.Z),
                           ElbowRight = new Point3D(point.X + 55, point.Y, point.Z),
                           HandLeft = new Point3D(point.X - 50, point.Y + 65, point.Z),
                           HandRight = new Point3D(point.X + 50, point.Y + 65, point.Z),
                           Spine = point,
                           HipLeft = new Point3D(point.X - 30, point.Y + 50, point.Z),
                           HipRight = new Point3D(point.X + 30, point.Y + 50, point.Z),
                           KneeLeft = new Point3D(point.X - 35, point.Y + 130, point.Z),
                           KneeRight = new Point3D(point.X + 35, point.Y + 130, point.Z),
                           FootLeft = new Point3D(point.X - 40, point.Y + 210, point.Z),
                           FootRight = new Point3D(point.X + 40, point.Y + 210, point.Z),
                       };
        }

        private static Point3D GetPoint3DCoordinates(int i, double x, double y, double z)
        {
            var point = new Point3D
                            {
                                X = (i*(x + 25))/x, 
                                Y = (i*(y + 25))/y, 
                                Z = i*z
                            };
            return point;
        }

        #region Nested type: CameraImageSize

        private enum CameraImageSize
        {
            Large,
            Small
        }

        #endregion
    }
}