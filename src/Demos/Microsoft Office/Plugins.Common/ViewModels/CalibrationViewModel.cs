using GalaSoft.MvvmLight;
using System.Windows.Threading;
using System;
using System.Windows.Media;
using GalaSoft.MvvmLight.Threading;
using System.Diagnostics;
using GalaSoft.MvvmLight.Command;
using System.Windows;

namespace Kinect.Plugins.Common.ViewModels
{
    public class CalibrationViewModel : ViewModelBase
    {
        private static object _lockObject = new object();

        private int _countdown;
        public int CountDown 
        {   
            get
            { 
                return _countdown; 
            } 
            set 
            { 
                if (value != _countdown)
                { 
                    _countdown = value;
                    CountDownMessage = CountDown.ToString();
                    RaisePropertyChanged("CountDown"); 
                } 
            } 
        }

        private string _countdownmessage = string.Empty;
        public string CountDownMessage
        {
            get
            {
                return _countdownmessage;
            }
            set
            {
                if (value != _countdownmessage)
                {
                    _countdownmessage = value;
                    RaisePropertyChanged("CountDownMessage");
                }
            }
        }

        private string _calibrationMessage = string.Empty;
        public string CalibrationMessage 
        { 
            get 
            { 
                return _calibrationMessage; 
            } 
            set 
            {
                if (value != _calibrationMessage) 
                { 
                    _calibrationMessage = value; RaisePropertyChanged("CalibrationMessage"); 
                } 
            } 
        }

        private ImageSource _cameraView;
        public ImageSource CameraView
        {
            get { return _cameraView; }
            set
            {
                lock (_lockObject)
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

        private DispatcherTimer _countdownTimer;

        public event EventHandler CountDownFinished;
        private void OnCountDownFinished()
        {
            var handler = CountDownFinished;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        public event EventHandler SaveCalibrationData;
        private void OnSaveCalibrationData()
        {
            var handler = SaveCalibrationData;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        public static CalibrationViewModel Current { get; private set; }

        /// <summary>
        /// Initializes a new instance of the CalibrationViewModel class.
        /// </summary>
        public CalibrationViewModel()
        {
            Current = this;
            if (!IsInDesignMode)
            {
                _countdownTimer = new DispatcherTimer();
                _countdownTimer.Interval = new TimeSpan(0, 0, 1);
                _countdownTimer.Tick += new EventHandler(CountdownTimerStep);
                SetCommands();
            }
        }

        public void StartCountDown(string message, int countdown)
        {
            CalibrationMessage = message;
            CountDown = countdown;
            _countdownTimer.Start();
        }

        public void StartCountDown(int countdown)
        {
            StartCountDown(CalibrationMessage, countdown);
        }

        private void CountdownTimerStep(object sender, EventArgs e)
        {
            if (CountDown > 0)
            {
                CountDown--;
                if (CountDown == 0)
                {
                    CountDownMessage = "Saving";
                    OnSaveCalibrationData();
                }
            }
            else
            {
                CountDownMessage = string.Empty;
                _countdownTimer.Stop();
                OnCountDownFinished();
            }
        }

        private void Kinect_CameraDataUpdated(object sender, Core.KinectEventArgs e)
        {
            try
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    if (KinectManager.Instance.Kinect != null)
                    {
                        CameraView = KinectManager.Instance.Kinect.GetCameraView(Core.CameraView.ColoredDepth);
                    }
                });
            }
            catch (Exception ex)
            {
                Trace.Write(ex.StackTrace);
            }
        }

        public override void Cleanup()
        {
            Current = null;
            KinectManager.Instance.Kinect.CameraDataUpdated -= Kinect_CameraDataUpdated;
            base.Cleanup();
        }

        public RelayCommand<EventArgs> WindowDeactivated { get; protected set; }
        public RelayCommand<EventArgs> WindowActivated { get; protected set; }
        public RelayCommand<RoutedEventArgs> WindowLoaded { get; protected set; }

        private void SetCommands()
        {
            WindowDeactivated = new RelayCommand<EventArgs>(e =>
            {
                DissableCamera();
            });
            WindowActivated = new RelayCommand<EventArgs>(e =>
            {
                EnableCamera();
            });
            WindowLoaded = new RelayCommand<RoutedEventArgs>(e => {
                EnableCamera();
            });
        }

        private void EnableCamera()
        {
            if (KinectManager.Instance != null)
            {
                KinectManager.Instance.Kinect.CameraDataUpdated -= Kinect_CameraDataUpdated;
                KinectManager.Instance.Kinect.CameraDataUpdated += Kinect_CameraDataUpdated; 
            }
        }

        private void DissableCamera()
        {
            if (KinectManager.Instance != null)
            {
                KinectManager.Instance.Kinect.CameraDataUpdated -= Kinect_CameraDataUpdated;
            }
        }
    }
}