using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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

        public static List<PieData> ConstructPies(IEnumerable<string> players)
        {
            return players.Select(player => new PieData() {Name = player, Size = 1}).ToList();
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
