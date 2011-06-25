using Kinect.Core.Eventing;

namespace Kinect.Workshop
{
    public class MyFilterEventArgs : FilterEventArgs
    {
        public MyFilterEventArgs(string message)
        {
            //TODO: Deze kun je uitbreiden met wat extra data indien nodig
            Message = message;
        }

        public string Message { get; private set; }

        public override string Name
        {
            get { return "MyFilterEventArgs"; }
        }
    }
}