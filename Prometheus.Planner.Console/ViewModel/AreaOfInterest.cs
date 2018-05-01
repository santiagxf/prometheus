using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prometeo.Planner.Console.ViewModel
{
    public class AreaOfInterest
    {
        public double Area { get; set; }
        public double AnalizedArea { get; set; }
        public double AnalizedAreaPercentage => Area == 0 ? 0 : AnalizedArea / Area;
        public double RequiredFlightTime { get; set; }
        public int RequiredImages { get; set; }
    }
}
