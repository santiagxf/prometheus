using Prometeo.Planner.Console.Map;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Prometeo.Planner.Console.ViewModel
{
    public class AreaOfInterest : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public double Area { get; set; }
        public double AnalizedArea { get; set; }
        public double AnalizedAreaPercentage => Area == 0 ? 0 : AnalizedArea / Area;
        public double RequiredFlightTime { get; set; }
        public int RequiredImages { get; set; }
        public MapModel MapModel { get; set; }
        public List<Flight> Flights { get; private set; }

        public AreaOfInterest()
        {
            Flights = new List<Flight>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
