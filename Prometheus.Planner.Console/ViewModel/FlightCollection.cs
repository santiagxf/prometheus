using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prometeo.Planner.Console.Map;

namespace Prometeo.Planner.Console.ViewModel
{
    public class FlightCollection : ObservableCollection<Flight>
    {
        public bool RemoveMark(LocationMark location)
        {
            foreach (var f in Items)
                if (f.MapModel.RemoveMark(location))
                    return true;

            return false;
        }
    }
}
