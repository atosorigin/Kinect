using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common.ColorHelpers;
using xn;
using Kinect.Core.Exceptions;

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
        private Color[] _colors = { Colors.Red, Colors.Blue, Colors.ForestGreen, Colors.Yellow, Colors.Orange, Colors.Purple };

        /// <summary>
        /// Holds last frames log time
        /// </summary>
        private int _lastFPSlog = 0;

        /// <summary>
        /// Holds the framecount
        /// </summary>
        private int _frames = 0;

        /// <summary>
        /// Frames per second
        /// </summary>
        private int _fps = 0;

        /// <summary>
        /// Instance of imagegenerator
        /// </summary>
        private ImageGenerator _image;

        /// <summary>
        /// Event for notifying if a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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
            get
            {
                return this._fps;
            }
            private set
            {
                if (value != this._fps)
                {
                    this._fps = value;
                    this.OnPropertyChanged("Fps");
                }
            }
        }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <returns>The CamerView</returns>
        public BitmapSource GetView()
        {
            return this.GetView(this.ViewType);
        }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <returns>The CameraView</returns>
        public BitmapSource GetView(CameraView viewType)
        {
            this.ViewType = viewType;
            if (!this.Running || this.ViewType == CameraView.None)
            {
                return null;
            }

            switch (this.ViewType)
            {
                case CameraView.Color:
                    this.View = this.GetColorImage();
                    break;
                case CameraView.Depth:
                    this.View = this.GetDepthImage();
                    break;
                case CameraView.ColoredDepth:
                    this.View = this.GetColoredDepthWithImage();
                    break;
                default:
                    break;
            }

            return this.View;
        }

        /// <summary>
        /// Initializes the specified usergenerator.
        /// </summary>
        /// <param name="usergenerator">The usergenerator.</param>
        internal void Initialize(UserGenerator usergenerator)
        {
            var generator = new ColorGenerator();
            for (int i = 0; i < this._colors.Length; i++)
            {
                this._colors[i] = generator.NextColor();
            }

            this.UserGenerator = usergenerator;

            this.Depth = Context.FindExistingNode(NodeType.Depth) as DepthGenerator;
            if (this.Depth == null)
            {
                throw new CameraException("Viewer must have a depth node!");
            }

            this._image = this.Context.FindExistingNode(NodeType.Image) as ImageGenerator;
            if (this._image == null)
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
            this._colors[id % this._colors.Length] = color;
        }

        /// <summary>
        /// Gets the color of the user.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        internal Color GetUserColor(uint id)
        {
            return this._colors[id % this._colors.Length];
        }

        /// <summary>
        /// Calculates the FPS.
        /// </summary>
        internal void CalculateFPS()
        {
            this._frames++;
            int time = System.Environment.TickCount;
            if (time > this._lastFPSlog + 1000)
            {
                this.Fps = this._frames;
                this._frames = 0;
                this._lastFPSlog = time;
            }
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
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
            if (!this.Running || this.Depth == null || (colorUsers && this.UserGenerator == null))
            {
                return null;
            }

            ushort[] depths = this.GetDepths(out resolutionX, out resolutionY);
            if (depths == null)
            {
                return null;
            }

            const int BytesPerPixel = 3;

            ushort[] usermap = new ushort[1];
            ////Only get the information if the user needs to be coloured
            if (colorUsers)
            {
                usermap = this.GetUserColors();
            }
            //// convert the depths to a grayscale image
            byte[] bytes = new byte[depths.Length * BytesPerPixel];
            for (int depthIndex = 0; depthIndex < depths.Length; depthIndex++)
            {
                int pixelIndex = depthIndex * BytesPerPixel;
                ushort singleDepth = depths[depthIndex];
                ushort gray = (ushort)(singleDepth == 0 ? 0x00 : (0xFF - (singleDepth >> 4)));

                ushort user = 0;
                ////Only get the information if the user needs to be coloured
                if (colorUsers)
                {
                    user = usermap[depthIndex];
                }

                if (user != 0)
                {
                    Color labelColor = this._colors[user % this._colors.Length];
                    bytes[pixelIndex] = (byte)(gray * (labelColor.B / 256.0));
                    bytes[pixelIndex + 1] = (byte)(gray * (labelColor.G / 256.0));
                    bytes[pixelIndex + 2] = (byte)(gray * (labelColor.R / 256.0));
                }
                else
                {
                    bytes[pixelIndex] = (byte)gray;
                    bytes[pixelIndex + 1] = (byte)gray;
                    bytes[pixelIndex + 2] = (byte)gray;
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
            var bytes = this.GetDepthImageBytes(false, out resolutionX, out resolutionY);

            if (bytes == null)
            {
                return null;
            }

            return BitmapSource.Create(resolutionX, resolutionY, 96, 96, PixelFormats.Rgb24, null, bytes, resolutionX * 3);
        }

        /// <summary>
        /// Gets the colored depth with image.
        /// </summary>
        /// <returns>The colored depth image bitmapsource</returns>
        private BitmapSource GetColoredDepthWithImage()
        {
            int resolutionX;
            int resolutionY;
            var bytes = this.GetDepthImageBytes(true, out resolutionX, out resolutionY);

            if (bytes == null)
            {
                return null;
            }

            return BitmapSource.Create(resolutionX, resolutionY, 96, 96, PixelFormats.Rgb24, null, bytes, resolutionX * 3);
        }

        /// <summary>
        /// Gets the color image.
        /// </summary>
        /// <returns>The colored image bitmapsource</returns>
        private BitmapSource GetColorImage()
        {
            ImageMetaData metadata = this._image.GetMetaData();
            const int BytesPerPixel = 3;
            int totalPixels = metadata.XRes * metadata.YRes;

            IntPtr imageMapPtr = this._image.GetImageMapPtr();

            return BitmapSource.Create(metadata.XRes, metadata.YRes, 96, 96, PixelFormats.Rgb24, null, imageMapPtr, totalPixels * BytesPerPixel, metadata.XRes * BytesPerPixel);
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

            if (this.Depth == null || !this.Running)
            {
                return null;
            }

            // calculate the core metadata
            DepthMetaData metadata = this.Depth.GetMetaData();
            resolutionX = metadata.XRes;
            resolutionY = metadata.YRes;
            int totalDepths = metadata.XRes * metadata.YRes;

            // copy the depths
            // TODO: Is there a better way to marshal ushorts from an IntPtr?
            IntPtr depthMapPtr = this.Depth.GetDepthMapPtr();
            short[] depthsTemp = new short[totalDepths];
            Marshal.Copy(depthMapPtr, depthsTemp, 0, depthsTemp.Length);
            ushort[] depths = new ushort[totalDepths];
            Buffer.BlockCopy(depthsTemp, 0, depths, 0, totalDepths * metadata.BytesPerPixel);

            return depths;
        }

        /// <summary>
        /// Gets the user colors.
        /// </summary>
        /// <returns>The user colors</returns>
        private ushort[] GetUserColors()
        {
            if (this.UserGenerator == null || !this.Running)
            {
                return null;
            }

            var sceneMetaData = this.UserGenerator.GetUserPixels(0);

            int size = sceneMetaData.XRes * sceneMetaData.YRes;
            short[] úsersTemp = new short[size];
            Marshal.Copy(sceneMetaData.SceneMapPtr, úsersTemp, 0, úsersTemp.Length);
            ushort[] users = new ushort[size];
            Buffer.BlockCopy(úsersTemp, 0, users, 0, size * sceneMetaData.BytesPerPixel);

            return users;
        }
    }
}
