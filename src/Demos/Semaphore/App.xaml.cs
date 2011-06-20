using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Common;
using GalaSoft.MvvmLight.Threading;
using log4net;

namespace Kinect.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(App));

        private Models.SemaphoreGames _semaphoreGames;
        static BindableTraceListener traceListener;

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
            _semaphoreGames = Models.SemaphoreGames.Instance;
        }
    }
}
