using System.Windows;
using GalaSoft.MvvmLight.Threading;

namespace Kinect.GestureDetection
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Initialize();
        }

        private void Initialize()
        {
            DispatcherHelper.Initialize();
        }
    }
}