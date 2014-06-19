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
                state = value;
                RaisePropertyChanged();
            }
        }
        public Point Location { get; private set; }

        public Cell(Point location)
        {
            this.Location = location;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged([CallerMemberName] string propName = "")
        {
                var handler = PropertyChanged;
                handler(this, new PropertyChangedEventArgs(propName));
            }

        internal void FlipState()
        {
            state = !state;
            RaisePropertyChanged("State");
        }
    }
}
