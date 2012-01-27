using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Kinect.SpinToWin.Controls
{
    class PieData : INotifyPropertyChanged
    {
        private String _name;

        public String Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChangeEvent("Name");
            }
        }

        private double _size;

        public double Size
        {
            get { return _size; }
            set
            {
                _size = value;
                RaisePropertyChangeEvent("Size");
            }
        }

        public static List<PieData> ConstructPies()
        {
            var pies = new List<PieData>();

            for (var i = 1; i <= 51; i++)
            {
                pies.Add(new PieData() { Name = String.Concat("Player",i), Size = 1 });
            }
 
            return pies;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangeEvent(String propertyName)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }

        #endregion
    }
}
