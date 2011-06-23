using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Kinect.MouseControl
{

    /// <summary>
    /// Mouse buttons that can be pressed
    /// </summary>
    public enum MouseButton
    {
        Left = 0x2,
        Right = 0x8,
        Middle = 0x20
    }

    /// <summary>
    /// Operations that simulate mouse events
    /// </summary>
    public static class MouseSimulator
    {

        #region Windows API Code

        [DllImport("user32.dll")]
        static extern int ShowCursor(bool show);

        [DllImport("user32.dll")]
        static extern void mouse_event(int flags, int dX, int dY, int buttons, int extraInfo);

        [DllImportAttribute("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool SetCursorPos(int X, int Y);

        const int MOUSEEVENTF_MOVE = 0x1;
        const int MOUSEEVENTF_LEFTDOWN = 0x2;
        const int MOUSEEVENTF_LEFTUP = 0x4;
        const int MOUSEEVENTF_RIGHTDOWN = 0x8;
        const int MOUSEEVENTF_RIGHTUP = 0x10;
        const int MOUSEEVENTF_MIDDLEDOWN = 0x20;
        const int MOUSEEVENTF_MIDDLEUP = 0x40;
        const int MOUSEEVENTF_WHEEL = 0x800;
        const int MOUSEEVENTF_ABSOLUTE = 0x8000; 

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a structure that represents both X and Y mouse coordinates
        /// </summary>
        public static Point Position
        {
            get
            {
                return new Point(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);
            }
            set
            {
                System.Windows.Forms.Cursor.Position = new System.Drawing.Point(Convert.ToInt32(value.X),
                                                                                Convert.ToInt32(value.Y));
            }
        }

        /// <summary>
        /// Gets or sets only the mouse's x coordinate
        /// </summary>
        public static int X
        {
            get { return System.Windows.Forms.Cursor.Position.X; }
            set
            {
                System.Windows.Forms.Cursor.Position = new System.Drawing.Point(value, Y);
            }
        }

        /// <summary>
        /// Gets or sets only the mouse's y coordinate
        /// </summary>
        public static int Y
        {
            get
            {
                return System.Windows.Forms.Cursor.Position.Y;
            }
            set
            {
                System.Windows.Forms.Cursor.Position = new System.Drawing.Point(X, value);
            }
        } 

        #endregion

        #region Methods

        /// <summary>
        /// Press a mouse button down
        /// </summary>
        /// <param name="button"></param>
        public static void MouseDown(MouseButton button)
        {
            mouse_event(((int)button), 0, 0, 0, 0);
        }

        /// <summary>
        /// Press a mouse button down
        /// </summary>
        /// <param name="button"></param>
        public static void MouseDown(System.Windows.Input.MouseButton button)
        {
            switch (button)
            {
                case System.Windows.Input.MouseButton.Left:
                    MouseDown(MouseButton.Left);
                    break;
                case System.Windows.Input.MouseButton.Middle:
                    MouseDown(MouseButton.Middle);
                    break;
                case System.Windows.Input.MouseButton.Right:
                    MouseDown(MouseButton.Right);
                    break;
            }
        }

        /// <summary>
        /// Let a mouse button up
        /// </summary>
        /// <param name="button"></param>
        public static void MouseUp(MouseButton button)
        {
            mouse_event(((int)button) * 2, 0, 0, 0, 0);
        }

        /// <summary>
        /// Let a mouse button up
        /// </summary>
        /// <param name="button"></param>
        public static void MouseUp(System.Windows.Input.MouseButton button)
        {
            switch (button)
            {
                case System.Windows.Input.MouseButton.Left:
                    MouseUp(MouseButton.Left);
                    break;
                case System.Windows.Input.MouseButton.Middle:
                    MouseUp(MouseButton.Middle);
                    break;
                case System.Windows.Input.MouseButton.Right:
                    MouseUp(MouseButton.Right);
                    break;
            }
        }

        /// <summary>
        /// Click a mouse button (down then up)
        /// </summary>
        /// <param name="button"></param>
        public static void Click(System.Windows.Input.MouseButton button)
        {
            MouseDown(button);
            MouseUp(button);
        }

        /// <summary>
        /// Click a mouse button (down then up)
        /// </summary>
        /// <param name="button"></param>
        //public static void Click(System.Windows.Input.MouseButton button)
        //{
        //    switch (button)
        //    {
        //        case System.Windows.Input.MouseButton.Left:
        //            Click(System.Windows.Input.MouseButton.Left);
        //            break;
        //        case System.Windows.Input.MouseButton.Middle:
        //            Click(System.Windows.Input.MouseButton.Middle);
        //            break;
        //        case System.Windows.Input.MouseButton.Right:
        //            Click(System.Windows.Input.MouseButton.Right);
        //            break;
        //    }
        //}

        /// <summary>
        /// Double click a mouse button (down then up twice)
        /// </summary>
        /// <param name="button"></param>
        public static void DoubleClick(System.Windows.Input.MouseButton button)
        {
            Click(button);
            Click(button);
        }

        /// <summary>
        /// Double click a mouse button (down then up twice)
        /// </summary>
        /// <param name="button"></param>
        //public static void DoubleClick(MouseButtons button)
        //{
        //    switch (button)
        //    {
        //        case MouseButtons.Left:
        //            DoubleClick(MouseButton.Left);
        //            break;
        //        case MouseButtons.Middle:
        //            DoubleClick(MouseButton.Middle);
        //            break;
        //        case MouseButtons.Right:
        //            DoubleClick(MouseButton.Right);
        //            break;
        //    }
        //}

        public static void ChangeMouseCursor()
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.IBeam;
        }

        /// <summary>
        /// Roll the mouse wheel. Delta of 120 wheels up once normally, -120 wheels down once normally
        /// </summary>
        /// <param name="delta"></param>
        public static void MouseWheel(int delta)
        {

            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, delta, 0);

        }

        /// <summary>
        /// Show a hidden current on currently application
        /// </summary>
        public static void Show()
        {
            ShowCursor(true);
        }

        /// <summary>
        /// Hide mouse cursor only on current application's forms
        /// </summary>
        public static void Hide()
        {
            ShowCursor(false);
        } 

        #endregion

    }

}
