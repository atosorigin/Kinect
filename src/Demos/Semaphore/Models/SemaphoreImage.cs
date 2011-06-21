using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Kinect.Common;
using Kinect.Core.Gestures.Model;

namespace Kinect.Semaphore.Models
{
    public class SemaphoreImage : ICopyAble<SemaphoreImage>
    {
        private SemaphoreImage()
        {
            ////Needed for the copyable function
        }

        public SemaphoreImage(char semaphore)
        {
            Semaphore = Semaphores.GetSemaphore(semaphore);
            ImageSource = Get(Semaphore.Name);
        }

        public ImageSource ImageSource { get; private set; }

        public Core.Gestures.Model.Semaphore Semaphore { get; private set; }

        #region ICopyAble<SemaphoreImage> Members

        public SemaphoreImage CreateCopy()
        {
            var semimage = new SemaphoreImage();
            semimage.ImageSource = ImageSource; ////Image on the disk will be the same
            semimage.Semaphore = Semaphore.CreateCopy();
            return semimage;
        }

        #endregion

        private ImageSource Get(string semaphore)
        {
            if (string.IsNullOrEmpty(semaphore))
            {
                return null;
            }

            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(string.Format(@"/Kinect.Semaphore;component/Images/Semafoor/{0}.png", semaphore),
                                      UriKind.Relative);
            image.EndInit();

            return image;
        }
    }
}