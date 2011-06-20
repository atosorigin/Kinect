using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Kinect.Common.ColorHelpers;
using Kinect.Core.Exceptions;
using xn;

namespace Kinect.Core
{
    /// <summary>
    /// For getting the image from the Kinect camera
    /// </summary>
    public class Camera : INotifyPropertyChanged
    {
        /// <summary>
        /// Colors for colored depthview
        /// </summary>
        private readonly Color[] _colors = {
                                               Colors.Red, Colors.Blue, Colors.ForestGreen, Colors.Yellow, Colors.Orange,
                                               Colors.Purple
                                           };

        /// <summary>
        /// Frames per second
        /// </summary>
        private int _fps;

        /// <summary>
        /// Holds the framecount
        /// </summary>
        private int _frames;

        /// <summary>
        /// Instance of imagegenerator
        /// </summary>
        private ImageGenerator _image;

        /// <summary>
        /// Holds last frames log time
        /// </summary>
        private int _lastFPSlog;

        /// <summary>
        /// Gets or sets The Kinect context
        /// </summary>
        public Context Context { get; set; }

        /// <summary>
        /// Gets or sets type of CameraView
        /// </summary>
        public CameraView ViewType { get; set; }

        /// <summary>
        /// Gets the current image of the Camera
        /// </summary>
        public BitmapSource View { get; private set; }

        /// <summary>
        /// Gets the imagedepthgenerator
        /// </summary>
        public DepthGenerator Depth { get; private set; }

        /// <summary>
        /// Gets the usergenerator
        /// </summary>
        public UserGenerator UserGenerator { get; private set; }

        /// <summary>
        /// Gets or sets wether Kinect is Running
        /// </summary>
        public bool Running { get; set; }

        /// <summary>
        /// Gets the Frames Per Second rate
        /// </summary>
        public int Fps
        {
            get { return _fps; }
            private set
            {
                if (value != _fps)
                {
                    _fps = value;
                    OnPropertyChanged("Fps");
                }
            }
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Event for notifying if a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <returns>The CamerView</returns>
        public BitmapSource GetView()
        {
            return GetView(ViewType);
        }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <returns>The CameraView</returns>
        public BitmapSource GetView(CameraView viewType)
        {
            ViewType = viewType;
            if (!Running || ViewType == CameraView.None)
            {
                return null;
            }

            switch (ViewType)
            {
                case CameraView.Color:
                    View = GetColorImage();
                    break;
                case CameraView.Depth:
                    View = GetDepthImage();
                    break;
                case CameraView.ColoredDepth:
                    View = GetColoredDepthWithImage();
                    break;
                default:
                    break;
            }

            return View;
        }

        /// <summary>
        /// Initializes the specified usergenerator.
        /// </summary>
        /// <param name="usergenerator">The usergenerator.</param>
        internal void Initialize(UserGenerator usergenerator)
        {
            var generator = new ColorGenerator();
            for (int i = 0; i < _colors.Length; i++)
            {
                _colors[i] = generator.NextColor();
            }

            UserGenerator = usergenerator;

            Depth = Context.FindExistingNode(NodeType.Depth) as DepthGenerator;
            if (Depth == null)
            {
                throw new CameraException("Viewer must have a depth node!");
            }

            _image = Context.FindExistingNode(NodeType.Image) as ImageGenerator;
            if (_image == null)
            {
                throw new CameraException("Viewer must have a image node!");
            }
        }

        /// <summary>
        /// Sets the color of the user.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="color">The color.</param>
        internal void SetUserColor(uint id, Color color)
        {
            _colors[id%_colors.Length] = color;
        }

        /// <summary>
        /// Gets the color of the user.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        internal Color GetUserColor(uint id)
        {
            return _colors[id%_colors.Length];
        }

        /// <summary>
        /// Calculates the FPS.
        /// </summary>
        internal void CalculateFPS()
        {
            _frames++;
            int time = Environment.TickCount;
            if (time > _lastFPSlog + 1000)
            {
                Fps = _frames;
                _frames = 0;
                _lastFPSlog = time;
            }
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Gets the depth image bytes.
        /// </summary>
        /// <param name="colorUsers">if set to <c>true</c> [color users].</param>
        /// <param name="resolutionX">The resolution X.</param>
        /// <param name="resolutionY">The resolution Y.</param>
        /// <returns>The depthimage bytes </returns>
        private byte[] GetDepthImageBytes(bool colorUsers, out int resolutionX, out int resolutionY)
        {
            resolutionX = 0;
            resolutionY = 0;
            if (!Running || Depth == null || (colorUsers && UserGenerator == null))
            {
                return null;
            }

            ushort[] depths = GetDepths(out resolutionX, out resolutionY);
            if (depths == null)
            {
                return null;
            }

            const int BytesPerPixel = 3;

            var usermap = new ushort[1];
            ////Only get the information if the user needs to be coloured
            if (colorUsers)
            {
                usermap = GetUserColors();
            }
            //// convert the depths to a grayscale image
            var bytes = new byte[depths.Length*BytesPerPixel];
            for (int depthIndex = 0; depthIndex < depths.Length; depthIndex++)
            {
                int pixelIndex = depthIndex*BytesPerPixel;
                ushort singleDepth = depths[depthIndex];
                var gray = (ushort) (singleDepth == 0 ? 0x00 : (0xFF - (singleDepth >> 4)));

                ushort user = 0;
                ////Only get the information if the user needs to be coloured
                if (colorUsers)
                {
                    user = usermap[depthIndex];
                }

                if (user != 0)
                {
                    Color labelColor = _colors[user%_colors.Length];
                    bytes[pixelIndex] = (byte) (gray*(labelColor.B/256.0));
                    bytes[pixelIndex + 1] = (byte) (gray*(labelColor.G/256.0));
                    bytes[pixelIndex + 2] = (byte) (gray*(labelColor.R/256.0));
                }
                else
                {
                    bytes[pixelIndex] = (byte) gray;
                    bytes[pixelIndex + 1] = (byte) gray;
                    bytes[pixelIndex + 2] = (byte) gray;
                }
            }

            return bytes;
        }

        /// <summary>
        /// Gets the depth image.
        /// </summary>
        /// <returns>The depth image bitmapsource</returns>
        private BitmapSource GetDepthImage()
        {
            int resolutionX;
            int resolutionY;
            byte[] bytes = GetDepthImageBytes(false, out resolutionX, out resolutionY);

            if (bytes == null)
            {
                return null;
            }

            return BitmapSource.Create(resolutionX, resolutionY, 96, 96, PixelFormats.Rgb24, null, bytes, resolutionX*3);
        }

        /// <summary>
        /// Gets the colored depth with image.
        /// </summary>
        /// <returns>The colored depth image bitmapsource</returns>
        private BitmapSource GetColoredDepthWithImage()
        {
            int resolutionX;
            int resolutionY;
            byte[] bytes = GetDepthImageBytes(true, out resolutionX, out resolutionY);

            if (bytes == null)
            {
                return null;
            }

            return BitmapSource.Create(resolutionX, resolutionY, 96, 96, PixelFormats.Rgb24, null, bytes, resolutionX*3);
        }

        /// <summary>
        /// Gets the color image.
        /// </summary>
        /// <returns>The colored image bitmapsource</returns>
        private BitmapSource GetColorImage()
        {
            ImageMetaData metadata = _image.GetMetaData();
            const int BytesPerPixel = 3;
            int totalPixels = metadata.XRes*metadata.YRes;

            IntPtr imageMapPtr = _image.GetImageMapPtr();

            return BitmapSource.Create(metadata.XRes, metadata.YRes, 96, 96, PixelFormats.Rgb24, null, imageMapPtr,
                                       totalPixels*BytesPerPixel, metadata.XRes*BytesPerPixel);
        }

        /// <summary>
        /// Gets the depths.
        /// </summary>
        /// <param name="resolutionX">The resolution X.</param>
        /// <param name="resolutionY">The resolution Y.</param>
        /// <returns>The image depths</returns>
        private ushort[] GetDepths(out int resolutionX, out int resolutionY)
        {
            resolutionX = 0;
            resolutionY = 0;

            if (Depth == null || !Running)
            {
                return null;
            }

            // calculate the core metadata
            DepthMetaData metadata = Depth.GetMetaData();
            resolutionX = metadata.XRes;
            resolutionY = metadata.YRes;
            int totalDepths = metadata.XRes*metadata.YRes;

            // copy the depths
            // TODO: Is there a better way to marshal ushorts from an IntPtr?
            IntPtr depthMapPtr = Depth.GetDepthMapPtr();
            var depthsTemp = new short[totalDepths];
            Marshal.Copy(depthMapPtr, depthsTemp, 0, depthsTemp.Length);
            var depths = new ushort[totalDepths];
            Buffer.BlockCopy(depthsTemp, 0, depths, 0, totalDepths*metadata.BytesPerPixel);

            return depths;
        }

        /// <summary>
        /// Gets the user colors.
        /// </summary>
        /// <returns>The user colors</returns>
        private ushort[] GetUserColors()
        {
            if (UserGenerator == null || !Running)
            {
                return null;
            }

            SceneMetaData sceneMetaData = UserGenerator.GetUserPixels(0);

            int size = sceneMetaData.XRes*sceneMetaData.YRes;
            var úsersTemp = new short[size];
            Marshal.Copy(sceneMetaData.SceneMapPtr, úsersTemp, 0, úsersTemp.Length);
            var users = new ushort[size];
            Buffer.BlockCopy(úsersTemp, 0, users, 0, size*sceneMetaData.BytesPerPixel);

            return users;
        }
    }
}