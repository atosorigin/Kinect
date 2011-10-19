using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Kinect.ShowCase
{
    internal class ChangeResolution
    {
        private int _deviceModeNum = -1;
        private DEVMODE _originalResolution;


        internal ChangeResolution()
        {
            EnumDevices();

            if (_deviceModeNum == -1)
            {
                throw new Exception("Main display device not found");
            }

            _originalResolution = GetDevmode(_deviceModeNum, -1);
        }
        
        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayDevice
        {
            private readonly int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] 
            internal readonly string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] 
            private readonly string DeviceString;

            internal readonly int StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] 
            private readonly string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] 
            private readonly string DeviceKey;

            public DisplayDevice(int flags)
            {
                cb = 0;
                StateFlags = flags;
                DeviceName = new string((char)32, 32);
                DeviceString = new string((char)32, 128);
                DeviceID = new string((char)32, 128);
                DeviceKey = new string((char)32, 128);
                cb = Marshal.SizeOf(this);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] 
            private readonly string dmDeviceName;

            private readonly short dmSpecVersion;
            private readonly short dmDriverVersion;
            private readonly short dmSize;
            private readonly short dmDriverExtra;
            private readonly int dmFields;
            private readonly short dmOrientation;
            private readonly short dmPaperSize;
            private readonly short dmPaperLength;
            private readonly short dmPaperWidth;
            private readonly short dmScale;
            private readonly short dmCopies;
            private readonly short dmDefaultSource;
            private readonly short dmPrintQuality;
            //private readonly Point dmPosition;
            //private readonly short dmDisplayOrientation;
            //private readonly short dmDisplayFixedOutput;
            private readonly short dmColor;
            private readonly short dmDuplex;
            private readonly short dmYResolution;
            private readonly short dmTTOption;
            private readonly short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] 
            private readonly string dmFormName;

            private readonly short dmUnusedPadding;
            private readonly short dmBitsPerPel;
            public readonly int dmPelsWidth;
            public readonly int dmPelsHeight;
            private readonly int dmDisplayFlags;
            private readonly int dmDisplayFrequency;
        }

        private DEVMODE? EnumModes(int x, int y)
        {
            var devName = GetDeviceName(_deviceModeNum);
            var devMode = new DEVMODE();
            var modeNum = 0;
            bool result;
            do
            {
                result = EnumDisplaySettings(devName,
                    modeNum, ref devMode);

                if (result)
                {
                    if (devMode.dmPelsWidth == x &&
                       devMode.dmPelsHeight == y)
                    {
                       //devMode.dmBitsPerPel
                       //devMode.dmDisplayFrequency
                        return devMode;
                    }
                }
                modeNum++;
            } while (result);

            return null;
        }

        private void EnumDevices()
        {   //Find main display device
            var d = new DisplayDevice(0);

            var devNum = 0;
            bool result;
            do
            {
                result = EnumDisplayDevices(IntPtr.Zero,
                    devNum, ref d, 0);

                if (result)
                {
                    if ((d.StateFlags & 4) != 0)
                    {
                        _deviceModeNum = devNum;
                        break;
                    }
                }
                devNum++;
            } while (result);
        }

        public void ChangeScreenResolution(int x, int y)
        {
            var d = EnumModes(x, y);
            if (!d.HasValue)
            {
                throw new Exception("Resolution not found");
            }

            var dev = d.Value;

            if (dev.dmPelsWidth == _originalResolution.dmPelsWidth &&
                dev.dmPelsHeight == _originalResolution.dmPelsHeight)
            {
                //Resolution is the same, return
                return;
            }

            ChangeDisplaySettings(ref dev, 0);
        }

        public void ChangeScreenResolutionBackToOriginal()
        {
            ChangeDisplaySettings(ref _originalResolution, 0);
        }

        private string GetDeviceName(int devNum)
        {
            var d = new DisplayDevice(0);
            bool result = EnumDisplayDevices(IntPtr.Zero, devNum, ref d, 0);
            return (result ? d.DeviceName.Trim() : "#error#");
        }

        private DEVMODE GetDevmode(int devNum, int modeNum)
        { //populates DEVMODE for the specified device and mode
            var devMode = new DEVMODE();
            string devName = GetDeviceName(devNum);
            EnumDisplaySettings(devName, modeNum, ref devMode);
            return devMode;
        }

        [DllImport("User32.dll")]
        private static extern bool EnumDisplayDevices(
            IntPtr lpDevice, int iDevNum,
            ref DisplayDevice lpDisplayDevice, int dwFlags);

        [DllImport("User32.dll")]
        private static extern bool EnumDisplaySettings(
            string devName, int modeNum, ref DEVMODE devMode);

        [DllImport("user32.dll")]
        private static extern int ChangeDisplaySettings(
            ref DEVMODE devMode, int flags);
    }
}
