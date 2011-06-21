using System.Windows;
using GalaSoft.MvvmLight.Threading;
using Kinect.Common;
using Kinect.Semaphore.Models;
using log4net;

namespace Kinect.Semaphore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (App));

        private static BindableTraceListener traceListener;
        private SemaphoreGames _semaphoreGames;

        public App()
        {
            Initialize();
        }

        public static BindableTraceListener TraceListener
        {
            get
            {
                if (traceListener == null)
                {
                    traceListener = new BindableTraceListener();
                }
                return traceListener;
            }
        }

        private void Initialize()
        {
            DispatcherHelper.Initialize();
            _semaphoreGames = SemaphoreGames.Instance;
        }
    }
}