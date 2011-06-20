using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Media.Media3D;
using System.Threading;
using Kinect.Plugins.Common;
using System.Windows;
using Kinect.Plugins.Common.ViewModels;

namespace Kinect.Plugins.PowerPoint2007
{
    public class PresentationOverlayViewModel : PowerpointOverlayViewModelBase
    {
        public Microsoft.Office.Interop.PowerPoint.SlideShowWindow SlideShowWindow { get; set; }

        public PresentationOverlayViewModel()
            : base()
        {
            if (!IsInDesignMode)
            {
                //StartMouseAndLaserSimulation();
                SetCommands();
            }
            else
            {

            }
        }

        private void SetCommands()
        {
            WindowLoaded = new RelayCommand<RoutedEventArgs>(e =>
            {
                InitializeKinect();
            });
            KeyDownCommand = new RelayCommand<KeyEventArgs>(e =>
            {
                if (CameraCommand(e))
                {
                    //Done in baseclass
                }
                else if (e.Key == Key.Escape)
                {
                    StopMouseAndLaserSimulation();
                    //TODO We need a better soloution. On presentation end!
                    Cleanup();
                    SlideShowWindow.View.Exit();
                }
            });
        }

        protected override void NextSlide()
        {
            SlideShowWindow.View.Next();
        }

        protected override void PreviousSlide()
        {
            SlideShowWindow.View.Previous();
        }
    }
}
