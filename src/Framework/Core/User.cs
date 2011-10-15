using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Kinect.Core.Eventing;

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
            Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="evt">The IUserChangedEvent data</param>
        public User(IUserChangedEvent evt)
        {
            Id = evt.Id;
            Head = evt.Head;
            ShoulderCenter = evt.ShoulderCenter;
            Spine = evt.Spine;
            ElbowLeft = evt.ElbowLeft;
            ElbowRight = evt.ElbowRight;
            ShoulderLeft = evt.ShoulderLeft;
            ShoulderRight = evt.ShoulderRight;
            HandLeft = evt.HandLeft;
            HandRight = evt.HandRight;
            HipLeft = evt.HipLeft;
            HipRight = evt.HipRight;
            KneeLeft = evt.KneeLeft;
            KneeRight = evt.KneeRight;
            AnkleLeft = evt.AnkleLeft;
            AnkleRight = evt.AnkleRight;
            FootLeft = evt.FootLeft;
            FootRight = evt.FootRight;
            HipCenter = evt.HipCenter;
        }

        /// <summary>
        /// Gets Color assigned to user,
        /// meant for drawing the user on the screen and for the colored depthimage
        /// </summary>
        public Color Color { get; internal set; }

        #region IUserChangedEvent Members

        /// <summary>
        /// Gets the id Kinect assigns to the user
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users head
        /// </summary>
        public Point3D Head { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users neck
        /// </summary>
        public Point3D ShoulderCenter { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users torso
        /// </summary>
        public Point3D Spine { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left elbow
        /// </summary>
        public Point3D ElbowLeft { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right elbow
        /// </summary>
        public Point3D ElbowRight { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left shoulder
        /// </summary>
        public Point3D ShoulderLeft { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right shoulder
        /// </summary>
        public Point3D ShoulderRight { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left hand
        /// </summary>
        public Point3D HandLeft { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right hand
        /// </summary>
        public Point3D HandRight { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left hip
        /// </summary>
        public Point3D HipLeft { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right hip
        /// </summary>
        public Point3D HipRight { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left knee
        /// </summary>
        public Point3D KneeLeft { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right knee
        /// </summary>
        public Point3D KneeRight { get; internal set; }

        /// <summary>
        /// Gets the wrist left.
        /// </summary>
        public Point3D WristLeft { get; internal set; }

        /// <summary>
        /// Gets the wrist right.
        /// </summary>
        public Point3D WristRight { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left ankle
        /// </summary>
        public Point3D AnkleLeft { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right ankle
        /// </summary>
        public Point3D AnkleRight { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left foot
        /// </summary>
        public Point3D FootLeft { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right foot
        /// </summary>
        public Point3D FootRight { get; internal set; }

        /// <summary>
        /// Gets three dimensional coordinate of the users waist
        /// </summary>
        public Point3D HipCenter { get; internal set; }

        #endregion

        #region IUserUpdated Members

        /// <summary>
        /// Event will notify if user is updated
        /// </summary>
        public event EventHandler<ProcessEventArgs<IUserChangedEvent>> Updated;

        #endregion

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
            EventHandler<ProcessEventArgs<IUserChangedEvent>> handler = Updated;
            if (handler != null)
            {
                handler(this, new ProcessEventArgs<IUserChangedEvent>(this));
            }
        }
    }
}