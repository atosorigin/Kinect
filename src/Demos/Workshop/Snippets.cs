//namespace Kinect.Workshop
//{
//    public class Snippets
//    {
//        //TODO: Workshop -> Step 1: Start Kinect by using base property Kinect
//        Kinect.StartKinect();

//        //TODO: Workshop -> Step 2: Stop Kinect by using base property Kinect
//        Kinect.StopKinect();

//        //TODO: Workshop -> Step 3: Subscribe to events and add Messages to the Messages property in the eventhandlers
//        Kinect.KinectStarted += new EventHandler<KinectEventArgs>(Kinect_KinectStarted);
//        Kinect.KinectStopped += new EventHandler<KinectEventArgs>(Kinect_KinectStopped);
//        Kinect.UserCreated += new EventHandler<KinectUserEventArgs>(Kinect_UserCreated);
//        Kinect.UserRemoved += new EventHandler<KinectUserEventArgs>(Kinect_UserRemoved);

//        void Kinect_UserRemoved(object sender, KinectUserEventArgs e)
//        {
//            UpdateUserInterface(()=> Messages.Add(string.Format("User {0} removed", e.User.ID)));
//        }

//        void Kinect_KinectStarted(object sender, KinectEventArgs e)
//        {
//            UpdateUserInterface(() => Messages.Add("Kinect Started"));
//        }

//        void Kinect_KinectStopped(object sender, KinectEventArgs e)
//        {
//            UpdateUserInterface(() => Messages.Add("Kinect Stopped"));
//        }

//        //TODO: Workshop -> Step 4: UnSubscribe the events you subscribed to in step 3
//        Kinect.KinectStarted -= new EventHandler<KinectEventArgs>(Kinect_KinectStarted);
//        Kinect.KinectStopped -= new EventHandler<KinectEventArgs>(Kinect_KinectStopped);
//        Kinect.UserCreated -= new EventHandler<KinectUserEventArgs>(Kinect_UserCreated);
//        Kinect.UserRemoved -= new EventHandler<KinectUserEventArgs>(Kinect_UserRemoved);

//        //TODO: Workshop -> Step 5: instantiate the field _kinectUser, use the eventArgs and Kinect
//        _kinectUser = Kinect.GetUser(e.User.ID);

//        //TODO: Workshop -> Step 6: Subscribe to UserUpdated event
//        _kinectUser.Updated += KinectUserUpdated;

//        void KinectUserUpdated(object sender, Core.Eventing.ProcessEventArgs<IUserChangedEvent> e)
//        {
//            UpdateUserInterface(() => {
//                Head = e.Event.Head.GetDebugString();
//                Neck = e.Event.Neck.GetDebugString();
//                LeftShoulder = e.Event.LeftShoulder.GetDebugString();
//                RightShoulder = e.Event.RightShoulder.GetDebugString();
//                Torso = e.Event.Torso.GetDebugString();
//                LeftElbow = e.Event.LeftElbow.GetDebugString();
//                RightElbow = e.Event.RightElbow.GetDebugString();
//                LeftHand = e.Event.LeftHand.GetDebugString();
//                RightHand = e.Event.RightHand.GetDebugString();
//                LeftHip = e.Event.LeftHip.GetDebugString();
//                RightHip = e.Event.RightHip.GetDebugString();
//                LeftKnee = e.Event.LeftKnee.GetDebugString();
//                RightKnee = e.Event.RightKnee.GetDebugString();
//                LeftFoot = e.Event.LeftFoot.GetDebugString();
//                RightFoot = e.Event.RightFoot.GetDebugString();
//            });
//        }

//        //TODO: Workshop -> Step 7: Set the property PointerPosition and call this method in the _kinectUser_Updated eventHandler
//        var screenCoordinate = rightHandCoordinate.ToScreenPosition(new Size(640, 480), new Size(800, 650));
//        UpdateUserInterface(() => PointerPosition = new Point(screenCoordinate.X, screenCoordinate.Y));

//        //In _kinectUser_Updated
//        TrackRightHand(e.Event.RightHand);

//        //TODO: Workshop -> Step 8: Implement MyFilter and MyGesture (See Gestures Folder in this Workshop project)
//        //MyFilter filtering code
//        _framesCount++;
//        if (_framesCount >= 30)
//        {
//            filtered = false;
//            _framesCount = 0;
//        }

//        //MyGesture detection code
//        if (Math.Abs(evt.LeftHand.X - evt.RightHand.X) < 40 &&
//        Math.Abs(evt.LeftHand.Y - evt.RightHand.Y) < 40 &&
//        Math.Abs(evt.LeftHand.Z - evt.RightHand.Z) < 200)
//        handsAreTogether = true;

//        //TODO: Workshop -> Step 9: Create instance of MyFilter and MyGesture
//        var filter = new MyFilter();
//        var gesture = new MyGesture();

//        //TODO: Workshop -> Step 10: Attach the Filter and Pipeline to the _kinectUser
//        _kinectUser.AttachPipeline(filter);
//        filter.AttachPipeline(gesture);

//        //TODO: Workshop -> Step 11: Subscribe to the filter and gesture events and Set the Messages Property
//          filter.Filtered += FilterFiltered;
//          gesture.ProcessedEvent += GestureProcessedEvent;

//          private void FilterFiltered(object sender, FilterEventArgs e)
//          {
//              //UpdateUserInterface(() => Messages.Add("Frame filtered"));
//          }

//          void GestureProcessedEvent(object sender, ProcessEventArgs<IUserChangedEvent> e)
//          {
//              //UpdateUserInterface(() => Messages.Add("Gesture processed"));
//          }

            //TODO: Workshop -> Step 12: When the event has occured set the PointerColor property each time to another color
//          gesture.GestureDetected += new EventHandler(GestureGestureDetected);

//          void GestureGestureDetected(object sender, EventArgs e)
//          {
//              UpdateUserInterface(() => this.PointerColor = new SolidColorBrush(GetNewColor()));
//              UpdateUserInterface(() => Messages.Add("Hands are together"));
//          }

//    }
//}