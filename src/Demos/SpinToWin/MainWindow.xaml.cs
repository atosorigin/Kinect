using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
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
        private bool _winnerVisible;

        public MainWindow()
        {
            ReadParticipants();
            _changeResolution = new ChangeResolution();
            _changeResolution.ChangeScreenResolution(1024, 768);
            InitializeComponent();
            InitializeData(_participants);
            InitKinect();
            piePlotter.Win += PiePlotterWin;
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
            //TODO: Enable when kinect is connected
            //_kinect.StartKinect();
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

        private void InitializeData(IEnumerable<string> participants)
        {
            // create our test dataset and bind it
            _pies = new ObservableCollection<PieData>(PieData.ConstructPies(participants));
            DataContext = _pies;
        }

        private void StartMouseLeave(object sender, MouseEventArgs e)
        {
            _start = DateTime.Now;
        }

        private void RotateMouseEnter(object sender, MouseEventArgs e)
        {
            _milliseconds = (DateTime.Now - _start).TotalMilliseconds;
            _start = DateTime.MinValue;
            if (_milliseconds < 1000)
            {
                SpinIt();
            }
        }

        private void PiePlotterWin(object sender, WinnerEventArgs winner)
        {
            _winnerVisible = true;
            PlayStoryBoard("WinnerVisibleStoryBoard", Winner);
            Action action = () =>
            {
                Winner.Text = string.Format("   {0}   ",winner.Winner);
            };
            Winner.Dispatcher.BeginInvoke(action);
            _participants.Remove(winner.Winner);
            LogWinner(winner.Winner);
        }

        private void RemoveWinnerMouseEnter(object sender, MouseEventArgs e)
        {
            if (!_winnerVisible) return;
            PlayStoryBoard("WinnerHiddenStoryBoard", Winner);
            PlayStoryBoard("WheelHiddenStoryBoard", piePlotter,
                () =>
                {
                    InitializeData(_participants);
                    PlayStoryBoard("WheelVisibleStoryBoard", piePlotter);
                });
            _winnerVisible = false;
        }

        private void PlayStoryBoard(string storyBoard, DependencyObject obj, Action action = null)
        {
            var sb = FindResource(storyBoard) as Storyboard;
            if (sb == null) return;
            Storyboard.SetTarget(sb, obj);
            if (action != null)
            {
                EventHandler completeAction = (sender, args) => action();
                sb.Completed += completeAction;
            }
            sb.Begin();
        }

        private void SpinIt()
        {
            if (_winnerVisible) return;
            var seconds = (int)(10000 - _milliseconds) / 1000;
            var angle = (int)((1800) - _milliseconds);
            piePlotter.RotatePies(angle, new TimeSpan(0, 0, seconds));
        }

        private void WindowKeyUp(object sender, KeyEventArgs e)
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

        private static void LogWinner(string winner)
        {
            try
            {
                using (var writer = new StreamWriter("Winnaars.txt",true))
                {
                    writer.WriteLine("{0}: {1}", DateTime.Now.ToLongTimeString(), winner);
                }
            }
// ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
            {
                //Do nothing
            }
// ReSharper restore EmptyGeneralCatchClause
        }
    }
}
