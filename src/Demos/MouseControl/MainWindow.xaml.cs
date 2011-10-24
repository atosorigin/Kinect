using System.Windows;
using GalaSoft.MvvmLight.Threading;

namespace Kinect.MouseControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DispatcherHelper.Initialize();
            InitializeComponent();
        }
    }
}
