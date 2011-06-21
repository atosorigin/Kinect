using System;

namespace Kinect.Plugins.Common
{
    public class UserEventArgs : EventArgs
    {
        public UserEventArgs(int userID)
        {
            UserID = userID;
        }

        public int UserID { get; private set; }
    }
}