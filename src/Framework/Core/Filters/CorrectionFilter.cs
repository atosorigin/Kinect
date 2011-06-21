using System.Windows.Media.Media3D;
using Kinect.Core.Eventing;
using Kinect.Core.Filters.Helper;
using Microsoft.Research.Kinect.Nui;

namespace Kinect.Core.Filters
{
    public class CorrectionFilter : Filter<IUserChangedEvent>
    {
        private const string _name = "CorrectionFilter";

        /// <summary>
        /// Initializes a new instance of the <see cref="CorrectionFilter"/> class.
        /// </summary>
        /// <param name="jointToCorrect">The joint to correct.</param>
        /// <param name="correction">The correction.</param>
        public CorrectionFilter(JointID jointToCorrect, Point3D correction)
        {
            JointToCorrect = jointToCorrect;
            Correction = correction;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get { return _name; }
        }

        public Point3D Correction { get; internal set; }

        public JointID JointToCorrect { get; internal set; }

        /// <summary>
        /// Processes the specified evt.
        /// </summary>
        /// <param name="evt">The evt.</param>
        public override void Process(IUserChangedEvent evt)
        {
            Point3D point = evt.GetPoint(JointToCorrect);
            OnFilteringEvent(new CorrectionFilterEventArgs(JointToCorrect, point, Correction));
            point.X += Correction.X;
            point.Y += Correction.Y;
            point.Z += Correction.Z;
            //TODO: think about solution for this workaround, no <mutatie op de event uitvoeren>
            var user = new User(evt);
            user.SetPoint(JointToCorrect, point);
            base.Process(user);
        }
    }
}