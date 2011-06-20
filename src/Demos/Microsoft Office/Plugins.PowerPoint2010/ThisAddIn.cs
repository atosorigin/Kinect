using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Office = Microsoft.Office.Core;
using GalaSoft.MvvmLight.Threading;
using Kinect.Plugins.Common;
using Kinect.Plugins.Common.Views;

namespace Kinect.Plugins.PowerPoint2010
{
    public partial class ThisAddIn
    {
        private PresentationOverlay _overlay;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
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

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
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

        private void Application_SlideShowBegin(PowerPoint.SlideShowWindow Wn)
        {
            _overlay = new PresentationOverlay();
            _overlay.DataContext = new PresentationOverlayViewModel() { SlideShowWindow = Wn };
            _overlay.Show();
        }

        private void Application_SlideShowEnd(PowerPoint.Presentation Pres)
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
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
