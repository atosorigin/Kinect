using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinect.Workshop
{
    class KinectForm_CodeSnippets
    {
        private void BtnStartClick(object sender, EventArgs e)
        {
            AddItemToListbox(lbMessages, "Button start hit: starting Kinect");
            _kinect = MyKinect.Instance;
            _kinect.SingleUserMode = true;
            _kinect.CameraMessage += KinectCameraMessage;
            //Abonneren op de overige events die voor jou belangrijk zijn
            _kinect.UserCreated += KinectUserCreated;
            _kinect.UserRemoved += KinectUserRemoved;

            _kinect.StartKinect();
            AddItemToListbox(lbMessages, "Kinect started");
        }

        private void _kinect_UserCreated(object sender, Core.Eventing.ProcessEventArgs<IUserChangedEvent> e)
        {
            //TODO: Workshop ->Update alle labels, door gebruik te maken van de UpdateLabel functie, met de gegevens uit de event args.
            //Labels beginnen standaard met lbl, voorbeeld: lblLeftHand
            UpdateLabel(lblHead, e.Event.Head.GetDebugString());
            UpdateLabel(lblNeck, e.Event.Neck.GetDebugString());
            UpdateLabel(lblLeftShoulder, e.Event.LeftShoulder.GetDebugString());
            UpdateLabel(lblRightShoulder, e.Event.RightShoulder.GetDebugString());
            UpdateLabel(lblLeftElbow, e.Event.LeftElbow.GetDebugString());
            UpdateLabel(lblRightElbow, e.Event.RightElbow.GetDebugString());
            UpdateLabel(lblLeftHand, e.Event.LeftHand.GetDebugString());
            UpdateLabel(lblTorso, e.Event.Torso.GetDebugString());
            UpdateLabel(lblRightHand, e.Event.RightHand.GetDebugString());
            UpdateLabel(lblLeftHip, e.Event.LeftHip.GetDebugString());
            UpdateLabel(lblRightHip, e.Event.RightHip.GetDebugString());
            UpdateLabel(lblLeftKnee, e.Event.LeftKnee.GetDebugString());
            UpdateLabel(lblRightKnee, e.Event.RightKnee.GetDebugString());
            UpdateLabel(lblLeftFoot, e.Event.LeftFoot.GetDebugString());
            UpdateLabel(lblRightFoot, e.Event.RightFoot.GetDebugString());
        }

        private void BtnStartClick(object sender, EventArgs e)
        {
            AddItemToListbox(lbMessages, "Button start hit: starting Kinect");
            _kinect = MyKinect.Instance;
            _kinect.SingleUserMode = true;
            _kinect.CameraMessage += KinectCameraMessage;
            //TODO: Workshop -> Abonneren op de overige events die voor jou belangrijk zijn
            //TODO: Workshop -> De volgende functies moet je in ieder geval koppelen (KinectUserCreated en  KinectUserRemoved)
            //TODO: Workshop -> Naar CodeSnippets
            _kinect.UserCreated += KinectUserCreated;
            _kinect.UserRemoved += KinectUserRemoved;

            //En start de Kinect op
            _kinect.StartKinect();
            AddItemToListbox(lbMessages, "Kinect started");
        }

        private void KinectUserCreated(object sender, KinectUserEventArgs e)
        {
            AddItemToListbox(lbMessages, string.Format("Kinect created a new user with id: {0}", e.User.ID));
            //TODO: Workshop -> Vraag de gebruiker op bij Kinect met behulp van de eventargs
            //TODO: Workshop -> Vergeet niet te controleren of de gebruiker niet null is (ivm multi threading)
            //TODO: Workshop -> En Abonneren je daarna het updated event (UserUpdated)


            //TODO: Workshop -> Vul de MyFilter class en MyGesture Class
            //TODO: Workshop -> Koppel ze dan aan elkaar en aan de gebruiker zodat een pipe ontstaat
            //TODO: Workshop -> zie http://atosorig.in/aonlkinect voor documentatie
            //TODO: Workshop -> Abonneren je dan op het MyGestureDetected event met behulp van de GestureMyGestureDetected functie
            //TODO: Workshop -> Naar CodeSnippets
            var user = _kinect.GetUser(e.User.ID);
            //Controleer altijd eerst of de gebruiker nog wel bestaat
            if (user != null)
            {
                //En Abonneren je hier op events
                //Voeg hier standaard gestures toe
                //Of voeg hier je eigen pipeline toe
                user.Updated += UserUpdated;

                //Maak je eigen filter en gesture aan en koppel deze aan de user
                var filter = new MyFilter();
                var gesture = new MyGesture();
                //Koppel je gesture aan je filter
                filter.AttachPipeline(gesture);
                //En koppel dan de filter aan de user
                user.AttachPipeline(filter);
                //Nu zal de data van de user naar de filter gaan 
                //Als hij door de filter komt, zal de data naar de gesture gaan
                gesture.MyGestureDetected += GestureMyGestureDetected;
            }
        }

        private void KinectUserRemoved(object sender, KinectUserEventArgs e)
        {
            //Er wordt een melding aan de bovenste listbox
            AddItemToListbox(lbMessages, string.Format("Kinect removed user with id: {0}", e.User.ID));
        }

        private void KinectCameraMessage(object sender, KinectMessageEventArgs e)
        {
            //Alle camera messages worden weergegeven in de onderste listbox
            AddItemToListbox(lbCameraMessages, e.Message);
        }

        private void GestureMyGestureDetected(object sender, Core.Gestures.GestureEventArgs e)
        {
            AddItemToListbox(lbMessages, string.Format("My gesture detected for user: {0}", e.UserID));
        }

    }

    class MyFilter_CodeSnippets
    {
        //TODO: Workshop -> Naar CodeSnippets
        private int _count;
        private const int FilteredFrames = 6;

        public override void Process(IUserChangedEvent evt)
        {
            bool gestureDetected = false;
            //TODO: Workshop -> Controleer hier of de data voldoet aan je gesture criteria

            //TODO: Workshop -> Naar CodeSnippets
            //We gaan controleren of de X van de linker hand dicht bij de X van de rechterhand is
            if (Math.Abs(evt.LeftHand.X - evt.RightHand.X) < 10)
            {
                //Als het verschil van de X van de linker hand en de X van de rechterhand onder de 10 is
                //Dan zijn ze dicht bij elkaar in de buurt
                gestureDetected = true;
            }

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
            var handler = MyGestureDetected;
            if (handler != null)
            {
                handler(this, new GestureEventArgs(userid));
            }
        }
    }

    class MyGesture_CodeSnippets
    {
        public event EventHandler<GestureEventArgs> MyGestureDetected;

        protected override string GestureName
        {
            get { return "MyGesture"; }
        }

        public override void Process(IUserChangedEvent evt)
        {
            bool gestureDetected = false;
            //TODO: Workshop -> Controleer hier of de data voldoet aan je gesture criteria

            //TODO: Workshop -> Naar CodeSnippets
            //We gaan controleren of de X van de linker hand dicht bij de X van de rechterhand is
            if (Math.Abs(evt.LeftHand.X - evt.RightHand.X) < 10)
            {
                //Als het verschil van de X van de linker hand en de X van de rechterhand onder de 10 is
                //Dan zijn ze dicht bij elkaar in de buurt
                gestureDetected = true;
            }

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
            var handler = MyGestureDetected;
            if (handler != null)
            {
                handler(this, new GestureEventArgs(userid));
            }
        }
    }
}
