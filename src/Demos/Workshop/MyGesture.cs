using System;
using Kinect.Core;
using Kinect.Core.Gestures;

namespace Kinect.Workshop.Winforms
{
    public class MyGesture : GestureBase
    {
        protected override string GestureName
        {
            get { return "MyGesture"; }
        }

        public event EventHandler<GestureEventArgs> MyGestureDetected;

        public override void Process(IUserChangedEvent evt)
        {
            bool gestureDetected = false;
            //TODO: Workshop -> Part 3:
            //TODO: Workshop -> Controleer hier of de data voldoet aan je gesture criteria

            //Als je gesture goed is, laat dan weten dat je gesture is afgegaan
            if (gestureDetected)
            {
                OnMyGestureDetected(evt.ID);
            }
        }

        protected virtual void OnMyGestureDetected(int userid)
        {
            //Dit is nodig ivm multi threading.
            //Het kan zijn dat jij je abonnement opheft en op hetzelfde moment
            //De kinect thread het event probeert af te vuren
            //Dit zorgt er dan voor dat het .NET framework er dan geen problemen mee heeft
            EventHandler<GestureEventArgs> handler = MyGestureDetected;
            if (handler != null)
            {
                handler(this, new GestureEventArgs(userid));
            }
        }
    }
}