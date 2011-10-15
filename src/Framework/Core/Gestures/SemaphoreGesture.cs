using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Kinect.Common;
using Kinect.Core.Gestures.Helper;
using Kinect.Core.Gestures.Model;

namespace Kinect.Core.Gestures
{
    public class SemaphoreGesture : GestureBase
    {
        private static readonly object _syncRoot = new object();

        private static readonly double _angleMargin =
            GestureXmlReader.ReadSpecificValue<double>(GestureXmlFiles.GesturesXmlFile, "Semaphores", "AngleMargin");

        private static readonly int _historyCount =
            GestureXmlReader.ReadSpecificValue<int>(GestureXmlFiles.GesturesXmlFile, "Semaphores", "HistoryCount");

        private static string _logfile = GestureXmlReader.ReadSpecificValue(GestureXmlFiles.GesturesXmlFile,
                                                                            "Semaphores", "LogFile");

        private readonly LimitedQueue<Semaphore> _history;

        public SemaphoreGesture()
        {
            _history = new LimitedQueue<Semaphore>(_historyCount);
        }

        protected override string GestureName
        {
            get { return "SemaphoreGesture"; }
        }

        public event EventHandler<KinectSemaphoreGestureEventArgs> SemafoorDetected;

        public override void Process(IUserChangedEvent evt)
        {
            AddPoints(evt.HandLeft, evt.ShoulderLeft, evt.HandRight, evt.ShoulderRight);
            Semaphore detectedSemaphore = CheckForSemafoor();

            if (detectedSemaphore != null)
            {
                OnSemafoorDetected(evt, detectedSemaphore);
            }
        }

        protected virtual void OnSemafoorDetected(IUserChangedEvent userEvent, Semaphore semafoor)
        {
            EventHandler<KinectSemaphoreGestureEventArgs> handler = SemafoorDetected;
            if (handler != null)
            {
                handler(this, new KinectSemaphoreGestureEventArgs(userEvent, semafoor));
            }
        }

        private void AddPoints(Point3D leftHand, Point3D leftShoulder, Point3D rightHand, Point3D rightShoulder)
        {
            _history.Enqueue(CalculateSemafoor(leftHand, leftShoulder, rightHand, rightShoulder));
        }

        private Semaphore CheckForSemafoor()
        {
            Semaphore detectedSemaphore = null;
            lock (_syncRoot)
            {
                if (_history == null || _history.Count == 0)
                {
                    return null;
                }

                Semaphore check = _history.Peek();
                int count = _history.AsParallel().Count(s => null != s && s.Equals(check));
                if (count == _history.Count)
                {
                    ////TODO: dit zo nog even controleren
                    detectedSemaphore = check;
                    _history.Clear();
                }
            }

            return detectedSemaphore;
        }

        private Semaphore CalculateSemafoor(Point3D leftHand, Point3D leftBody, Point3D rightHand, Point3D rightBody)
        {
            double leftAngle = 0;
            double rightAngle = 0;
            Parallel.Invoke(
                () => leftAngle = Calculator.CalculateAngle(leftHand.X, leftHand.Y, leftBody.X, leftBody.Y),
                () => rightAngle = Calculator.CalculateAngle(rightHand.X, rightHand.Y, rightBody.X, rightBody.Y));

            return CalculateSemafoor(leftAngle, rightAngle);
        }

        private Semaphore CalculateSemafoor(double leftAngle, double rightAngle)
        {
            return Semaphores.SemafoorGestures.AsParallel()
                .FirstOrDefault(s => WithinMargin(leftAngle, s.LeftAngle, _angleMargin) &&
                                     WithinMargin(rightAngle, s.RightAngle, _angleMargin));
        }

        private bool WithinMargin(double left, double right, double margin)
        {
            return (Math.Abs((left - right)) < margin) || (Math.Abs((right - left)) < margin);
        }
    }
}