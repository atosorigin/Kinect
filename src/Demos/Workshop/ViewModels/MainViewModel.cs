using System;
using Kinect.Core;
using Kinect.Workshop.Gestures;

namespace Kinect.Workshop.ViewModels
{
    public class MainViewModel : WorkshopViewModelBase
    {
        private User _kinectUser;

        public override void StartKinect()
        {
            //TODO: Workshop -> Step 1: Start Kinect by using base property Kinect
        }

        public override void StopKinect()
        {
            //TODO: Workshop -> Step 2: Stop Kinect by using base property Kinect
        }

        public override void SubscribeToKinectEvents()
        {
            Kinect.CameraMessage += base.KinectCameraMessage;
            Kinect.CameraDataUpdated += base.KinectCameraDataUpdated;
            //TODO: Workshop -> Step 3:
            //TODO: Workshop -> Subscribe to events and add Messages to the Messages property in the eventhandlers
        }

        public override void UnSubscribeToKinectEvents()
        {
            Kinect.CameraMessage -= base.KinectCameraMessage;
            Kinect.CameraDataUpdated -= base.KinectCameraDataUpdated;
            //TODO: Workshop -> Step 4: UnSubscribe the events you subscribed to in step 1
        }

        private void Kinect_UserCreated(object sender, KinectUserEventArgs e)
        {
            UpdateUserInterface(() => Messages.Add(string.Format("Kinect created a new user with id: {0}", e.User.ID)));
            //TODO: Workshop -> Step 5: instantiate _kinectUser, use the eventArgs and Kinect

            SubscribeToUserUpdatedEvent();
            AttachGesture();
        }

        public override void SubscribeToUserUpdatedEvent()
        {
            //TODO: Workshop -> Step 6: Subscribe to UserUpdated event
            //TODO: Workshop -> (In the eventhandler) Set the values of the properties
            //Head, Neck, LeftShoulder, RightShoulder, Torso, LeftElbow, RightElbow, LeftHand, RightHand, LeftHip, RightHip, LeftKnee, RightKnee, LeftFoot, RightFoot
            //Tip: use base.
        }

        public override void TrackRightHand()
        {
            //TODO: Workshop -> Step 7: Set the property PointerPosition

        }

        public override void AttachGesture()
        {
            //TODO: Workshop -> Step 8: Implement MyFilter and MyGesture (See Gestures Folder in this Workshop project)
            //TODO: Workshop -> Step 9: Create instance of MyFilter and MyGesture
            //TODO: Workshop -> Step 10: Attach the Filter and Pipeline to the _kinectUser
            //TODO: Workshop -> Step 11: Subscribe to the filter and gesture events and Set the Messages Property
            //TODO: Workshop -> Step 12: When the event is processed (ocurred) set the PointerColor property each time to another color
        }
    }
}