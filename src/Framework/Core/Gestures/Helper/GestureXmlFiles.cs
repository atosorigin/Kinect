using System.IO;
using System.Reflection;

namespace Kinect.Core.Gestures.Helper
{
    public static class GestureXmlFiles
    {
        public static readonly string GesturesXmlFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + "/Configs/GestureConfiguration.xml";
    }
}