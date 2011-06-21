using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Kinect.Plugins.Common.ViewModels;
using Microsoft.Office.Interop.PowerPoint;

namespace Kinect.Plugins.PowerPoint2007
{
    public class PresentationOverlayViewModel : PowerpointOverlayViewModelBase
    {
        public PresentationOverlayViewModel()
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

        public SlideShowWindow SlideShowWindow { get; set; }

        private void SetCommands()
        {
            WindowLoaded = new RelayCommand<RoutedEventArgs>(e => { InitializeKinect(); });
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