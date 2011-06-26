using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Kinect.Core;
using GalaSoft.MvvmLight.Threading;

namespace Kinect.Workshop.ViewModels
{
    public abstract class WorkshopViewModelBase : ViewModelBase
    {
        protected MyKinect Kinect;

        protected WorkshopViewModelBase()
        {
            if (IsInDesignModeStatic)
            {
                Head = "Head";
                Neck = "Neck";
                LeftShoulder = "Left Shoulder";
                RightShoulder = "Right Shoulder";
                Torso = "Torso";
                LeftElbow = "Left Elbow";
                RightElbow = "Right Elbow";
                LeftHand = "Left Hand";
                RightHand = "Right Hand";
                LeftHand = "Left Hand";
                RightHand = "Right Hand";
                LeftHip = "Left Hip";
                RightHip = "Right Hip";
                LeftKnee = "Left Knee";
                RightKnee = "Right Knee";
                LeftFoot = "Left Foot";
                RightFoot = "Right Foot";
            }
            else
            {
                Kinect = MyKinect.Instance;
                PointerColor = new SolidColorBrush(Color.FromRgb(139, 0, 0));
                Messages = new ObservableCollection<string>();
                Start = new RelayCommand<RoutedEventArgs>(e => StartKinect());
                Stop = new RelayCommand<RoutedEventArgs>(e => StopKinect());
            }
        }

        protected void KinectCameraMessage(object sender, KinectMessageEventArgs e)
        {
            UpdateUserInterface(() => Messages.Add(e.Message));
        }

        protected void KinectCameraDataUpdated(object sender, KinectCameraEventArgs e)
        {
            UpdateUserInterface(() => Camera = e.Image);
        }

        private void StartKinect()
        {
            if (Kinect.KinectState == KinectState.Running) return;
            Messages.Clear();
            SubscribeToKinectEvents();
            Kinect.StartKinect();
        }

        private void StopKinect()
        {
            if (Kinect.KinectState != KinectState.Running) return;
            UnSubscribeToKinectEvents();
            Kinect.StopKinect();
            Camera = null;
        }

        public abstract void SubscribeToKinectEvents();
        public abstract void UnSubscribeToKinectEvents();
        public abstract void SubscribeToUserUpdatedEvent();
        public abstract void TrackRightHand();
        public abstract void AttachGesture();
        protected virtual void UpdateUserInterface(Action action)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(action);
        }

        public RelayCommand<RoutedEventArgs> Start { get; set; }
        public RelayCommand<RoutedEventArgs> Stop { get; set; }

        public ObservableCollection<string> Messages { get; set; }
        public string LastMessage
        {
            get { return Messages.LastOrDefault(); }
        }

        private ImageSource _camera;
        public ImageSource Camera
        {
            get { return _camera; }
            set
            {
                if (value == _camera) return;
                _camera = value;
                //TODO: Change to RaisePropertyChanged(() => CameraView)
                //when MvvMLight V4 is released
                RaisePropertyChanged("Camera");
            }
        }

        private Brush _pointerColor;
        public Brush PointerColor
        {
            get { return _pointerColor; }
            set
            {
                if (value == _pointerColor) return;
                _pointerColor = value;
                RaisePropertyChanged("PointerColor");
            }
        }

        private Point _pointerPosition;
        public Point PointerPosition
        {
            get { return _pointerPosition; }
            set
            {
                if (value == _pointerPosition) return;
                _pointerPosition = value;
                RaisePropertyChanged("PointerPosition");
            }
        }

        private string _head;
        public string Head
        {
            get { return _head; }
            set
            {
                if (value == _head) return;
                _head = value;
                RaisePropertyChanged("Head");
            }
        }

        private string _neck;
        public string Neck
        {
            get { return _neck; }
            set
            {
                if (value == _neck) return;
                _neck = value;
                RaisePropertyChanged("Neck");
            }
        }

        private string _leftShoulder;
        public string LeftShoulder
        {
            get { return _leftShoulder; }
            set
            {
                if (value == _leftShoulder) return;
                _leftShoulder = value;
                RaisePropertyChanged("LeftShoulder");
            }
        }

        private string _rightShoulder;
        public string RightShoulder
        {
            get { return _rightShoulder; }
            set
            {
                if (value == _rightShoulder) return;
                _rightShoulder = value;
                RaisePropertyChanged("RightShoulder");
            }
        }

        private string _torso;
        public string Torso
        {
            get { return _torso; }
            set
            {
                if (value == _torso) return;
                _torso = value;
                RaisePropertyChanged("Torso");
            }
        }

        private string _leftElbow;
        public string LeftElbow
        {
            get { return _leftElbow; }
            set
            {
                if (value == _leftElbow) return;
                _leftElbow = value;
                RaisePropertyChanged("LeftElbow");
            }
        }

        private string _rightElow;
        public string RightElbow
        {
            get { return _rightElow; }
            set
            {
                if (value == _rightElow) return;
                _rightElow = value;
                RaisePropertyChanged("RightElbow");
            }
        }

        private string _leftHand;
        public string LeftHand
        {
            get { return _leftHand; }
            set
            {
                if (value == _leftHand) return;
                _leftHand = value;
                RaisePropertyChanged("LeftHand");
            }
        }

        private string _rightHand;
        public string RightHand
        {
            get { return _rightHand; }
            set
            {
                if (value == _rightHand) return;
                _rightHand = value;
                RaisePropertyChanged("RightHand");
            }
        }

        private string _leftHip;
        public string LeftHip
        {
            get { return _leftHip; }
            set
            {
                if (value == _leftHip) return;
                _leftHip = value;
                RaisePropertyChanged("LeftHip");
            }
        }

        private string _rightHip;
        public string RightHip
        {
            get { return _rightHip; }
            set
            {
                if (value == _rightHip) return;
                _rightHip = value;
                RaisePropertyChanged("RightHip");
            }
        }

        private string _leftKnee;
        public string LeftKnee
        {
            get { return _leftKnee; }
            set
            {
                if (value == _leftKnee) return;
                _leftKnee = value;
                RaisePropertyChanged("LeftKnee");
            }
        }

        private string _rightKnee;
        public string RightKnee
        {
            get { return _rightKnee; }
            set
            {
                if (value == _rightKnee) return;
                _rightKnee = value;
                RaisePropertyChanged("RightKnee");
            }
        }

        private string _leftFoot;
        public string LeftFoot
        {
            get { return _leftFoot; }
            set
            {
                if (value == _leftFoot) return;
                _leftFoot = value;
                RaisePropertyChanged("LeftFoot");
            }
        }

        private string _rightFoot;
        public string RightFoot
        {
            get { return _rightFoot; }
            set
            {
                if (value == _rightFoot) return;
                _rightFoot = value;
                RaisePropertyChanged("RightFoot");
            }
        }
    }
}