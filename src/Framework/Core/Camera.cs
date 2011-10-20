using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Kinect.Common.ColorHelpers;
using Microsoft.Research.Kinect.Nui;

namespace Kinect.Core
{
    /// <summary>
    /// For getting the image from the Kinect camera
    /// </summary>
    public class Camera : INotifyPropertyChanged
    {
        private const int RED_IDX = 2;
        private const int GREEN_IDX = 1;
        private const int BLUE_IDX = 0;

        /// <summary>
        /// Colors for colored depthview
        /// </summary>
        private readonly Color[] _colors = {
                                               Colors.Red, Colors.Blue, Colors.ForestGreen, Colors.Yellow, Colors.Orange,
                                               Colors.Purple
                                           };

        private readonly byte[] _depthFrame32 = new byte[320*240*4];

        /// <summary>
        /// Frames per second
        /// </summary>
        private int _fps;

        /// <summary>
        /// Holds the framecount
        /// </summary>
        private int _frames;

        /// <summary>
        /// Holds last frames log time
        /// </summary>
        private int _lastFPSlog;

        /// <summary>
        /// Gets or sets The Kinect context
        /// </summary>
        public Runtime Context { get; set; }

        /// <summary>
        /// Gets or sets type of CameraView
        /// </summary>
        public CameraView ViewType { get; set; }

        /// <summary>
        /// Gets the current image of the Camera
        /// </summary>
        public BitmapSource View { get; private set; }

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
        /// Instance of imagegenerator
        /// </summary>
        //private ImageGenerator _image;
        /// <summary>
        /// Event for notifying if a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public event EventHandler<KinectCameraEventArgs> CameraUpdated;

        /// <summary>
        /// Initializes the specified usergenerator.
        /// </summary>
        internal void Initialize()
        {
            var generator = new ColorGenerator();
            for (int i = 0; i < _colors.Length; i++)
            {
                _colors[i] = generator.NextColor();
            }
            Context.DepthFrameReady += Context_DepthFrameReady;
            Context.VideoFrameReady += Context_VideoFrameReady;
        }

        private void Context_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            if (!ViewType.HasFlag(CameraView.Color))
            {
                return;
            }
            // 32-bit per pixel, RGBA image
            PlanarImage image = e.ImageFrame.Image;
            BitmapSource bitmap = BitmapSource.Create(
                image.Width, image.Height, 96, 96, PixelFormats.Bgr32, null, image.Bits, image.Width*image.BytesPerPixel);
            bitmap.Freeze();
            OnCameraUpdated(bitmap, CameraView.Color);
        }

        private void Context_DepthFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            if (!(ViewType.HasFlag(CameraView.Depth) || ViewType.HasFlag(CameraView.ColoredDepth)))
            {
                //We don't need the depth camera
                return;
            }
            //TODO: Depth view beter uitwerken
            //Aan de hand van de examples
            // 32-bit per pixel, RGBA image
            PlanarImage image = e.ImageFrame.Image;
            BitmapSource bitmap = BitmapSource.Create(image.Width, image.Height, 96, 96, PixelFormats.Gray16, null, image.Bits, image.Width * image.BytesPerPixel);
            bitmap.Freeze();
            //PlanarImage image = e.ImageFrame.Image;
            //byte[] convertedDepthFrame = ConvertDepthFrame(image.Bits, ViewType.HasFlag(CameraView.ColoredDepth));
            //BitmapSource bitmap = BitmapSource.Create(
            //    image.Width, image.Height, 96, 96, PixelFormats.Bgr32, null, convertedDepthFrame, image.Width*4);
            //bitmap.Freeze();
            OnCameraUpdated(bitmap, ViewType);
        }

        /// <summary>
        /// Sets the color of the user.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="color">The color.</param>
        internal void SetUserColor(int id, Color color)
        {
            _colors[id%_colors.Length] = color;
        }

        /// <summary>
        /// Gets the color of the user.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        internal Color GetUserColor(int id)
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
        /// Called when the correct camera image is updated
        /// </summary>
        protected virtual void OnCameraUpdated(BitmapSource source, CameraView view)
        {
            EventHandler<KinectCameraEventArgs> handler = CameraUpdated;
            if (handler != null)
            {
                handler(this, new KinectCameraEventArgs(source, view));
            }
        }

        ///// <summary>
        ///// Gets the depth image bytes.
        ///// </summary>
        ///// <param name="colorUsers">if set to <c>true</c> [color users].</param>
        ///// <param name="resolutionX">The resolution X.</param>
        ///// <param name="resolutionY">The resolution Y.</param>
        ///// <returns>The depthimage bytes </returns>
        //private byte[] GetDepthImageBytes(bool colorUsers, out int resolutionX, out int resolutionY)
        //{
        //    resolutionX = 0;
        //    resolutionY = 0;
        //    if (!this.Running || this.Depth == null || (colorUsers && this.UserGenerator == null))
        //    {
        //        return null;
        //    }

        //    ushort[] depths = this.GetDepths(out resolutionX, out resolutionY);
        //    if (depths == null)
        //    {
        //        return null;
        //    }

        //    const int BytesPerPixel = 3;

        //    ushort[] usermap = new ushort[1];
        //    ////Only get the information if the user needs to be coloured
        //    if (colorUsers)
        //    {
        //        usermap = this.GetUserColors();
        //    }
        //    //// convert the depths to a grayscale image
        //    byte[] bytes = new byte[depths.Length * BytesPerPixel];
        //    for (int depthIndex = 0; depthIndex < depths.Length; depthIndex++)
        //    {
        //        int pixelIndex = depthIndex * BytesPerPixel;
        //        ushort singleDepth = depths[depthIndex];
        //        ushort gray = (ushort)(singleDepth == 0 ? 0x00 : (0xFF - (singleDepth >> 4)));

        //        ushort user = 0;
        //        ////Only get the information if the user needs to be coloured
        //        if (colorUsers)
        //        {
        //            user = usermap[depthIndex];
        //        }

        //        if (user != 0)
        //        {
        //            Color labelColor = this._colors[user % this._colors.Length];
        //            bytes[pixelIndex] = (byte)(gray * (labelColor.B / 256.0));
        //            bytes[pixelIndex + 1] = (byte)(gray * (labelColor.G / 256.0));
        //            bytes[pixelIndex + 2] = (byte)(gray * (labelColor.R / 256.0));
        //        }
        //        else
        //        {
        //            bytes[pixelIndex] = (byte)gray;
        //            bytes[pixelIndex + 1] = (byte)gray;
        //            bytes[pixelIndex + 2] = (byte)gray;
        //        }
        //    }

        //    return bytes;
        //}

        // Converts a 16-bit grayscale depth frame which includes player indexes into a 32-bit frame
        // that displays different players in different colors
        private byte[] ConvertDepthFrame(IList<byte> depthFrame16, bool colorUsers)
        {
            for (int i16 = 0, i32 = 0; i16 < depthFrame16.Count && i32 < _depthFrame32.Length; i16 += 2, i32 += 4)
            {
                int player = depthFrame16[i16] & 0x07;
                int realDepth = (depthFrame16[i16 + 1] << 5) | (depthFrame16[i16] >> 3);
                // transform 13-bit depth information into an 8-bit intensity appropriate
                // for display (we disregard information in most significant bit)
                var intensity = (byte) (255 - (255*realDepth/0x0fff));

                _depthFrame32[i32 + RED_IDX] = 0;
                _depthFrame32[i32 + GREEN_IDX] = 0;
                _depthFrame32[i32 + BLUE_IDX] = 0;

                if (!colorUsers)
                {
                    //IF the users don't need to get specific colors, please continue
                    continue;
                }

                // choose different display colors based on player
                switch (player)
                {
                    case 0:
                        _depthFrame32[i32 + RED_IDX] = (byte) (intensity/2);
                        _depthFrame32[i32 + GREEN_IDX] = (byte) (intensity/2);
                        _depthFrame32[i32 + BLUE_IDX] = (byte) (intensity/2);
                        break;
                    case 1:
                        _depthFrame32[i32 + RED_IDX] = intensity;
                        break;
                    case 2:
                        _depthFrame32[i32 + GREEN_IDX] = intensity;
                        break;
                    case 3:
                        _depthFrame32[i32 + RED_IDX] = (byte) (intensity/4);
                        _depthFrame32[i32 + GREEN_IDX] = intensity;
                        _depthFrame32[i32 + BLUE_IDX] = intensity;
                        break;
                    case 4:
                        _depthFrame32[i32 + RED_IDX] = intensity;
                        _depthFrame32[i32 + GREEN_IDX] = intensity;
                        _depthFrame32[i32 + BLUE_IDX] = (byte) (intensity/4);
                        break;
                    case 5:
                        _depthFrame32[i32 + RED_IDX] = intensity;
                        _depthFrame32[i32 + GREEN_IDX] = (byte) (intensity/4);
                        _depthFrame32[i32 + BLUE_IDX] = intensity;
                        break;
                    case 6:
                        _depthFrame32[i32 + RED_IDX] = (byte) (intensity/2);
                        _depthFrame32[i32 + GREEN_IDX] = (byte) (intensity/2);
                        _depthFrame32[i32 + BLUE_IDX] = intensity;
                        break;
                    case 7:
                        _depthFrame32[i32 + RED_IDX] = (byte) (255 - intensity);
                        _depthFrame32[i32 + GREEN_IDX] = (byte) (255 - intensity);
                        _depthFrame32[i32 + BLUE_IDX] = (byte) (255 - intensity);
                        break;
                }
            }
            return _depthFrame32;
        }

        ///// <summary>
        ///// Gets the depth image.
        ///// </summary>
        ///// <returns>The depth image bitmapsource</returns>
        //private BitmapSource GetDepthImage()
        //{
        //    PlanarImage image = e.ImageFrame.Image;
        //    byte[] convertedDepthFrame = convertDepthFrame(image.Bits, false);

        //    return BitmapSource.Create(image.Width, image.Height, 96, 96, PixelFormats.Bgr32, BitmapPalettes.Gray256, convertedDepthFrame, image.Width * 4);
        //}

        ///// <summary>
        ///// Gets the colored depth with image.
        ///// </summary>
        ///// <returns>The colored depth image bitmapsource</returns>
        //private BitmapSource GetColoredDepthWithImage()
        //{
        //    PlanarImage image = e.ImageFrame.Image;
        //    byte[] convertedDepthFrame = convertDepthFrame(image.Bits, true);

        //    return BitmapSource.Create(image.Width, image.Height, 96, 96, PixelFormats.Bgr32, BitmapPalettes.WebPalette, convertedDepthFrame, image.Width * 4);
        //}

        ///// <summary>
        ///// Gets the color image.
        ///// </summary>
        ///// <returns>The colored image bitmapsource</returns>
        //private BitmapSource GetColorImage()
        //{
        //    ImageMetaData metadata = this._image.GetMetaData();
        //    const int BytesPerPixel = 3;
        //    int totalPixels = metadata.XRes * metadata.YRes;

        //    IntPtr imageMapPtr = this._image.GetImageMapPtr();

        //    return BitmapSource.Create(metadata.XRes, metadata.YRes, 96, 96, PixelFormats.Rgb24, null, imageMapPtr, totalPixels * BytesPerPixel, metadata.XRes * BytesPerPixel);
        //}

        ///// <summary>
        ///// Gets the depths.
        ///// </summary>
        ///// <param name="resolutionX">The resolution X.</param>
        ///// <param name="resolutionY">The resolution Y.</param>
        ///// <returns>The image depths</returns>
        //private ushort[] GetDepths(out int resolutionX, out int resolutionY)
        //{
        //    resolutionX = 0;
        //    resolutionY = 0;

        //    if (this.Depth == null || !this.Running)
        //    {
        //        return null;
        //    }

        //    // calculate the core metadata
        //    DepthMetaData metadata = this.Depth.GetMetaData();
        //    resolutionX = metadata.XRes;
        //    resolutionY = metadata.YRes;
        //    int totalDepths = metadata.XRes * metadata.YRes;

        //    // copy the depths
        //    // TODO: Is there a better way to marshal ushorts from an IntPtr?
        //    IntPtr depthMapPtr = this.Depth.GetDepthMapPtr();
        //    short[] depthsTemp = new short[totalDepths];
        //    Marshal.Copy(depthMapPtr, depthsTemp, 0, depthsTemp.Length);
        //    ushort[] depths = new ushort[totalDepths];
        //    Buffer.BlockCopy(depthsTemp, 0, depths, 0, totalDepths * metadata.BytesPerPixel);

        //    return depths;
        //}

        ///// <summary>
        ///// Gets the user colors.
        ///// </summary>
        ///// <returns>The user colors</returns>
        //private ushort[] GetUserColors()
        //{
        //    if (!this.Running)
        //    {
        //        return null;
        //    }

        //    var sceneMetaData = this.UserGenerator.GetUserPixels(0);

        //    int size = sceneMetaData.XRes * sceneMetaData.YRes;
        //    short[] úsersTemp = new short[size];
        //    Marshal.Copy(sceneMetaData.SceneMapPtr, úsersTemp, 0, úsersTemp.Length);
        //    ushort[] users = new ushort[size];
        //    Buffer.BlockCopy(úsersTemp, 0, users, 0, size * sceneMetaData.BytesPerPixel);

        //    return users;
        //}
    }
}