using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CellularSim.Model
{
    public class Cell : INotifyPropertyChanged
    {
        private bool state;
        public bool State
        {
            get
            {
                return state;
            }
            set
            {
                SetAndRaisePropertyChanged(ref state, value);
            }
        }
        public Point Location { get; private set; }

        public Cell(Point location)
        {
            this.Location = location;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void SetAndRaisePropertyChanged(ref bool field,  bool value, [CallerMemberName] string propName = "")
        {
            if (!EqualityComparer<bool>.Default.Equals(value, field))
            {
                var handler = PropertyChanged;
                field = value;
                handler(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
