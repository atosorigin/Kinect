using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using System.Windows.Media.Media3D;
using Kinect.Common;
using Kinect.Core;
using Common;
using System.Windows.Threading;

namespace Kinect.WPF.GestureDetection.ViewModels
{
    class TrackingViewModel : ViewModelBase
    {
        LimitedObservations<double> _capturedSequence;
        private HiddenMarkovModel.Utils.MotionCalculator _motionCalculator;
        private Point3D _startValue;
        private readonly int timeBetweenCapture = 2;
        private GestureDetection.Models.GestureDetection _gestureDetection;

        int i = 0;

        private readonly double _updateMargin = 10;
        private static object _syncRoot = new object();
        private User _user;

        public uint ID
        {
            get { return _user.ID; }
        }

        private bool Changed(Point3D newValue, Point3D oldValue)
        {
            if (Math.Abs(newValue.X - oldValue.X) >= _updateMargin ||
                Math.Abs(newValue.Y - oldValue.Y) >= _updateMargin //||
               // Math.Abs(newValue.Z - oldValue.Z) >= _updateMargin
                )
            {
                return true;
            }
            return false;
        }

        public TrackingViewModel(uint id)
            : this(new User(id))
        {
        }

        public TrackingViewModel(User user)
        {
            _user = user;
            _user.Updated += _user_Updated;
            _motionCalculator = new HiddenMarkovModel.Utils.MotionCalculator();
            _gestureDetection = new Models.GestureDetection();
            _capturedSequence = new LimitedObservations<double>(_gestureDetection.ObservationLength);
        }

        private void _user_Updated(object sender, Core.Eventing.ProcessEventArgs<IUserChangedEvent> e)
        {
            var eventProperties = e.Event.GetType().GetProperties();
            var thisType = this.GetType();
            eventProperties.AsParallel().ForAll(pi =>
            {
                var prop = thisType.GetProperty(pi.Name);
                if (prop != null && prop.CanWrite)
                {
                    var propValue = pi.GetValue(e.Event, null);
                    object newValue = propValue;
                    if (pi.PropertyType == typeof(xn.Point3D))
                    {
                        newValue = ((xn.Point3D)propValue).GetMediaPoint3D();
                    }
                    prop.SetValue(this, newValue, null);
                }
            });
        }

        private string _direction;
        public string Direction
        {
            get { return _direction; }
            set
            {
                if (value != _direction)
                {
                    _direction = value;
                    RaisePropertyChanged("Direction");
                }
            }
        }

        private string _classification;
        public string Classification
        {
            get { return _classification; }
            set
            {
                if (value != _classification && value != string.Empty)
                {
                    _classification = value;
                    _capturedSequence.Clear();
                    RaisePropertyChanged("Classification");
                }
            }
        }

        private string _sequence;
        public string Sequence
        {
            get { return _sequence; }
            set
            {
                if (value != _sequence)
                {
                    _sequence = value;
                    RaisePropertyChanged("Sequence");
                }
            }
        }


        private string Classify()
        {
                return _gestureDetection.ProcessObservation(_capturedSequence.ToList());
        }

        private Point3D _rightHand;
        public Point3D RightHand
        {
            get { return _rightHand; }
            set
            {
                if (Changed(value, _rightHand))
                {
                    _rightHand = value;
                    RaisePropertyChanged("RightHand");
                    if (i == 1)
                    {
                        _startValue = value;
                    }
                    else if (i >= timeBetweenCapture && Changed(value, _startValue))
                    {
                        //Calculate direction and insert into observations
                        var direction = _motionCalculator.CalculateMotion(_startValue, value);
                        if (_capturedSequence.LastOrDefault() != direction)
                        {
                            _capturedSequence.InsertObservation(direction);
                            Sequence += direction.ToString();
                        }
                        
                        if (_capturedSequence.Count == _gestureDetection.ObservationLength)
                        {
                            Classification = Classify();
                        }
                        if (Sequence != null && Sequence.Length > 50)
                        { Sequence = string.Empty; }
                        

                        Direction = direction.ToString();
                        i = 0;
                    }
                    i++;
                }
            }
        }
    }
}
