using System.Windows.Media.Media3D;

namespace Kinect.Core
{
    public interface IUserChangedEvent
    {
        /// <summary>
        /// Gets the id Kinect assigns to the user
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users head
        /// </summary>
        Point3D Head { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users neck
        /// </summary>
        Point3D ShoulderCenter { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users torso
        /// </summary>
        Point3D Spine { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left elbow
        /// </summary>
        Point3D ElbowLeft { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right elbow
        /// </summary>
        Point3D ElbowRight { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left shoulder
        /// </summary>
        Point3D ShoulderLeft { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right shoulder
        /// </summary>
        Point3D ShoulderRight { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left hand
        /// </summary>
        Point3D HandLeft { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right hand
        /// </summary>
        Point3D HandRight { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left hip
        /// </summary>
        Point3D HipLeft { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right hip
        /// </summary>
        Point3D HipRight { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left knee
        /// </summary>
        Point3D KneeLeft { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right knee
        /// </summary>
        Point3D KneeRight { get; }

        /// <summary>
        /// Gets the wrist left.
        /// </summary>
        Point3D WristLeft { get; }

        /// <summary>
        /// Gets the wrist right.
        /// </summary>
        Point3D WristRight { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left ankle
        /// </summary>
        Point3D AnkleLeft { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right ankle
        /// </summary>
        Point3D AnkleRight { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left foot
        /// </summary>
        Point3D FootLeft { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right foot
        /// </summary>
        Point3D FootRight { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users waist
        /// </summary>
        Point3D HipCenter { get; }
    }
}