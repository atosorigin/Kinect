using System;
using GalaSoft.MvvmLight.Threading;
using Kinect.Plugins.Common.Views;
using Microsoft.Office.Interop.PowerPoint;
using log4net;
using Office = Microsoft.Office.Core;

namespace Kinect.Plugins.PowerPoint2007
{
    public partial class ThisAddIn
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ThisAddIn));
        private PresentationOverlay _overlay;

        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
            Log.Debug("Startup");
            if (Application.Version == "12.0")
            {
                DispatcherHelper.Initialize();
                SubscribeEvents();
            }
            else
            {
                OnShutdown();
                Dispose();
            }
        }

        private void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
            UnSubscribeEvents();
        }

        private void SubscribeEvents()
        {
            Application.SlideShowBegin += Application_SlideShowBegin;
            Application.SlideShowEnd += Application_SlideShowEnd;
        }

        private void UnSubscribeEvents()
        {
            Application.SlideShowBegin -= Application_SlideShowBegin;
            Application.SlideShowEnd -= Application_SlideShowEnd;
        }

        private void Application_SlideShowBegin(SlideShowWindow Wn)
        {
            if (_overlay == null)
            {
                _overlay = new PresentationOverlay();
                _overlay.DataContext = new PresentationOverlayViewModel {SlideShowWindow = Wn};
                _overlay.Show();
            }
        }

        private void Application_SlideShowEnd(Presentation Pres)
        {
            if (_overlay != null)
            {
                _overlay.Close();
                _overlay = null;
            }
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            Log.Debug("Internal startup");
            Startup += ThisAddIn_Startup;
            Shutdown += ThisAddIn_Shutdown;
        }

        #endregion
    }
}