using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Kinect.Common;
using Kinect.Core;
using Kinect.Core.Eventing;
using Kinect.SpinToWin.Controls;

namespace Kinect.SpinToWin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Size _screenResolution = new Size(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
        private readonly ChangeResolution _changeResolution;
        private ObservableCollection<PieData> _pies;
        private MyKinect _kinect;
        private User _currentUser;

        private DateTime _start;
        private double _milliseconds;
        private readonly List<string> _participants = new List<string>();

        public MainWindow()
        {
            ReadParticipants();
            _changeResolution = new ChangeResolution();
            _changeResolution.ChangeScreenResolution(1024, 768);
            InitializeComponent();
            InitializeData();
            InitKinect();
        }

        private void ReadParticipants()
        {
            using (var reader = new StreamReader("Deelnemerslijst.txt"))
                while (!reader.EndOfStream) 
                    _participants.Add(reader.ReadLine());
        }

        private void InitKinect()
        {
            _kinect = MyKinect.Instance;
            _kinect.ChangeMaxSkeletonPositions(.5f, .5f);
            _kinect.ElevationAngleInitialPosition = 10;
            _kinect.UserCreated += KinectUserCreated;
            _kinect.UserRemoved += KinectUserRemoved;
            //_kinect.KinectStarted += new EventHandler<KinectEventArgs>(_kinect_KinectStarted);
            _kinect.StartKinect();
        }

        private void KinectUserRemoved(object sender, KinectUserEventArgs e)
        {
            var lostUser = _kinect.GetUser(e.User.Id);
            if (lostUser != null && _currentUser == lostUser)
            {
                lostUser.Updated -= KinectUserUpdated;
                _currentUser = null;
                Dispatcher.BeginInvoke(new Func<Cursor>(() => Mouse.OverrideCursor = Cursors.Arrow));
            }
            //throw new NotImplementedException();
        }

        private void KinectUserCreated(object sender, KinectUserEventArgs e)
        {
            var user = _kinect.GetUser(e.User.Id);
            if (_currentUser!= null && user != _currentUser)
            {
                _currentUser.Updated -= KinectUserUpdated;
            }
            Dispatcher.BeginInvoke(new Func<Cursor>(() => Mouse.OverrideCursor = Cursors.None));
            
            _currentUser = user;
            _currentUser.Updated += KinectUserUpdated;
        }

        private void KinectUserUpdated(object sender, ProcessEventArgs<IUserChangedEvent> e)
        {
            var screenpoint = e.Event.HandLeft.ToScreenPosition(new Size(640, 480), _screenResolution);
            MouseSimulator.X = (int)screenpoint.X;
            MouseSimulator.Y = (int)screenpoint.Y;
            HandImage.Dispatcher.BeginInvoke(new Func<Thickness>(() => HandImage.Margin = new Thickness(screenpoint.X - 140, screenpoint.Y - 120, 0, 0)));

        }

        private void InitializeData()
        {
            // create our test dataset and bind it
            _pies = new ObservableCollection<PieData>(PieData.ConstructPies(_participants));
            DataContext = _pies;
        }

        private void StartMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _start = DateTime.Now;
        }

        private void RotateMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _milliseconds = (DateTime.Now - _start).TotalMilliseconds;
            _start = DateTime.MinValue;
            if (_milliseconds < 1000)
            {
                SpinIt();
            }
        }

        private void SpinIt()
        {
            var seconds = (int)(10000 - _milliseconds) / 1000;
            var angle = (int)((1800) - _milliseconds);
            piePlotter.RotatePies(angle, new TimeSpan(0, 0, seconds));
        }

        private void WindowKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Close();
                    break;
                case Key.Up:
                    if (_kinect != null) _kinect.MotorUp(2);
                    break;
                case Key.Down:
                    if (_kinect != null) _kinect.MotorDown(2);
                    break;
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_kinect != null)
            {
                _kinect.StopKinect();
            }
            _changeResolution.ChangeScreenResolutionBackToOriginal();
        }
    }
}
