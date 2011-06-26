using Kinect.Core;
using Kinect.Core.Eventing;

namespace Kinect.Workshop.Gestures
{
    public class MyFilterEventArgs : FilterEventArgs
    {
        private readonly string _name;

        public MyFilterEventArgs(Filter<IUserChangedEvent> sender, string message)
        {
            Message = message;
            _name = sender.Name;
        }

        public string Message { get; private set; }

        /// <summary>
        /// Name of the filter
        /// </summary>
        public override string Name
        {
            get { return _name; }
        }
    }
}