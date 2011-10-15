using System;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Media3D;
using GalaSoft.MvvmLight;
using HiddenMarkovModel.Utils;
using Kinect.Common;
using Kinect.Core;
using Kinect.Core.Eventing;

namespace Kinect.GestureDetection.ViewModels
{
    internal class TrackingViewModel : ViewModelBase
    {
        private static object _syncRoot = new object();
        private readonly LimitedObservations<double> _capturedSequence;
        private readonly Models.GestureDetection _gestureDetection;
        private readonly MotionCalculator _motionCalculator;

        private readonly double _updateMargin = 10;
        private readonly User _user;
        private readonly int timeBetweenCapture = 2;
        private string _classification;
        private string _direction;
        private Point3D _rightHand;
        private string _sequence;
        private Point3D _startValue;
        private int i;

        public TrackingViewModel(int id)
            : this(new User(id))
        {
        }

        public TrackingViewModel(User user)
        {
            _user = user;
            _user.Updated += _user_Updated;
            _motionCalculator = new MotionCalculator();
            _gestureDetection = new Models.GestureDetection();
            _capturedSequence = new LimitedObservations<double>(_gestureDetection.ObservationLength);
        }

        public int ID
        {
            get { return _user.Id; }
        }

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
                        double direction = _motionCalculator.CalculateMotion(_startValue, value);
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
                        {
                            Sequence = string.Empty;
                        }


                        Direction = direction.ToString();
                        i = 0;
                    }
                    i++;
                }
            }
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

        private void _user_Updated(object sender, ProcessEventArgs<IUserChangedEvent> e)
        {
            PropertyInfo[] eventProperties = e.Event.GetType().GetProperties();
            Type thisType = GetType();
            eventProperties.AsParallel().ForAll(pi =>
                                                    {
                                                        PropertyInfo prop = thisType.GetProperty(pi.Name);
                                                        if (prop != null && prop.CanWrite)
                                                        {
                                                            object propValue = pi.GetValue(e.Event, null);
                                                            object newValue = propValue;
                                                            if (pi.PropertyType == typeof (Point3D))
                                                            {
                                                                newValue = ((Point3D) propValue);
                                                            }
                                                            prop.SetValue(this, newValue, null);
                                                        }
                                                    });
        }

        private string Classify()
        {
            return _gestureDetection.ProcessObservation(_capturedSequence.ToList());
        }
    }
}