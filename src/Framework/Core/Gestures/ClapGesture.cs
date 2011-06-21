using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media.Media3D;
using Kinect.Common;

namespace Kinect.Core.Gestures
{
    public class ClapGesture : GestureBase
    {
        public const string LogFile = @"c:\temp\LogFile.txt";
        internal static int PointCount = 20;

        internal static int MarginX = 8;
        internal static int MarginY = 20;
        internal static int MarginZ = 30;
        private readonly LimitedQueue<Hands> _history;
        private int _doubleClapCheck = -1;

        public ClapGesture()
        {
            _history = new LimitedQueue<Hands>(PointCount);
        }

        protected override string GestureName
        {
            get { return "ClapGesture"; }
        }

        public event EventHandler<KinectGestureEventArgs> SingleClap;

        protected virtual void OnSingleClap(IUserChangedEvent userEvent)
        {
            EventHandler<KinectGestureEventArgs> handler = SingleClap;
            if (handler != null)
            {
                handler(this, new KinectGestureEventArgs(userEvent));
            }
        }

        public event EventHandler<KinectGestureEventArgs> DoubleClap;

        protected virtual void OnDoubleClap(IUserChangedEvent userEvent)
        {
            EventHandler<KinectGestureEventArgs> handler = DoubleClap;
            if (handler != null)
            {
                handler(this, new KinectGestureEventArgs(userEvent));
            }
        }

        public void AddPoints(Point3D left, Point3D right)
        {
            AddPoints(new Hands(left, right));
        }

        public void AddPoints(Hands hand)
        {
            if (DoubleClap != null)
            {
                ////Do some work for double clap
                _history.Enqueue(hand);
                ////Do some work for single clap
                bool clap = hand.DetectClap();
                if (clap && _doubleClapCheck < 0)
                {
                    _doubleClapCheck = PointCount;
                }
                else if (_doubleClapCheck == 0)
                {
                    ////TODO: remove call to camera and use OnSingleclap Camera.Instance.OnKinectEventHandlerEvent(SingleClap);
                    _doubleClapCheck--;
                }
                else if (_doubleClapCheck >= 0)
                {
                    if (CheckForDoubleClap())
                    {
                        _doubleClapCheck = -1;
                    }
                    else
                    {
                        _doubleClapCheck--;
                    }
                }
            }
            else
            {
                ////Do some work for single clap
                if (hand.DetectClap() && _doubleClapCheck < 0)
                {
                    _doubleClapCheck = 10;
                }
                else if (_doubleClapCheck >= 0)
                {
                    _doubleClapCheck--;
                }
            }
        }

        private bool CheckForDoubleClap()
        {
            int nrOfClaps = 0;
            int wait = 0;
            foreach (Hands hands in _history)
            {
                if (wait > 0)
                {
                    wait--;
                }
                else if (hands.DetectClap())
                {
                    nrOfClaps++;
                    wait = 4;
                }
            }

            if (nrOfClaps >= 2)
            {
                _history.Clear();
                return true;
            }

            return false;
        }

        private void AddPoint(Point3D point, List<Point3D> list)
        {
            //list.Add(point);
            //while (list.Count > this.PointCount)
            //{
            //    list.RemoveAt(0);
            //}
        }

        public void WriteToLogFile()
        {
            using (StreamWriter log = !File.Exists(LogFile) ? new StreamWriter(LogFile) : File.AppendText(LogFile))
            {
                log.Write(WriteLog());
            }
        }

        public string WriteToLogString()
        {
            return WriteLog();
        }

        private string WriteLog()
        {
            var sb = new StringBuilder();
            var array = new Hands[PointCount];
            ;
            _history.CopyTo(array, 0);
            foreach (Hands p in array)
            {
                if (p != null)
                {
                    sb.AppendLine(p.ToString());
                }
            }
            return sb.ToString();
        }

        public override void Process(IUserChangedEvent evt)
        {
            ////TODO: Gesture <logica hier aanroepen en andere methodes private maken>
            ////OnSingleClap(evt);
            ////OnDoubleClap(evt);
        }
    }
}