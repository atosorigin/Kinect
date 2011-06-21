using System;
using System.Windows.Media;
using Kinect.Core.Eventing;
using System.Windows.Media.Media3D;

namespace Kinect.Core
{
    /// <summary>
    /// Represents the Kinect User
    /// </summary>
    public class User : EventPublisher<IUserChangedEvent>, IUserChangedEvent, IUserUpdated
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="id">userId assigned by Kinect</param>
        public User(int id)
        {
            this.ID = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="evt">The IUserChangedEvent data</param>
        public User(IUserChangedEvent evt)
        {
            this.ID = evt.ID;
            this.Head = evt.Head;
            this.Neck = evt.Neck;
            this.Torso = evt.Torso;
            this.LeftElbow = evt.LeftElbow;
            this.RightElbow = evt.RightElbow;
            this.LeftShoulder = evt.LeftShoulder;
            this.RightShoulder = evt.RightShoulder;
            this.LeftHand = evt.LeftHand;
            this.RightHand = evt.RightHand;
            this.LeftFingertip = evt.LeftFingertip;
            this.RightFingertip = evt.RightFingertip;
            this.LeftHip = evt.LeftHip;
            this.RightHip = evt.RightHip;
            this.LeftKnee = evt.LeftKnee;
            this.RightKnee = evt.RightKnee;
            this.LeftAnkle = evt.LeftAnkle;
            this.RightAnkle = evt.RightAnkle;
            this.LeftFoot = evt.LeftFoot;
            this.RightFoot = evt.RightFoot;
            this.Waist = evt.Waist;
        }

        /// <summary>
        /// Event will notify if user is updated
        /// </summary>
        public event EventHandler<ProcessEventArgs<IUserChangedEvent>> Updated;
        
        /// <summary>
        /// Gets the id Kinect assigns to the user
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users head
        /// </summary>
        public Point3D Head { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users neck
        /// </summary>
        public Point3D Neck { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users torso
        /// </summary>
        public Point3D Torso { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left elbow
        /// </summary>
        public Point3D LeftElbow { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right elbow
        /// </summary>
        public Point3D RightElbow { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left shoulder
        /// </summary>
        public Point3D LeftShoulder { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right shoulder
        /// </summary>
        public Point3D RightShoulder { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left hand
        /// </summary>
        public Point3D LeftHand { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right hand
        /// </summary>
        public Point3D RightHand { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left fingertip
        /// </summary>
        public Point3D LeftFingertip { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right fingertip
        /// </summary>
        public Point3D RightFingertip { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left hip
        /// </summary>
        public Point3D LeftHip { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right hip
        /// </summary>
        public Point3D RightHip { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left knee
        /// </summary>
        public Point3D LeftKnee { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right knee
        /// </summary>
        public Point3D RightKnee { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left ankle
        /// </summary>
        public Point3D LeftAnkle { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right ankle
        /// </summary>
        public Point3D RightAnkle { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left foot
        /// </summary>
        public Point3D LeftFoot { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right foot
        /// </summary>
        public Point3D RightFoot { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users waist
        /// </summary>
        public Point3D Waist { get; internal set; }

        /// <summary>
        /// Gets Color assigned to user,
        /// meant for drawing the user on the screen and for the colored depthimage
        /// </summary>
        public Color Color { get; internal set; }

        /// <summary>
        /// Update coordinates and publish event
        /// </summary>
        internal void Update()
        {
            OnUpdated();
            PublishEvent(this);
        }

        /// <summary>
        /// Triggers the Updated event
        /// </summary>
        protected virtual void OnUpdated()
        {
            var handler = this.Updated;
            if (handler != null)
            {
                handler(this, new ProcessEventArgs<IUserChangedEvent>(this));
            }
        }
    }
}
