using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinect.Plugins.Common
{
    public class UserEventArgs : EventArgs
    {
        public uint UserID { get; private set;}
        public UserEventArgs(uint userID)
        {
            UserID = userID;
        }

    }
}
