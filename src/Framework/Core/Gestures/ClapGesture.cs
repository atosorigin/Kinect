using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Common;
using xn;

namespace Kinect.Core.Gestures
{
    public class ClapGesture : GestureBase
    {
        private LimitedQueue<Hands> _history;
        internal static int PointCount = 20;

        internal static int MarginX = 8;
        internal static int MarginY = 20;
        internal static int MarginZ = 30;
        private int _doubleClapCheck = -1;

        public const string LogFile = @"c:\temp\LogFile.txt";

        public event EventHandler<KinectGestureEventArgs> SingleClap;
        protected virtual void OnSingleClap(IUserChangedEvent userEvent)
        {
            var handler = this.SingleClap;
            if (handler != null)
            {
                handler(this, new KinectGestureEventArgs(userEvent));
            }
        }

        public event EventHandler<KinectGestureEventArgs> DoubleClap;
        protected virtual void OnDoubleClap(IUserChangedEvent userEvent)
        {
            var handler = this.DoubleClap;
            if (handler != null)
            {
                handler(this, new KinectGestureEventArgs(userEvent));
            }
        }

        public ClapGesture()
            : base()
        {
            this._history = new LimitedQueue<Hands>(PointCount);
        }

        public void AddPoints(Point3D left, Point3D right)
        {
            this.AddPoints(new Hands(left, right));
        }

        public void AddPoints(Hands hand)
        {
            if (this.DoubleClap != null)
            {
                ////Do some work for double clap
                this._history.Enqueue(hand);
                ////Do some work for single clap
                bool clap = hand.DetectClap();
                if (clap && this._doubleClapCheck < 0)
                {
                    this._doubleClapCheck = PointCount;
                }
                else if (this._doubleClapCheck == 0)
                {
                    ////TODO: remove call to camera and use OnSingleclap Camera.Instance.OnKinectEventHandlerEvent(SingleClap);
                    this._doubleClapCheck--;
                }
                else if (this._doubleClapCheck >= 0)
                {
                    if (this.CheckForDoubleClap())
                    {
                        this._doubleClapCheck = -1;
                    }
                    else
                    {
                        this._doubleClapCheck--;
                    }
                }
            }
            else
            {
                ////Do some work for single clap
                if (hand.DetectClap() && this._doubleClapCheck < 0)
                {
                    this._doubleClapCheck = 10;
                }
                else if (this._doubleClapCheck >= 0)
                {
                    this._doubleClapCheck--;
                }
            }
        }

        private bool CheckForDoubleClap()
        {
            int nrOfClaps = 0;
            int wait = 0;
            foreach (var hands in this._history)
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
                this._history.Clear();
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
            return this.WriteLog();
        }

        private string WriteLog()
        {
            StringBuilder sb = new StringBuilder();
            Hands[] array = new Hands[PointCount]; ;
            this._history.CopyTo(array, 0);
            foreach (Hands p in array)
            {
                if (p != null)
                {
                    sb.AppendLine(p.ToString());
                }
            }
            return sb.ToString();
        }

        protected override string GestureName
        {
            get { return "ClapGesture"; }
        }

        public override void Process(IUserChangedEvent evt)
        {
            ////TODO: Gesture <logica hier aanroepen en andere methodes private maken>
            ////OnSingleClap(evt);
            ////OnDoubleClap(evt);
        }
    }
}
