using Kinect.Core;
using Kinect.Core.Eventing;

namespace Kinect.Workshop
{
    public class MyFilter : Filter<IUserChangedEvent>
    {
        public override string Name
        {
            get { return "FilterPipe"; }
        }

        public override void Process(IUserChangedEvent evt)
        {
            bool continueProcess = false;
            //TODO: Workshop -> Part 3:
            //TODO: Workshop -> Filter hier de frames uit die niet nodig zijn

            //Als de data goed is en je wilt dat de filter door gaat naar de volgende stap
            if (continueProcess)
            {
                //Ga door naar de volgende stap van deze pipe
                base.Process(evt);
            }
            else
            {
                //Data is gefilterd.
                //Als er mensen geabonneerd zijn op het FilteredEvent
                //laat ze dan weten dat deze data gefilterd is
                OnFilteredEvent(new MyFilterEventArgs("Data is gefilterd"));
            }
        }
    }
}