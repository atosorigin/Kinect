using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common;
using Kinect.Core.Gestures.Model;

namespace Kinect.WPF.Models
{
    public class SemaphoreImage : ICopyAble<SemaphoreImage>
    {
        public ImageSource ImageSource { get; private set; }

        public Semaphore Semaphore { get; private set; }

        private SemaphoreImage()
        {
            ////Needed for the copyable function
        }

        public SemaphoreImage(char semaphore)
        {
            this.Semaphore = Semaphores.GetSemaphore(semaphore);
            this.ImageSource = Get(Semaphore.Name);
        }

        public SemaphoreImage CreateCopy()
        {
            var semimage = new SemaphoreImage();
            semimage.ImageSource = ImageSource; ////Image on the disk will be the same
            semimage.Semaphore = Semaphore.CreateCopy();
            return semimage;
        }

        private ImageSource Get(string semaphore)
        {
            if (string.IsNullOrEmpty(semaphore))
            {
                return null;
            }

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(string.Format(@"/Kinect.WPF;component/Images/Semafoor/{0}.png", semaphore), UriKind.Relative);
            image.EndInit();

            return image;
        }
    }
}
