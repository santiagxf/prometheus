using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Prometeo.Planner.Console.ViewModel
{
    public class AreasOfInterestCollection : ObservableCollection<AreaOfInterest>
    {
        public void AddToAnalizedArea(Flight flight)
        {
            var areaUnfilled = Items.Where((a) => a.AnalizedAreaPercentage < 1).FirstOrDefault();
            if (areaUnfilled != null)
            {
                areaUnfilled.Flights.Add(flight);
                areaUnfilled.AnalizedArea += flight.CoveredArea;
            }
        }

        internal void NotifyAllPropertyChanged(string v)
        {
            foreach (var ar in Items)
                ar.NotifyPropertyChanged(v);
        }
    }
}
