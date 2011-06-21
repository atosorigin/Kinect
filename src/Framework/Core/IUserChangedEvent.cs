using System.Windows.Media.Media3D;

namespace Kinect.Core
{
    public interface IUserChangedEvent
    {
        /// <summary>
        /// Gets the id Kinect assigns to the user
        /// </summary>
        int ID { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users head
        /// </summary>
        Point3D Head { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users neck
        /// </summary>
        Point3D Neck { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users torso
        /// </summary>
        Point3D Torso { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left elbow
        /// </summary>
        Point3D LeftElbow { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right elbow
        /// </summary>
        Point3D RightElbow { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left shoulder
        /// </summary>
        Point3D LeftShoulder { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right shoulder
        /// </summary>
        Point3D RightShoulder { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left hand
        /// </summary>
        Point3D LeftHand { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right hand
        /// </summary>
        Point3D RightHand { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left fingertip
        /// </summary>
        Point3D LeftFingertip { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right fingertip
        /// </summary>
        Point3D RightFingertip { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left hip
        /// </summary>
        Point3D LeftHip { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right hip
        /// </summary>
        Point3D RightHip { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left knee
        /// </summary>
        Point3D LeftKnee { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right knee
        /// </summary>
        Point3D RightKnee { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left ankle
        /// </summary>
        Point3D LeftAnkle { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right ankle
        /// </summary>
        Point3D RightAnkle { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users left foot
        /// </summary>
        Point3D LeftFoot { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users right foot
        /// </summary>
        Point3D RightFoot { get; }

        /// <summary>
        /// Gets three dimensional coordinate of the users waist
        /// </summary>
        Point3D Waist { get; }
    }
}