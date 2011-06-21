using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Kinect.Core
{
    public class KinectCameraEventArgs : KinectEventArgs
    {
        public BitmapSource Image { get; private set; }
        public CameraView CameraType { get; private set; }
        
        public KinectCameraEventArgs(BitmapSource image, CameraView type)
        {
            Image = image;
            CameraType = type;
        }
    }
}
