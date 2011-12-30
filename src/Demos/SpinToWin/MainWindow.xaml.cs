using System;
using System.Collections.ObjectModel;
using System.Windows;
using Kinect.SpinToWin.Controls;

namespace Kinect.SpinToWin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<PieData> _pies;

        private DateTime _start;
        private double _milliseconds;

        public MainWindow()
        {
            InitializeComponent();
            InitializeData();
        }

        private void InitializeData()
        {
            // create our test dataset and bind it
            _pies = new ObservableCollection<PieData>(PieData.ConstructPies());
            DataContext = _pies;
        }

        private void Window_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
        }

        private void Start_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _start = DateTime.Now;
        }

        private void Start_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void Rotate_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void Rotate_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _milliseconds = (DateTime.Now - _start).TotalMilliseconds;
            _start = DateTime.MinValue;
            if (_milliseconds < 1000)
            {
                TimeTextBloxk.Text = _milliseconds.ToString();
                SpinIt();
            }
        }

        private void SpinIt()
        {
            var seconds = (int)(5000 - _milliseconds) / 1000;
            var angle = (int)((3360) - _milliseconds);
            piePlotter.RotatePies(angle, new TimeSpan(0, 0, seconds));
        }
    }
}
