using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinect.Plugins.Common
{
    public class UserEventArgs : EventArgs
    {
        public int UserID { get; private set; }
        public UserEventArgs(int userID)
        {
            UserID = userID;
        }

    }
}
