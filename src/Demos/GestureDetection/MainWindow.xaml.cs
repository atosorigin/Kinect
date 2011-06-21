using System.Windows;

namespace Kinect.GestureDetection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DockPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //var x = new GestureDetection.Models.GestureDetection();
        }
    }
}