using System;
using System.Windows;
using Common;
using GalaSoft.MvvmLight.Threading;
using Kinect.Core;
using Kinect.Core.Filters.Helper;
using Kinect.Core.Gestures;
using Kinect.Plugins.Common.ViewModels;
using Kinect.Plugins.Common.Views;
using log4net;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using System.Linq;

namespace Kinect.Plugins.Common
{
    public class KinectManager
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(KinectManager));

        private static readonly KinectManager _instance = new KinectManager();
        public static KinectManager Instance { get { return _instance; } }

        private static object _syncRoot = new object();
        //private User _kinectUser;

        private List<User> _kinectUsers;
        private User _calibratingUser;

        private MyKinect _kinect;
        internal MyKinect Kinect
        {
            get { return _kinect != null && _kinect.KinectState == KinectState.Running ? _kinect : null; }
        }

        private DateTime _lastHit = DateTime.Now;
        private bool _saveCalibrationData = false;
        private int _calibration = 0;
        private bool _userCalibrated = false;
        private Calibration _calibrationView;
        private CalibrationViewModel _calibrationViewModel;
        public ConfigureKinectViewModel ConfigurationViewModel { get; private set; }
        private SelfTouchGesture _nextSlideSelfTouch, _previousSlideSelfTouch, _togglePointerSelfTouch;

        public int EventIntervalInMilliseconds { get; set; }
        public bool Running { get; private set; }

        public void StartKinect()
        {
            _kinectUsers = new List<User>();
            _kinect.UserCreated += _kinect_UserCreated;
            _kinect.UserRemoved += _kinect_UserRemoved;
            _kinect.KinectStarted += _kinect_Started;
            _kinect.KinectStopped += _kinect_Stopped;
            _kinect.KinectCrashed += _kinect_KinectCrashed;
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
                _log.DebugFormat("Start calibration for user {0}", user.ID);
                _calibration = 0;
                _calibratingUser = user;
                OnUserCalibrating();
                RunCalibration(_calibratingUser);
            }
        }

        internal event EventHandler<SinglePointEventArgs> LaserUpdated;
        private void OnLaserUpdated(uint userid, Point3D point)
        {
            //TODO: Hier was ik gebleven. Toggle laser aan user binden
            var handler = LaserUpdated;
            if (handler != null)
            {
                handler.Invoke(this, new SinglePointEventArgs(userid,point));
            }
        }

        internal event EventHandler<UserEventArgs> NextSlide;
        private void OnNextSlide(uint userid)
        {
            var handler = NextSlide;
            if (handler != null)
            {
                handler.Invoke(this, new UserEventArgs(userid));
            }
        }

        internal event EventHandler<UserEventArgs> PreviousSlide;
        private void OnPreviousSlide(uint userid)
        {
            var handler = PreviousSlide;
            if (handler != null)
            {
                handler.Invoke(this, new UserEventArgs(userid));
            }
        }

        internal event EventHandler<UserEventArgs> TogglePointer;
        private void OnTogglePointer(uint userID)
        {
            var handler = TogglePointer;
            if (handler != null)
            {
                handler.Invoke(this, new UserEventArgs(userID));
            }
        }

        public event EventHandler UserCalibrating;
        private void OnUserCalibrating()
        {
            var handler = UserCalibrating;
            if (handler != null)
            {
                handler.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler UserCalibrated;
        private void OnUserCalibrated()
        {
            var handler = UserCalibrated;
            if (handler != null)
            {
                handler.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler<UserEventArgs> UserFound;
        private void OnUserFound(uint id)
        {
            var handler = UserFound;
            if (handler != null)
            {
                handler.Invoke(this, new UserEventArgs(id));
            }
        }

        public event EventHandler<UserEventArgs> UserLost;
        private void OnUserLost(uint id)
        {
            var handler = UserLost;
            if (handler != null)
            {
                handler.Invoke(this, new UserEventArgs(id));
            }
        }

        public event EventHandler KinectStarted;
        private void OnKinectStarted()
        {
            var handler = KinectStarted;
            if (handler != null)
            {
                handler.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler KinectStopped;
        private void OnKinectStopped()
        {
            var handler = KinectStopped;
            if (handler != null)
            {
                handler.Invoke(this, new EventArgs());
            }
        }

        private KinectManager()
        {
            EventIntervalInMilliseconds = 2000;
            _kinect = MyKinect.Instance;
            _kinect.SingleUserMode = true;
            ConfigurationViewModel = new ConfigureKinectViewModel();
            _calibrationView = new Calibration();
            _calibrationViewModel = CalibrationViewModel.Current;

            if (_calibrationViewModel != null)
            {
                _calibrationViewModel.SaveCalibrationData += CalibrationView_SaveCalibrationData;
                _calibrationViewModel.CountDownFinished += CalibrationView_CountDownFinished;
            }
        }

        private void _kinect_Started(object sender, Core.KinectEventArgs e)
        {
            Running = true;
            OnKinectStarted();
            System.Threading.Thread.Sleep(100);
            ShowCalibrationMessage("Please initialise user");
        }

        private void _kinect_Stopped(object sender, Core.KinectEventArgs e)
        {
            CleanUpAfterKinectStopped();
        }

        private void CleanUpAfterKinectStopped()
        {
            Running = false;
            _userCalibrated = false;
            OnKinectStopped();
        }

        private void _kinect_UserRemoved(object sender, Core.KinectUserEventArgs e)
        {
            if (_kinectUsers != null)
            {

                var user = (from kinectuser in _kinectUsers where kinectuser.ID == e.User.ID select kinectuser).FirstOrDefault();
                if (user != null)
                {
                    _kinectUsers.Remove(user);
                    _log.DebugFormat("User {0} removed from the list of active Powerpoint users", user.ID);
                }
                //_kinectUser = null;
            }
            OnUserLost(e.User.ID);
        }

        private void _kinect_UserCreated(object sender, Core.KinectUserEventArgs e)
        {

            if (_kinectUsers.Count > 0 && !_userCalibrated)
            {
                //De gebruiker is al aan het initialiseren
                _log.DebugFormat("User {0} ignored, because another user is calibrating",e.User.ID);
                return;
            }

            var user = _kinect.GetUser(e.User.ID);
            user.Updated += _kinectUser_Updated;
            _kinectUsers.Add(user);
            _log.DebugFormat("User {0} added to the list of active Powerpoint users", user.ID);
            OnUserFound(e.User.ID);

            if (_userCalibrated)
            {
                AddUserTouchEvents(user);
            }
            else
            {
                StartCalibration(user);
            }
        }

        private void _kinect_KinectCrashed(object sender, Core.KinectEventArgs e)
        {
            CleanUpAfterKinectStopped();
            MessageBox.Show("Kinect is gecrashed, herstart Kinect");
        }

        private void AddUserTouchEvents(User user)
        {
            //Need to lock. If the user is lost, you don't want that the user is getting set to null during adding of events
            lock (_syncRoot)
            {
                if (ConfigurationViewModel.EnableNextSlide)
                {
                    _nextSlideSelfTouch = user.AddSelfTouchGesture(ConfigurationViewModel.NextSlideCorrection, ConfigurationViewModel.NextSlide1, ConfigurationViewModel.NextSlide2);
                    _nextSlideSelfTouch.SelfTouchDetected +=
                        ((s, evt) =>
                        {
                            if (CheckEventInterval())
                            {
                                _log.DebugFormat("Next Slide \tUser:{0}", evt.UserID);
                                OnNextSlide(evt.UserID);
                            }
                        });
                    _log.DebugFormat("Added NextSlideEvent\tUser:{0}\tCorrection:{1}\t({2} on {3})", user.ID, ConfigurationViewModel.NextSlideCorrection.GetDebugString(),ConfigurationViewModel.NextSlide1,ConfigurationViewModel.NextSlide2);
                }

                if (ConfigurationViewModel.EnablePreviousSlide)
                {
                    _previousSlideSelfTouch = user.AddSelfTouchGesture(ConfigurationViewModel.PreviousSlideCorrection, ConfigurationViewModel.PreviousSlide1, ConfigurationViewModel.PreviousSlide2);
                    _previousSlideSelfTouch.SelfTouchDetected +=
                        ((s, evt) =>
                        {
                            if (CheckEventInterval())
                            {
                                _log.DebugFormat("Previous Slide \tUser:{0}", evt.UserID);
                                OnPreviousSlide(evt.UserID);
                            }
                        });
                    _log.DebugFormat("Added PreviousSlideEvent\tUser:{0}\tCorrection:{1}\t({2} on {3})", user.ID, ConfigurationViewModel.PreviousSlideCorrection.GetDebugString(),ConfigurationViewModel.PreviousSlide1,ConfigurationViewModel.PreviousSlide2);
                }

                if (ConfigurationViewModel.EnableTogglePointer)
                {
                    _togglePointerSelfTouch = user.AddSelfTouchGesture(ConfigurationViewModel.TogglePointerCorrection, ConfigurationViewModel.TogglePointer1, ConfigurationViewModel.TogglePointer2);
                    _togglePointerSelfTouch.SelfTouchDetected +=
                        ((s, evt) =>
                        {
                            if (CheckEventInterval())
                            {
                                _log.DebugFormat("TogglePointer \tUser:{0}", evt.UserID);
                                OnTogglePointer(evt.UserID);
                            }
                        });
                    _log.DebugFormat("Added TogglePointerEvent\tUser:{0}\tCorrection:{1}\t({2} on {3})", user.ID, ConfigurationViewModel.TogglePointerCorrection.GetDebugString(), ConfigurationViewModel.TogglePointer1, ConfigurationViewModel.TogglePointer2);
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
            var message = GetCalibrationMessage();
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

        private void _kinectUser_Updated(object sender, Core.Eventing.ProcessEventArgs<IUserChangedEvent> e)
        {
            if (_saveCalibrationData)
            {
                lock (_syncRoot)
                {
                    switch (_calibration)
                    {
                        case 0: ConfigurationViewModel.NextSlideCorrection = FilterHelper.CalculateCorrection(e.Event.GetPoints(ConfigurationViewModel.NextSlide1, ConfigurationViewModel.NextSlide2)); break;
                        case 1: ConfigurationViewModel.PreviousSlideCorrection = FilterHelper.CalculateCorrection(e.Event.GetPoints(ConfigurationViewModel.PreviousSlide1, ConfigurationViewModel.PreviousSlide2)); break;
                        case 2: ConfigurationViewModel.TogglePointerCorrection = FilterHelper.CalculateCorrection(e.Event.GetPoints(ConfigurationViewModel.TogglePointer1, ConfigurationViewModel.TogglePointer2)); break;
                    }
                    _saveCalibrationData = false;
                }
            }

            if (ConfigurationViewModel.EnableTogglePointer)
            {
                OnLaserUpdated(e.Event.ID,e.Event.GetPoint(ConfigurationViewModel.MovePointer));
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
