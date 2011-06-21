using System.Collections.Generic;
using System.Linq;
using Kinect.Core.Gestures.Helper;

namespace Kinect.Core.Gestures.Model
{
    public static class Semaphores
    {
        internal static List<Semaphore> SemafoorGestures =
            GestureXmlReader.ReadNodesToList<Semaphore>(GestureXmlFiles.GesturesXmlFile);

        public static Semaphore GetSemaphore(char character)
        {
            return SemafoorGestures.FirstOrDefault(s => s.Char == character);
        }

        public static Semaphore GetSemaphore(string name)
        {
            return SemafoorGestures.FirstOrDefault(s => s.Name == name);
        }
    }
}