using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Media.Imaging;
using Kinect.Common;
using log4net;
using xn;
using ms = System.Windows.Media.Media3D;

namespace Kinect.Core
{
    /// <summary>
    /// Kinect states
    /// </summary>
    [Flags]
    public enum KinectState
    {
        /// <summary>
        /// Kinect context is open
        /// </summary>
        ContextOpen,

        /// <summary>
        /// Kinect is initializing
        /// </summary>
        Initializing,

        /// <summary>
        /// Kinect has failed
        /// </summary>
        Failed,

        /// <summary>
        /// Kinect is stopped
        /// </summary>
        Stopped,

        /// <summary>
        /// Kinect is running
        /// </summary>
        Running
    }

    /// <summary>
    /// Get all data from Kinect and control Kinect.
    /// </summary>
    public sealed class MyKinect : INotifyPropertyChanged
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(MyKinect));

        private static object _syncRoot = new object();
        private static readonly MyKinect _instance = new MyKinect();

        private MyKinect()
        {
            SingleUserMode = false;
        }

        private static bool _running = false;
        private static Thread _cameraThread;

        private Context _context;
        private UserGenerator _userGenerator;
        private SkeletonCapability _skeletonCapbility;
        private PoseDetectionCapability _poseDetectionCapability;
        private string _calibrationPose;
        private KinectState _kinectstate;

        private List<User> _activeUsers;
        private int _singleUserDataSlot = -1;
        private Camera _camera = new Camera();

        public static MyKinect Instance
        {
            get { return _instance; }
        }

        public KinectState KinectState
        {
            get
            {
                return _kinectstate;
            }
            private set
            {
                if (_kinectstate != value)
                {
                    _kinectstate = value;
                    OnPropertyChanged("KinectState");
                }
            }
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

        public event EventHandler<KinectMessageEventArgs> CameraMessage;

        public event EventHandler<KinectEventArgs> KinectStarted;

        public event EventHandler<KinectEventArgs> KinectStopped;

        public event EventHandler<KinectEventArgs> KinectCrashed;

        public event EventHandler<KinectEventArgs> CameraDataUpdated;

        public event EventHandler<KinectEventArgs> CameraStarted;

        public event EventHandler<KinectEventArgs> CameraStopped;

        public event EventHandler<KinectUserEventArgs> UserCreated;

        public event EventHandler<KinectUserEventArgs> UserRemoved;

        public event EventHandler<KinectUserEventArgs> UserCalibrating;

        public event EventHandler<KinectUserEventArgs> UserCalibrated;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<KinectEventArgs> NewUser;

        public event EventHandler<KinectEventArgs> CalibrationStarted;

        public event EventHandler<KinectEventArgs> CalibrationFailed;

        public void OnUserCalibrating(IUserChangedEvent user)
        {
            var handler = this.UserCalibrating;
            if (handler != null)
            {
                handler(this, new KinectUserEventArgs(user));
            }
        }

        public void OnUserCalibrated(IUserChangedEvent user)
        {
            var handler = this.UserCalibrated;
            if (handler != null)
            {
                handler(this, new KinectUserEventArgs(user));
            }
        }

        public void StartKinect()
        {
            lock (_syncRoot)
            {
                try
                {
                    if (!_running)
                    {
                        if ((KinectState.Stopped | KinectState.Failed).Has(KinectState))
                        {
                            _running = true;
                            try
                            {
                                _context = new xn.Context(@".\Configs\openniconfig.xml");
                                _camera.Context = _context;
                                _camera.PropertyChanged += new PropertyChangedEventHandler(_camera_PropertyChanged);
                                _camera.Running = true;

                                KinectState = KinectState.ContextOpen;
                            }
                            catch (Exception ex)
                            {
                                _log.IfError("Context creation failed", ex);

                                KinectState = KinectState.Failed;
                                _running = false;
                                return;
                            }

                            _log.IfInfo("Kinect device found.");

                            OnCameraMessage("Kinect device found.");

                            KinectState = KinectState.Initializing;

                            _userGenerator = new UserGenerator(this._context);
                            _skeletonCapbility = new SkeletonCapability(this._userGenerator);
                            _poseDetectionCapability = new PoseDetectionCapability(this._userGenerator);
                            _calibrationPose = this._skeletonCapbility.GetCalibrationPose();

                            _camera.Initialize(_userGenerator);

                            SubscribeToEvents();

                            _skeletonCapbility.SetSkeletonProfile(SkeletonProfile.All);
                            _activeUsers = new List<User>();
                            _userGenerator.StartGenerating();

                            _log.IfInfo("Kinect device configured.");

                            OnCameraMessage("Kinect device configured.");

                            _log.IfInfo("Start Kinect Thread.");

                            _cameraThread = new Thread(new ThreadStart(Execute));
                            _cameraThread.Start();
                            OnKinectStarted();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.IfError("Kinect failed.", ex);

                    OnCameraMessage(ex.Message);
                    KinectState = KinectState.Failed;

                    _running = false;
                    _camera.Running = false;
                }
            }
        }

        private void SubscribeToEvents()
        {
            _userGenerator.NewUser += UserGenerator_NewUser;
            _userGenerator.LostUser += UserGenerator_LostUser;
            _poseDetectionCapability.PoseDetected += PoseDetectionCapability_PoseDetected;
            _skeletonCapbility.CalibrationStart += SkeletonCapbility_CalibrationStart;
            _skeletonCapbility.CalibrationEnd += SkeletonCapbility_CalibrationEnd;
        }

        private void OnCameraMessage(string message)
        {
            var handler = this.CameraMessage;
            if (handler != null)
            {
                handler(this, new KinectMessageEventArgs { Message = message });
            }
        }

        private void OnCameraMessage(string format, params object[] args)
        {
            OnCameraMessage(string.Format(format, args));
        }

        private void OnKinectStarted()
        {
            var handler = this.KinectStarted;
            if (handler != null)
            {
                handler(this, new KinectEventArgs());
            }
        }

        private void OnKinectStopped()
        {
            var handler = this.KinectStopped;
            if (handler != null)
            {
                handler(this, new KinectEventArgs());
            }
        }

        private void OnKinectCrashed()
        {
            var handler = this.KinectCrashed;
            if (handler != null)
            {
                handler(this, new KinectEventArgs());
            }
        }

        private void OnCameraDataUpdated()
        {
            var handler = this.CameraDataUpdated;
            if (handler != null)
            {
                handler(this, new KinectEventArgs());
            }
        }

        private void OnCameraStarted()
        {
            var handler = this.CameraStarted;
            if (handler != null)
            {
                handler(this, new KinectEventArgs());
            }
        }

        private void OnCameraStopped()
        {
            var handler = this.CameraStopped;
            if (handler != null)
            {
                handler(this, new KinectEventArgs());
            }
        }

        private void OnUserCreated(IUserChangedEvent userEvent)
        {
            var handler = this.UserCreated;
            if (handler != null)
            {
                handler(this, new KinectUserEventArgs(userEvent));
            }
        }

        private void OnUserRemoved(IUserChangedEvent userEvent)
        {
            var handler = this.UserRemoved;
            if (handler != null)
            {
                handler(this, new KinectUserEventArgs(userEvent));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnPropertyChanged(Expression<Func<object>> propertyExpression)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler.Raise(propertyExpression);
            }
        }

        private void OnNewUser()
        {
            var handler = this.NewUser;
            if (handler != null)
            {
                handler(this, new KinectEventArgs());
            }
        }

        private void OnCalibrationStarted()
        {
            var handler = this.CalibrationStarted;
            if (handler != null)
            {
                handler(this, new KinectEventArgs());
            }
        }

        private void OnCalibrationFailed()
        {
            var handler = this.CalibrationFailed;
            if (handler != null)
            {
                handler(this, new KinectEventArgs());
            }
        }

        private void UserGenerator_NewUser(ProductionNode node, uint id)
        {
            this.OnCameraMessage("New User {0}", id);
            _log.IfInfoFormat("New user {0}", id);
            if (this.SingleUserMode && this._singleUserDataSlot != -1)
            {
                //Here we know the frontend will work with OnUserCreated
                lock (_syncRoot)
                {
                    if (this._singleUserDataSlot != -1)
                    {
                        this.OnNewUser();
                    }

                    this._skeletonCapbility.LoadCalibrationData(id, (uint)_singleUserDataSlot);
                    this._skeletonCapbility.StartTracking(id);
                    var user = new User(id);
                    user.Color = _camera.GetUserColor(id);
                    _activeUsers.Add(user);
                    this.OnUserCreated(user);
                }
            }
            else
            {
                this.OnNewUser();
                _poseDetectionCapability.StartPoseDetection(_calibrationPose, id);
            }
        }

        private void UserGenerator_LostUser(ProductionNode node, uint id)
        {
            var user = (from u in _activeUsers where u.ID == id select u).FirstOrDefault();
            if (user != null)
            {
                this._activeUsers.Remove(user);
            }

            _log.IfInfoFormat("User {0} lost", id);

            this.OnCameraMessage("User {0} lost", id);
            this.OnUserRemoved(user);
        }

        private void PoseDetectionCapability_PoseDetected(ProductionNode node, string pose, uint id)
        {
            this._poseDetectionCapability.StopPoseDetection(id);
            this._skeletonCapbility.RequestCalibration(id, true);

            _log.IfInfoFormat("User {0} pose detected: {1}", id, pose);

            this.OnCameraMessage("User {0} pose detected: {1}", id, pose);
        }

        private void SkeletonCapbility_CalibrationStart(ProductionNode node, uint id)
        {
            this.OnCalibrationStarted();
            this.OnUserCalibrating(new User(id));

            _log.IfInfoFormat("Start calibration for user: {0}", id);

            this.OnCameraMessage("Start calibration for user: {0}", id);
        }

        private void SkeletonCapbility_CalibrationEnd(ProductionNode node, uint id, bool success)
        {
            if (success)
            {
                lock (_syncRoot)
                {
                    this.OnUserCalibrated(new User(id));
                    _log.IfInfoFormat("User {0} successfull calibrated.", id);

                    this._skeletonCapbility.StartTracking(id);
                    if (SingleUserMode)
                    {
                        this._singleUserDataSlot = (int)id;
                        this._skeletonCapbility.SaveCalibrationData(id, (uint)_singleUserDataSlot);
                    }

                    var user = new User(id);
                    user.Color = _camera.GetUserColor(id);
                    this._activeUsers.Add(user);
                    this.OnUserCreated(user);
                    if (_log.IsInfoEnabled)
                    {
                        this.OnCameraMessage("Skeleton {0} calibrated {1}", id, success);
                    }
                }
            }
            else
            {
                _log.IfInfoFormat("User not calibrated {0}., restart calibration", id);
                OnCameraMessage("Calibration failed for user: {0}", id);
                this._poseDetectionCapability.StartPoseDetection(_calibrationPose, id);
                this.OnCalibrationFailed();
            }
        }

        public void StopKinect()
        {
            lock (_syncRoot)
            {

                _log.IfInfo("Stopping Kinect");

                _running = false;
                _singleUserDataSlot = -1;
                if (this._camera != null)
                {
                    this._camera.Running = false;
                }
                this.KinectState = KinectState.Stopped;
            }
            this.OnKinectStopped();
        }

        public User GetUser(uint userId)
        {
            return this._activeUsers.FirstOrDefault(u => u.ID == userId);
        }

        public BitmapSource GetCameraView(CameraView view)
        {
            return this._camera.GetView(view);
        }

        public bool IsJointAvailable(SkeletonJoint skeletonJoint)
        {
            return this._skeletonCapbility.IsJointAvailable(skeletonJoint);
        }

        public ms.Point3D GetSkeletonPoint(uint userId, SkeletonJoint skeletonJoint)
        {
            if (!this.IsJointAvailable(skeletonJoint))
            {
                return new ms.Point3D(-999, -999, -999);
            }

            var pos = new SkeletonJointPosition();
            _skeletonCapbility.GetSkeletonJointPosition(userId, skeletonJoint, ref pos);
            if (pos.position.Z == 0)
            {
                pos.fConfidence = 0;
            }
            else
            {
                pos.position = _camera.Depth.ConvertRealWorldToProjective(pos.position);
            }

            return pos.position.GetMediaPoint3D();
        }

        private void Execute()
        {
            this.OnCameraMessage("Kinect running.");

            _log.IfInfo("Kinect running.");

            this.KinectState = KinectState.Running;
            bool firstUpdate = false;
            while (_running)
            {
                try
                {
                    this._context.WaitAndUpdateAll();

                    if (!firstUpdate)
                    {
                        OnCameraStarted();
                        firstUpdate = true;
                    }

                    this._activeUsers.AsParallel().ForAll(user =>
                    {
                        user.Head = GetSkeletonPoint(user.ID, SkeletonJoint.Head);
                        user.Neck = GetSkeletonPoint(user.ID, SkeletonJoint.Neck);
                        user.Torso = GetSkeletonPoint(user.ID, SkeletonJoint.Torso);
                        user.LeftShoulder = GetSkeletonPoint(user.ID, SkeletonJoint.LeftShoulder);
                        user.RightShoulder = GetSkeletonPoint(user.ID, SkeletonJoint.RightShoulder);
                        user.LeftElbow = GetSkeletonPoint(user.ID, SkeletonJoint.LeftElbow);
                        user.RightElbow = GetSkeletonPoint(user.ID, SkeletonJoint.RightElbow);
                        user.LeftHand = GetSkeletonPoint(user.ID, SkeletonJoint.LeftHand);
                        user.RightHand = GetSkeletonPoint(user.ID, SkeletonJoint.RightHand);
                        user.LeftFingertip = GetSkeletonPoint(user.ID, SkeletonJoint.LeftFingertip);
                        user.RightFingertip = GetSkeletonPoint(user.ID, SkeletonJoint.RightFingertip);
                        user.LeftHip = GetSkeletonPoint(user.ID, SkeletonJoint.LeftHip);
                        user.RightHip = GetSkeletonPoint(user.ID, SkeletonJoint.RightHip);
                        user.LeftKnee = GetSkeletonPoint(user.ID, SkeletonJoint.LeftKnee);
                        user.RightKnee = GetSkeletonPoint(user.ID, SkeletonJoint.RightKnee);
                        user.LeftAnkle = GetSkeletonPoint(user.ID, SkeletonJoint.LeftAnkle);
                        user.RightAnkle = GetSkeletonPoint(user.ID, SkeletonJoint.RightAnkle);
                        user.LeftFoot = GetSkeletonPoint(user.ID, SkeletonJoint.LeftFoot);
                        user.RightFoot = GetSkeletonPoint(user.ID, SkeletonJoint.RightFoot);
                        user.Waist = GetSkeletonPoint(user.ID, SkeletonJoint.Waist);
                        user.Update();
                    });

                    this._camera.CalculateFPS();

                    OnCameraDataUpdated();
                }
                catch (Exception ex)
                {
                    _log.IfFatalFormat("", ex);

                    this.OnKinectCrashed();
                    this.StopKinect();
                }
            }
            _log.IfInfo("Kinect stopping.");

            this._context.Shutdown();
            this._context.Dispose();
            OnCameraStopped();

            _log.IfInfo("Kinect stopped.");

            this.StopKinect();
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
