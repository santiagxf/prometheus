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
        public double Area { get; set; }
        public double AnalizedArea { get; set; }
        public double AnalizedAreaPercentage => Area == 0 ? 0 : AnalizedArea / Area;
        public double RequiredFlightTime { get; set; }
        public int RequiredImages { get; set; }
        public MapModel MapModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
