using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Xml.Linq;
using Kinect.Common;
using Kinect.Core.Eventing;
using Kinect.Core.Filters;
using Kinect.Core.Gestures.Helper;
using log4net;
using Microsoft.Research.Kinect.Nui;

namespace Kinect.Core.Gestures
{
    //TODO: implement code for manage filters and pipelines internally here
    //this way we can cleanup all filters and pipelines and add or attach new pipelines and reuse them
    //Make singleton
    public static class GestureFactory
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (GestureFactory));

        public static SemaphoreGesture AddSemaphoreTouchGesture(this User user)
        {
            var gesture = new SemaphoreGesture();
            var fpsFilter = new FramesFilter(6);
            user.AttachPipeline(fpsFilter);
            fpsFilter.AttachPipeline(gesture);
            return gesture;
        }

        public static AccelerationGesture AddAccelerationGesture(this User user)
        {
            var gesture = new AccelerationGesture();
            var fpsFilter = new FramesFilter(6);
            user.AttachPipeline(fpsFilter);
            fpsFilter.AttachPipeline(gesture);
            return gesture;
        }

        public static SelfTouchGesture AddSelfTouchGesture(this User user, Point3D correction, params JointID[] joints)
        {
            if (joints.Length < 2)
            {
                throw new ArgumentException("At least 2 joints are expected for a SelfTouchGesture", "joints");
            }

            XDocument xmlDoc = XDocument.Load(GestureXmlFiles.GesturesXmlFile);
            IEnumerable<XElement> selfTouchGestureNodes = xmlDoc.Root.Descendants("SelfTouchGesture");

            var selfTouchGestures = new List<SelfTouchGesture>(selfTouchGestureNodes.Count());
            SelfTouchGesture selfTouchGesture = null;
            List<Filter<IUserChangedEvent>> filters;

            foreach (XElement node in selfTouchGestureNodes)
            {
                if (!Convert.ToBoolean(node.Attribute(XName.Get("Active")).Value))
                {
                    continue;
                }

                if (!AreJointsMatching(node.Element(XName.Get("PointsToCheck")), joints))
                {
                    continue;
                }

                filters = GetFilters(node.Element(XName.Get("Filters")), joints);
                if (filters == null || filters.Count == 0)
                {
                    filters = GetStandardSelfTouchFilters(joints);
                }

                for (int i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                    {
                        user.AttachPipeline(filters[i]);
                    }
                    else
                    {
                        filters[i - 1].AttachPipeline(filters[i]);
                    }

                    var correctionFilter = filters[i] as CorrectionFilter;
                    if (correctionFilter != null && joints[1].Equals(correctionFilter.JointToCorrect))
                    {
                        correctionFilter.Correction = correction;
                    }
                }

                int historyCount = 10;
                string history = node.Attribute(XName.Get("History")).Value;
                int.TryParse(history, out historyCount);
                selfTouchGesture = new SelfTouchGesture
                                       {
                                           HistoryCount = historyCount,
                                           Joints = joints
                                       };

                filters[filters.Count - 1].AttachPipeline(selfTouchGesture);
            }

            if (selfTouchGesture == null)
            {
                var sb = new StringBuilder();
                sb.Append("No SelfTouch configuration in \"");
                sb.Append(GestureXmlFiles.GesturesXmlFile);
                sb.Append("\" found for joints: ");

                for (int i = 0; i < joints.Length; i++)
                {
                    sb.Append("{"+i+"}");
                }
                _log.IfErrorFormat(sb.ToString(), joints);
                throw new NullReferenceException(string.Format(sb.ToString(), joints));
            }

            return selfTouchGesture;
        }

        public static List<SelfTouchGesture> AddSelfTouchGestures(this User user)
        {
            XDocument xmlDoc = XDocument.Load(GestureXmlFiles.GesturesXmlFile);
            IEnumerable<XElement> selfTouchGestureNodes = xmlDoc.Root.Descendants("SelfTouchGesture");

            var selfTouchGestures = new List<SelfTouchGesture>(selfTouchGestureNodes.Count());
            foreach (XElement node in selfTouchGestureNodes)
            {
                if (!Convert.ToBoolean(node.Attribute(XName.Get("Active")).Value))
                {
                    continue;
                }

                JointID[] joints = null;
                joints = GetJoints(node.Element(XName.Get("PointsToCheck"))).ToArray();

                if (joints != null)
                {
                    List<Filter<IUserChangedEvent>> filters;
                    filters = GetFilters(node.Element(XName.Get("Filters")), joints);

                    for (int i = 0; i < filters.Count; i++)
                    {
                        if (i == 0)
                        {
                            user.AttachPipeline(filters[i]);
                        }
                        else
                        {
                            filters[i - 1].AttachPipeline(filters[i]);
                        }
                    }

                    SelfTouchGesture gesture = null;
                    int historyCount;
                    string history = node.Attribute(XName.Get("History")).Value;

                    if (int.TryParse(history, out historyCount))
                    {
                        gesture = new SelfTouchGesture {HistoryCount = historyCount, Joints = joints};
                    }
                    else
                    {
                        gesture = new SelfTouchGesture {HistoryCount = 10, Joints = joints};
                    }

                    filters[filters.Count - 1].AttachPipeline(gesture);
                    selfTouchGestures.Add(gesture);
                }
            }

            return selfTouchGestures;
        }

        ////TODO: Remove this after implementing full factory
        public static void RemoveGesture(this User user, GestureBase gesture)
        {
            if (gesture == null)
            {
                throw new ArgumentNullException("gesture");
            }

            var filter = user.GetParentFilter(gesture);

            user.DetachPipeline(filter);
            ////TODO: Destroy all pipes
            gesture = null;
        }

        ////TODO: Remove this after implementing full factory
        private static Filter<IUserChangedEvent> GetParentFilter(this EventPublisher<IUserChangedEvent> publisher,
                                                                 GestureBase gesture)
        {
            foreach (Filter<IUserChangedEvent> parentFilter in publisher._pipelines)
            {
                if (parentFilter.ContainsPipe(gesture))
                {
                    return parentFilter;
                }
            }

            return null;
        }

        ////TODO: Remove this after implementing full factory
        private static bool ContainsPipe(this Filter<IUserChangedEvent> publisher, GestureBase gesture)
        {
            if (publisher._pipelines.Contains(gesture))
            {
                return true;
            }
            else
            {
                foreach (Filter<IUserChangedEvent> pub in publisher._pipelines)
                {
                    return pub.ContainsPipe(gesture);
                }
            }

            return false;
        }

        ////TODO: Remove this after implementing full factory
        private static bool ContainsPipe(this EventPublisher<IUserChangedEvent> publisher, GestureBase gesture)
        {
            if (publisher._pipelines.Contains(gesture))
            {
                return true;
            }
            else
            {
                foreach (EventPublisher<IUserChangedEvent> pub in publisher._pipelines)
                {
                    return pub.ContainsPipe(gesture);
                }
            }

            return false;
        }

        private static bool AreJointsMatching(XElement pointsToCheck, params JointID[] givenJoints)
        {
            List<JointID> xmlJoints = GetJoints(pointsToCheck);

            if (xmlJoints != null && xmlJoints.Count == givenJoints.Length)
            {
                foreach (JointID joint in givenJoints)
                {
                    xmlJoints.Remove(joint);
                }
                return xmlJoints.Count == 0;
            }

            return false;
        }

        private static List<Filter<IUserChangedEvent>> GetStandardSelfTouchFilters(JointID[] joints)
        {
            if (joints.Length < 2)
            {
                throw new ArgumentException("At least 2 joints are expected for a SelfTouchGesture", "joints");
            }

            var list = new List<Filter<IUserChangedEvent>>();
            list.Add(new FramesFilter(6));
            var correctionFilter = new CorrectionFilter(joints[1], new Point3D());
            var collisionFilter = new CollisionFilter(new Point3D(), joints);
            return list;
        }

        private static List<JointID> GetJoints(XElement pointsToCheck)
        {
            IEnumerable<XElement> points = pointsToCheck.Descendants();
            var joints = new List<JointID>(points.Count());
            foreach (XElement point in points)
            {
                XAttribute joint = point.Attribute(XName.Get("SkeletonJoint"));
                JointID skeletonJoint;
                if (Enum.TryParse(joint.Value, out skeletonJoint))
                {
                    joints.Add(skeletonJoint);
                }
                else
                {
                    throw new ArgumentException("The provided SkeletonJoint doesn't match the SkeletonJointEnumeration");
                }
            }
            return joints;
        }

        private static List<Filter<IUserChangedEvent>> GetFilters(XElement filters, params JointID[] joints)
        {
            IOrderedEnumerable<XElement> myFilters = from node in filters.Descendants()
                                                     orderby node.Attribute(XName.Get("Sequence")).Value ascending
                                                     select node;

            var filterCollection = new List<Filter<IUserChangedEvent>>(myFilters.Count());
            foreach (XElement filter in myFilters)
            {
                Filter<IUserChangedEvent> createdFilter = null;
                if (filter.Name == "FramesFilter")
                {
                    int fpsCount;
                    string fps = filter.Attribute(XName.Get("FPS")).Value;
                    if (int.TryParse(fps, out fpsCount))
                    {
                        createdFilter = new FramesFilter(fpsCount);
                    }
                    else
                    {
                        createdFilter = new FramesFilter(10);
                    }
                }
                else if (filter.Name == "CollisionFilter")
                {
                    int x, y, z;
                    string xValue = filter.Attribute(XName.Get("MarginX")).Value;
                    string yValue = filter.Attribute(XName.Get("MarginY")).Value;
                    string zValue = filter.Attribute(XName.Get("MarginZ")).Value;

                    if (int.TryParse(xValue, out x) && int.TryParse(yValue, out y) && int.TryParse(zValue, out z))
                    {
                        createdFilter = new CollisionFilter(new Point3D(x, y, z), joints);
                    }
                    else
                    {
                        var exception = new ArgumentException("You need to provide the XYZ margins");

                        _log.IfError(string.Empty, exception);

                        throw exception;
                    }
                }
                else if (filter.Name == "CorrectionFilter")
                {
                    var correction = new Point3D();
                    correction.X = SafeFloatParse(filter.Attribute(XName.Get("MarginX")));
                    correction.Y = SafeFloatParse(filter.Attribute(XName.Get("MarginY")));
                    correction.Z = SafeFloatParse(filter.Attribute(XName.Get("MarginZ")));
                    JointID joint;

                    //TODO better catch if XML isn't correct? DTD
                    if (Enum.TryParse(filter.Attribute(XName.Get("SkeletonJoint")).Value, out joint))
                    {
                        createdFilter = new CorrectionFilter(joint, correction);
                    }
                    else
                    {
                        var exception = new ArgumentException("You need to provide a SkeletonJoint to correct");

                        _log.IfError(string.Empty, exception);

                        throw exception;
                    }
                }
                else
                {
                    var exception = new ArgumentException("The provided filter isn't supported");

                    _log.IfError(string.Empty, exception);

                    throw exception;
                }

                if (createdFilter != null)
                {
                    filterCollection.Add(createdFilter);
                }
            }
            return filterCollection;
        }

        private static float SafeFloatParse(XAttribute attribute)
        {
            float floatvalue = 0;
            if (attribute != null)
            {
                float.TryParse(attribute.Value, out floatvalue);
            }
            return floatvalue;
        }
    }
}