using System;

namespace Kinect.SpinToWin.Controls
{
    public class WinnerEventArgs : EventArgs
    {
        public string Winner { get; private set; }
        public WinnerEventArgs(string winner)
        {
            Winner = winner;
        }
    }
}