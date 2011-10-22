using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Media3D;
using GalaSoft.MvvmLight.Threading;
using Kinect.Common;
using Kinect.Core;
using Kinect.Core.Eventing;
using Kinect.Core.Filters.Helper;
using Kinect.Core.Gestures;
using Kinect.Plugins.Common.ViewModels;
using Kinect.Plugins.Common.Views;
using log4net;

namespace Kinect.Plugins.Common
{
    public class KinectManager
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (KinectManager));

        private static readonly KinectManager _instance = new KinectManager();

        private static readonly object _syncRoot = new object();
        //private User _kinectUser;

        private readonly Calibration _calibrationView;
        private readonly CalibrationViewModel _calibrationViewModel;
        private readonly MyKinect _kinect;
        private User _calibratingUser;
        private int _calibration;
        private List<User> _kinectUsers;
        private DateTime _lastHit = DateTime.Now;
        private SelfTouchGesture _nextSlideSelfTouch, _previousSlideSelfTouch;
        private bool _saveCalibrationData;
        private SelfTouchGesture _togglePointerSelfTouch;
        private bool _userCalibrated;

        private KinectManager()
        {
            EventIntervalInMilliseconds = 2000;
            _kinect = MyKinect.Instance;
            _kinect.SingleUserMode = true;
            _kinect.ChangeMaxSkeletonPositions(.5f, .5f);
            ConfigurationViewModel = new ConfigureKinectViewModel();
            _calibrationView = new Calibration();
            _calibrationViewModel = CalibrationViewModel.Current;

            if (_calibrationViewModel != null)
            {
                _calibrationViewModel.SaveCalibrationData += CalibrationView_SaveCalibrationData;
                _calibrationViewModel.CountDownFinished += CalibrationView_CountDownFinished;
            }
        }

        public static KinectManager Instance
        {
            get { return _instance; }
        }

        internal MyKinect Kinect
        {
            get { return _kinect != null && _kinect.KinectState == KinectState.Running ? _kinect : null; }
        }

        public ConfigureKinectViewModel ConfigurationViewModel { get; private set; }

        public int EventIntervalInMilliseconds { get; set; }
        public bool Running { get; private set; }

        public void StartKinect()
        {
            _kinectUsers = new List<User>();
            _kinect.UserCreated += _kinect_UserCreated;
            _kinect.UserRemoved += _kinect_UserRemoved;
            _kinect.KinectStarted += _kinect_Started;
            _kinect.KinectStopped += _kinect_Stopped;
            _kinect.StartKinect();
        }

        public void StopKinect()
        {
            for (int i = 0; i < _kinectUsers.Count; i++)
            {
                _kinectUsers[i] = null;
            }
            _kinectUsers.Clear();
            _calibratingUser = null;

            _kinect.UserCreated -= _kinect_UserCreated;
            _kinect.UserRemoved -= _kinect_UserRemoved;
            _kinect.KinectStarted -= _kinect_Started;
            _kinect.StopKinect();
        }

        public void OpenConfiguration()
        {
            var configuration = new ConfigureKinect();
            configuration.DataContext = ConfigurationViewModel;
            configuration.Show();
        }

        public void StartCalibration(User user)
        {
            if (user != null)
            {
                _log.DebugFormat("Start calibration for user {0}", user.Id);
                _calibration = 0;
                _calibratingUser = user;
                OnUserCalibrating();
                RunCalibration(_calibratingUser);
            }
        }

        internal event EventHandler<SinglePointEventArgs> LaserUpdated;

        private void OnLaserUpdated(int userid, Point3D point)
        {
            //TODO: Hier was ik gebleven. Toggle laser aan user binden
            EventHandler<SinglePointEventArgs> handler = LaserUpdated;
            if (handler != null)
            {
                handler(this, new SinglePointEventArgs(userid, point));
            }
        }

        internal event EventHandler<UserEventArgs> NextSlide;

        private void OnNextSlide(int userid)
        {
            EventHandler<UserEventArgs> handler = NextSlide;
            if (handler != null)
            {
                handler.Invoke(this, new UserEventArgs(userid));
            }
        }

        internal event EventHandler<UserEventArgs> PreviousSlide;

        private void OnPreviousSlide(int userid)
        {
            EventHandler<UserEventArgs> handler = PreviousSlide;
            if (handler != null)
            {
                handler.Invoke(this, new UserEventArgs(userid));
            }
        }

        internal event EventHandler<UserEventArgs> TogglePointer;

        private void OnTogglePointer(int userID)
        {
            EventHandler<UserEventArgs> handler = TogglePointer;
            if (handler != null)
            {
                handler.Invoke(this, new UserEventArgs(userID));
            }
        }

        public event EventHandler UserCalibrating;

        private void OnUserCalibrating()
        {
            EventHandler handler = UserCalibrating;
            if (handler != null)
            {
                handler.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler UserCalibrated;

        private void OnUserCalibrated()
        {
            EventHandler handler = UserCalibrated;
            if (handler != null)
            {
                handler.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler<UserEventArgs> UserFound;

        private void OnUserFound(int id)
        {
            EventHandler<UserEventArgs> handler = UserFound;
            if (handler != null)
            {
                handler.Invoke(this, new UserEventArgs(id));
            }
        }

        public event EventHandler<UserEventArgs> UserLost;

        private void OnUserLost(int id)
        {
            EventHandler<UserEventArgs> handler = UserLost;
            if (handler != null)
            {
                handler.Invoke(this, new UserEventArgs(id));
            }
        }

        public event EventHandler KinectStarted;

        private void OnKinectStarted()
        {
            EventHandler handler = KinectStarted;
            if (handler != null)
            {
                handler.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler KinectStopped;

        private void OnKinectStopped()
        {
            EventHandler handler = KinectStopped;
            if (handler != null)
            {
                handler.Invoke(this, new EventArgs());
            }
        }

        private void _kinect_Started(object sender, KinectEventArgs e)
        {
            Running = true;
            OnKinectStarted();
            Thread.Sleep(100);
            ShowCalibrationMessage("Please initialise user");
        }

        private void _kinect_Stopped(object sender, KinectEventArgs e)
        {
            CleanUpAfterKinectStopped();
        }

        private void CleanUpAfterKinectStopped()
        {
            Running = false;
            _userCalibrated = false;
            OnKinectStopped();
        }

        private void _kinect_UserRemoved(object sender, KinectUserEventArgs e)
        {
            if (_kinectUsers != null)
            {
                User user =
                    (from kinectuser in _kinectUsers where kinectuser.Id == e.User.Id select kinectuser).FirstOrDefault();
                if (user != null)
                {
                    _kinectUsers.Remove(user);
                    _log.DebugFormat("User {0} removed from the list of active Powerpoint users", user.Id);
                }
                //_kinectUser = null;
            }
            OnUserLost(e.User.Id);
        }

        private void _kinect_UserCreated(object sender, KinectUserEventArgs e)
        {
            if (_kinectUsers.Count > 0 && !_userCalibrated)
            {
                //De gebruiker is al aan het initialiseren
                _log.DebugFormat("User {0} ignored, because another user is calibrating", e.User.Id);
                return;
            }

            User user = _kinect.GetUser(e.User.Id);
            user.Updated += _kinectUser_Updated;
            _kinectUsers.Add(user);
            _log.DebugFormat("User {0} added to the list of active Powerpoint users", user.Id);
            OnUserFound(e.User.Id);

            if (_userCalibrated)
            {
                AddUserTouchEvents(user);
            }
            else
            {
                StartCalibration(user);
            }
        }

        private void AddUserTouchEvents(User user)
        {
            //Need to lock. If the user is lost, you don't want that the user is getting set to null during adding of events
            lock (_syncRoot)
            {
                if (ConfigurationViewModel.EnableNextSlide)
                {
                    _nextSlideSelfTouch = user.AddSelfTouchGesture(ConfigurationViewModel.NextSlideCorrection,
                                                                   ConfigurationViewModel.NextSlide1,
                                                                   ConfigurationViewModel.NextSlide2);
                    _nextSlideSelfTouch.SelfTouchDetected +=
                        ((s, evt) =>
                             {
                                 if (CheckEventInterval())
                                 {
                                     _log.DebugFormat("Next Slide \tUser:{0}", evt.UserID);
                                     OnNextSlide(evt.UserID);
                                 }
                             });
                    _log.DebugFormat("Added NextSlideEvent\tUser:{0}\tCorrection:{1}\t({2} on {3})", user.Id,
                                     ConfigurationViewModel.NextSlideCorrection.GetDebugString(),
                                     ConfigurationViewModel.NextSlide1, ConfigurationViewModel.NextSlide2);
                }

                if (ConfigurationViewModel.EnablePreviousSlide)
                {
                    _previousSlideSelfTouch = user.AddSelfTouchGesture(ConfigurationViewModel.PreviousSlideCorrection,
                                                                       ConfigurationViewModel.PreviousSlide1,
                                                                       ConfigurationViewModel.PreviousSlide2);
                    _previousSlideSelfTouch.SelfTouchDetected +=
                        ((s, evt) =>
                             {
                                 if (CheckEventInterval())
                                 {
                                     _log.DebugFormat("Previous Slide \tUser:{0}", evt.UserID);
                                     OnPreviousSlide(evt.UserID);
                                 }
                             });
                    _log.DebugFormat("Added PreviousSlideEvent\tUser:{0}\tCorrection:{1}\t({2} on {3})", user.Id,
                                     ConfigurationViewModel.PreviousSlideCorrection.GetDebugString(),
                                     ConfigurationViewModel.PreviousSlide1, ConfigurationViewModel.PreviousSlide2);
                }

                if (ConfigurationViewModel.EnableTogglePointer)
                {
                    _togglePointerSelfTouch = user.AddSelfTouchGesture(ConfigurationViewModel.TogglePointerCorrection,
                                                                       ConfigurationViewModel.TogglePointer1,
                                                                       ConfigurationViewModel.TogglePointer2);
                    _togglePointerSelfTouch.SelfTouchDetected +=
                        ((s, evt) =>
                             {
                                 if (CheckEventInterval())
                                 {
                                     _log.DebugFormat("TogglePointer \tUser:{0}", evt.UserID);
                                     OnTogglePointer(evt.UserID);
                                 }
                             });
                    _log.DebugFormat("Added TogglePointerEvent\tUser:{0}\tCorrection:{1}\t({2} on {3})", user.Id,
                                     ConfigurationViewModel.TogglePointerCorrection.GetDebugString(),
                                     ConfigurationViewModel.TogglePointer1, ConfigurationViewModel.TogglePointer2);
                }
            }
        }

        private void RemoveUserTouchEvents(User user)
        {
            if (user != null)
            {
                if (_nextSlideSelfTouch != null)
                {
                    user.RemoveGesture(_nextSlideSelfTouch);
                }
                if (_previousSlideSelfTouch != null)
                {
                    user.RemoveGesture(_previousSlideSelfTouch);
                }
                if (_togglePointerSelfTouch != null)
                {
                    user.RemoveGesture(_togglePointerSelfTouch);
                }
            }
        }

        private void RunCalibration(User user)
        {
            string message = GetCalibrationMessage();
            if (_calibration < 3)
            {
                ShowCalibrationMessage(message);
                if (_calibrationViewModel != null)
                {
                    _calibrationViewModel.StartCountDown(ConfigurationViewModel.Countdown);
                }
            }
            else
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => { _calibrationView.Hide(); });
                RemoveUserTouchEvents(user);
                AddUserTouchEvents(user);
                _userCalibrated = true;
                OnUserCalibrated();
            }
        }

        private void ShowCalibrationMessage(string message)
        {
            _calibrationViewModel.CalibrationMessage = message;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                                      {
                                                          if (_calibrationView.Visibility != Visibility.Visible)
                                                          {
                                                              _calibrationView.Show();
                                                          }
                                                      });
        }

        private string GetCalibrationMessage()
        {
            if (_calibration == 0)
            {
                if (ConfigurationViewModel.EnableNextSlide && ConfigurationViewModel.CalibrateNextSlide)
                {
                    return ConfigurationViewModel.NextSlideCalibrationMessage;
                }
                _calibration++;
            }
            if (_calibration == 1)
            {
                if (ConfigurationViewModel.EnablePreviousSlide && ConfigurationViewModel.CalibratePreviousSlide)
                {
                    return ConfigurationViewModel.PreviousSlideCalibrationMessage;
                }
                _calibration++;
            }
            if (_calibration == 2)
            {
                if (ConfigurationViewModel.EnableTogglePointer && ConfigurationViewModel.CalibrateTogglePointer)
                {
                    return ConfigurationViewModel.TogglePointerCalibrationMessage;
                }
                _calibration++;
            }
            return string.Empty;
        }

        private void CalibrationView_SaveCalibrationData(object sender, EventArgs e)
        {
            lock (_syncRoot)
            {
                _saveCalibrationData = true;
            }
        }

        private void CalibrationView_CountDownFinished(object sender, EventArgs e)
        {
            _calibration++;
            RunCalibration(_calibratingUser);
        }

        private void _kinectUser_Updated(object sender, ProcessEventArgs<IUserChangedEvent> e)
        {
            if (_saveCalibrationData)
            {
                lock (_syncRoot)
                {
                    switch (_calibration)
                    {
                        case 0:
                            ConfigurationViewModel.NextSlideCorrection =
                                FilterHelper.CalculateCorrection(e.Event.GetPoints(ConfigurationViewModel.NextSlide1,
                                                                                   ConfigurationViewModel.NextSlide2));
                            break;
                        case 1:
                            ConfigurationViewModel.PreviousSlideCorrection =
                                FilterHelper.CalculateCorrection(e.Event.GetPoints(
                                    ConfigurationViewModel.PreviousSlide1, ConfigurationViewModel.PreviousSlide2));
                            break;
                        case 2:
                            ConfigurationViewModel.TogglePointerCorrection =
                                FilterHelper.CalculateCorrection(e.Event.GetPoints(
                                    ConfigurationViewModel.TogglePointer1, ConfigurationViewModel.TogglePointer2));
                            break;
                    }
                    _saveCalibrationData = false;
                }
            }

            if (ConfigurationViewModel.EnableTogglePointer)
            {
                OnLaserUpdated(e.Event.Id, e.Event.GetPoint(ConfigurationViewModel.MovePointer));
            }
        }

        private bool CheckEventInterval()
        {
            if ((DateTime.Now - _lastHit).TotalMilliseconds > EventIntervalInMilliseconds)
            {
                _lastHit = DateTime.Now;
                return true;
            }
            return false;
        }

        internal void NoEventsForSomeSeconds(int seconds)
        {
            _lastHit = DateTime.Now.AddSeconds(seconds);
        }
    }
}