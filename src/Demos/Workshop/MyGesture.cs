using System;
using Kinect.Core;
using Kinect.Core.Gestures;

namespace Kinect.Workshop
{
    public class MyGesture : GestureBase
    {
        public event EventHandler<GestureEventArgs> MyGestureDetected;

        protected override string GestureName
        {
            get { return "MyGesture"; }
        }

        public override void Process(IUserChangedEvent evt)
        {
            var gestureDetected = false;
            //TODO: Workshop -> Part 3:
            //TODO: Workshop -> Controleer hier of de data voldoet aan je gesture criteria

            //Als je gesture goed is, laat dan weten dat je gesture is afgegaan
            if (gestureDetected)
            {
                OnMyGestureDetected(evt.ID);
            }
        }

        protected virtual void OnMyGestureDetected(uint userid)
        {
            //Dit is nodig ivm multi threading.
            //Het kan zijn dat jij je abonnement opheft en op hetzelfde moment
            //De kinect thread het event probeert af te vuren
            //Dit zorgt er dan voor dat het .NET framework er dan geen problemen mee heeft
            var handler = MyGestureDetected;
            if (handler != null)
            {
                handler(this, new GestureEventArgs(userid));
            }
        }
    }
}
