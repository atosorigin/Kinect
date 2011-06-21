using System.Windows.Media.Imaging;

namespace Kinect.Core
{
    public class KinectCameraEventArgs : KinectEventArgs
    {
        public KinectCameraEventArgs(BitmapSource image, CameraView type)
        {
            Image = image;
            CameraType = type;
        }

        public BitmapSource Image { get; private set; }
        public CameraView CameraType { get; private set; }
    }
}