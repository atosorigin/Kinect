using System;
using GalaSoft.MvvmLight.Threading;
using Kinect.Plugins.Common.Views;
using Microsoft.Office.Interop.PowerPoint;
using Office = Microsoft.Office.Core;

namespace Kinect.Plugins.PowerPoint2010
{
    public partial class ThisAddIn
    {
        private PresentationOverlay _overlay;

        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
            if (Application.Version == "14.0")
            {
                SubscribeEvents();
                DispatcherHelper.Initialize();
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
            _overlay = new PresentationOverlay();
            _overlay.DataContext = new PresentationOverlayViewModel {SlideShowWindow = Wn};
            _overlay.Show();
        }

        private void Application_SlideShowEnd(Presentation Pres)
        {
            _overlay.Close();
            _overlay = null;
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            Startup += ThisAddIn_Startup;
            Shutdown += ThisAddIn_Shutdown;
        }

        #endregion
    }
}