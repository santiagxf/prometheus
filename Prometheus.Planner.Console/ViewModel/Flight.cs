using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Prometeo.Planner.Console.ViewModel
{
    public class Flight
    {
        public string Folder { get; set; }
        public DateTime Date { get; set; }
        public int Images { get; set; }
        public double CoveredArea { get; set; }
        public IEnumerable<ImageDetectionResult> Results { get; set; }
        public bool FireDetected { get; set; }
        public string ResultAsString => FireDetected ? "At risk" : "Clean";
        public SolidColorBrush AreaColor { get; set; }
    }
}
