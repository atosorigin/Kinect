using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Kinect.Common;
using log4net;
using Microsoft.Research.Kinect.Nui;
using System.Threading;

#pragma warning disable 1591

namespace Kinect.Core
{
    /// <summary>
    /// Get all data from Kinect and control Kinect.
    /// </summary>
    public sealed class MyKinect : INotifyPropertyChanged
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (MyKinect));

        private static readonly object SyncRoot = new object();
        private static readonly MyKinect _instance = new MyKinect();

        private static bool _running;
        private static Thread _kinectThread;
        private readonly Camera _camera = new Camera();
        private readonly Runtime _context = new Runtime();
        private List<User> _activeUsers = new List<User>(2);
        private KinectState _kinectstate = KinectState.Stopped;
        private int _nrOfUsers;

        private MyKinect()
        {
            SingleUserMode = false;
        }

        ///<summary>
        ///</summary>
        public static MyKinect Instance
        {
            get { return _instance; }
        }

        ///<summary>
        ///</summary>
        public KinectState KinectState
        {
            get { return _kinectstate; }
            private set
            {
                if (_kinectstate != value)
                {
                    _kinectstate = value;
                    OnPropertyChanged("KinectState");
                }
            }
        }

        public CameraView CameraViewType
        {
            get { return _camera.ViewType; }
            set { _camera.ViewType = value; }
        }

        public ReadOnlyCollection<User> ActiveUsers
        {
            get { return _activeUsers.AsReadOnly(); }
        }

        public int FPS
        {
            get { return _camera.Fps; }
        }

        public bool SingleUserMode { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public event EventHandler<KinectMessageEventArgs> CameraMessage;

        public event EventHandler<KinectEventArgs> KinectStarted;

        public event EventHandler<KinectEventArgs> KinectStopped;

        public event EventHandler<KinectCameraEventArgs> CameraDataUpdated
        {
            add { _camera.CameraUpdated += value; }
            remove { _camera.CameraUpdated -= value; }
        }

        public event EventHandler<KinectUserEventArgs> UserCreated;

        public event EventHandler<KinectUserEventArgs> UserRemoved;

        public void StartKinect()
        {
            if (_running ||
                (KinectState.ContextOpen | KinectState.Running | KinectState.Initializing).Has(KinectState))
            {
                //Kinect is already running
                return;
            }
            var start = new ThreadStart(StartKinectThread);
            _kinectThread = new Thread(start);
            _kinectThread.Start();
        }

        private void StartKinectThread()
        {
            lock (SyncRoot)
            {
                if (_running ||
                    (KinectState.ContextOpen | KinectState.Running | KinectState.Initializing).Has(KinectState))
                {
                    //Kinect is already running
                    return;
                }

                try
                {
                    _running = true;
                    try
                    {
                        _nrOfUsers = 0;
                        _context.Initialize(RuntimeOptions.UseDepthAndPlayerIndex |
                                            RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);
                        _context.SkeletonEngine.TransformSmooth = true;
                        _camera.Context = _context;
                        _camera.PropertyChanged += _camera_PropertyChanged;
                        _camera.Running = true;

                        KinectState = KinectState.ContextOpen;
                    }
                    catch (Exception ex)
                    {
                        Log.IfError(
                            "Runtime initialization failed. Please make sure Kinect device is plugged in. Error: {0}",
                            ex);

                        KinectState = KinectState.Failed;
                        _running = false;
                        return;
                    }

                    try
                    {
                        _context.NuiCamera.ElevationAngle = 10;
                        _context.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480,
                                                  ImageType.Color);
                        _context.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240,
                                                  ImageType.DepthAndPlayerIndex);
                    }
                    catch (Exception ex)
                    {
                        Log.IfError(
                            "Failed to open stream. Please make sure to specify a supported image type and resolution. Error: {0}",
                            ex);
                        KinectState = KinectState.Failed;
                        _running = false;
                        return;
                    }

                    Log.IfInfo("Kinect device found.");

                    OnCameraMessage("Kinect device found.");

                    KinectState = KinectState.Initializing;

                    _camera.Initialize();

                    _context.SkeletonFrameReady += context_SkeletonFrameReady;
                    _context.SkeletonEngine.IsEnabled = true;

                    _activeUsers = new List<User>();

                    Log.IfInfo("Kinect device configured.");

                    OnCameraMessage("Kinect device configured.");

                    Log.IfInfo("Kinect started");

                    KinectState = KinectState.Running;

                    OnKinectStarted();
                }
                catch (Exception ex)
                {
                    Log.IfError("Kinect failed.", ex);

                    OnCameraMessage(ex.Message);
                    KinectState = KinectState.Failed;

                    _running = false;
                    _camera.Running = false;
                }
            }

            while (_running && _context != null)
            {
                Thread.Sleep(10);
            }

            if (_context != null)
            {
                _context.Uninitialize();
            }
        }

        private void context_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            lock (SyncRoot)
            {
                foreach (SkeletonData skeleton in e.SkeletonFrame.Skeletons)
                {
                    User user = GetUser(skeleton.UserIndex);
                    if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        //First check if it is a new user
                        if (user == null)
                        {
                            user = new User(skeleton.UserIndex);
                            _activeUsers.Add(user);
                            OnUserCreated(user);
                        }

                        //Update user
                        user.Head = getDisplayPosition(skeleton.Joints[JointID.Head]);
                        user.Neck = getDisplayPosition(skeleton.Joints[JointID.ShoulderCenter]);
                        user.Torso = getDisplayPosition(skeleton.Joints[JointID.Spine]);
                        user.Waist = getDisplayPosition(skeleton.Joints[JointID.HipCenter]);

                        user.LeftAnkle = getDisplayPosition(skeleton.Joints[JointID.AnkleLeft]);
                        user.LeftElbow = getDisplayPosition(skeleton.Joints[JointID.ElbowLeft]);
                        user.LeftFoot = getDisplayPosition(skeleton.Joints[JointID.FootLeft]);
                        user.LeftHand = getDisplayPosition(skeleton.Joints[JointID.HandLeft]);
                        user.LeftHip = getDisplayPosition(skeleton.Joints[JointID.HipLeft]);
                        user.LeftKnee = getDisplayPosition(skeleton.Joints[JointID.KneeLeft]);
                        user.LeftShoulder = getDisplayPosition(skeleton.Joints[JointID.ShoulderLeft]);
                        user.RightAnkle = getDisplayPosition(skeleton.Joints[JointID.AnkleRight]);
                        user.RightElbow = getDisplayPosition(skeleton.Joints[JointID.ElbowRight]);
                        user.RightFoot = getDisplayPosition(skeleton.Joints[JointID.FootRight]);
                        user.RightHand = getDisplayPosition(skeleton.Joints[JointID.HandRight]);
                        user.RightHip = getDisplayPosition(skeleton.Joints[JointID.HipRight]);
                        user.RightKnee = getDisplayPosition(skeleton.Joints[JointID.KneeRight]);
                        user.RightShoulder = getDisplayPosition(skeleton.Joints[JointID.ShoulderRight]);
                        user.Update();
                    }
                    else if (user != null && skeleton.TrackingState == SkeletonTrackingState.NotTracked)
                    {
                        //User isn't tracking any more, remove the user
                        _activeUsers.Remove(user);
                        OnUserRemoved(user);
                    }
                }
            }
        }

        private Point3D getDisplayPosition(Joint joint)
        {
            //TODO: Implementeer logica voor de trackingstate. Zodat we ook weten of hij ook echt alles ziet
            //if (joint.TrackingState != JointTrackingState.Tracked)
            //{
            //    //Als hij hem niet ziet, laat hem dan ook links boven in beeld zien.
            //    return new Point3D(0, 0, 0);
            //}

            float depthX, depthY;
            short depthZ;
            _context.SkeletonEngine.SkeletonToDepthImage(joint.Position, out depthX, out depthY, out depthZ);
            depthX = Math.Max(0, Math.Min(depthX * 320, 320)); //convert to 640, 480 space
            depthY = Math.Max(0, Math.Min(depthY * 240, 240)); //convert to 640, 480 space
            //Make it milimeters
            return new Point3D(depthX, depthY, depthZ / 10);
        }

        private void OnCameraMessage(string message)
        {
            EventHandler<KinectMessageEventArgs> handler = CameraMessage;
            if (handler != null)
            {
                handler(this, new KinectMessageEventArgs {Message = message});
            }
        }

        private void OnKinectStarted()
        {
            EventHandler<KinectEventArgs> handler = KinectStarted;
            if (handler != null)
            {
                handler(this, new KinectEventArgs());
            }
        }

        private void OnKinectStopped()
        {
            EventHandler<KinectEventArgs> handler = KinectStopped;
            if (handler != null)
            {
                handler(this, new KinectEventArgs());
            }
        }

        private void OnUserCreated(IUserChangedEvent userEvent)
        {
            EventHandler<KinectUserEventArgs> handler = UserCreated;
            if (handler != null)
            {
                handler(this, new KinectUserEventArgs(userEvent));
            }
        }

        private void OnUserRemoved(IUserChangedEvent userEvent)
        {
            EventHandler<KinectUserEventArgs> handler = UserRemoved;
            if (handler != null)
            {
                handler(this, new KinectUserEventArgs(userEvent));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void StopKinect()
        {
            lock (SyncRoot)
            {
                Log.IfInfo("Stopping Kinect");
                //_context.Uninitialize();
                if (_camera != null)
                {
                    _camera.Running = false;
                }
                _running = false;
                KinectState = KinectState.Stopped;
            }
            OnKinectStopped();
        }

        public User GetUser(int userId)
        {
            return _activeUsers.FirstOrDefault(u => u.ID == userId);
        }

        public BitmapSource GetCameraView(CameraView view)
        {
            //return this._camera.GetView(view);
            return null;
        }

        public void MotorUp(int angle)
        {
            try
            {
                int newAngle = _context.NuiCamera.ElevationAngle + angle;
                if (newAngle > Microsoft.Research.Kinect.Nui.Camera.ElevationMaximum)
                {
                    newAngle = Microsoft.Research.Kinect.Nui.Camera.ElevationMaximum;
                }
                _context.NuiCamera.ElevationAngle = newAngle;
            }
            catch (InvalidOperationException ex)
            {
                OnCameraMessage(string.Concat("Couldn't change the angle of the motor up: ", ex.Message));
            }
        }

        public void MotorDown(int angle)
        {
            try
            {
                int newAngle = _context.NuiCamera.ElevationAngle - angle;
                if (newAngle < Microsoft.Research.Kinect.Nui.Camera.ElevationMinimum)
                {
                    newAngle = Microsoft.Research.Kinect.Nui.Camera.ElevationMinimum;
                }
                _context.NuiCamera.ElevationAngle = angle;
            }
            catch (InvalidOperationException ex)
            {
                OnCameraMessage(string.Concat("Couldn't change the angle of the motor down: ", ex.Message));
            }
        }

        private void _camera_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FPS")
            {
                OnPropertyChanged("FPS");
            }
        }
    }
}

#pragma warning restore 1591