using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Kinect.Common;
using Kinect.Core.Gestures.Helper;
using Kinect.Core.Gestures.Model;
using System.Windows.Media.Media3D;

namespace Kinect.Core.Gestures
{
    public class SemaphoreGesture : GestureBase
    {
        private static object _syncRoot = new object();
        private static double _angleMargin = GestureXmlReader.ReadSpecificValue<double>(GestureXmlFiles.GesturesXmlFile, "Semaphores", "AngleMargin");
        private static int _historyCount = GestureXmlReader.ReadSpecificValue<int>(GestureXmlFiles.GesturesXmlFile, "Semaphores", "HistoryCount");
        private static string _logfile = GestureXmlReader.ReadSpecificValue(GestureXmlFiles.GesturesXmlFile, "Semaphores", "LogFile");
        private LimitedQueue<Semaphore> _history;

        protected override string GestureName
        {
            get { return "SemaphoreGesture"; }
        }

        public event EventHandler<KinectSemaphoreGestureEventArgs> SemafoorDetected;

        public SemaphoreGesture()
            : base()
        {
            this._history = new LimitedQueue<Semaphore>(_historyCount);
        }

        public override void Process(IUserChangedEvent evt)
        {
            this.AddPoints(evt.LeftHand, evt.LeftShoulder, evt.RightHand, evt.RightShoulder);
            var detectedSemaphore = this.CheckForSemafoor();

            if (detectedSemaphore != null)
            {
                this.OnSemafoorDetected(evt, detectedSemaphore);
            }
        }

        protected virtual void OnSemafoorDetected(IUserChangedEvent userEvent, Semaphore semafoor)
        {
            var handler = this.SemafoorDetected;
            if (handler != null)
            {
                handler(this, new KinectSemaphoreGestureEventArgs(userEvent, semafoor));
            }
        }

        private void AddPoints(Point3D leftHand, Point3D leftShoulder, Point3D rightHand, Point3D rightShoulder)
        {
            this._history.Enqueue(this.CalculateSemafoor(leftHand, leftShoulder, rightHand, rightShoulder));
        }

        private Semaphore CheckForSemafoor()
        {
            Semaphore detectedSemaphore = null;
            lock (_syncRoot)
            {
                if (this._history == null || this._history.Count == 0)
                {
                    return null;
                }

                var check = this._history.Peek();
                var count = this._history.AsParallel().Count(s => null != s && s.Equals(check));
                if (count == this._history.Count)
                {
                    ////TODO: dit zo nog even controleren
                    detectedSemaphore = check;
                    this._history.Clear();
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

            return this.CalculateSemafoor(leftAngle, rightAngle);
        }

        private Semaphore CalculateSemafoor(double leftAngle, double rightAngle)
        {
            return Semaphores.SemafoorGestures.AsParallel()
                .FirstOrDefault(s => this.WithinMargin(leftAngle, s.LeftAngle, _angleMargin) &&
                    this.WithinMargin(rightAngle, s.RightAngle, _angleMargin));
        }

        private bool WithinMargin(double left, double right, double margin)
        {
            return (Math.Abs((left - right)) < margin) || (Math.Abs((right - left)) < margin);
        }
    }
}
