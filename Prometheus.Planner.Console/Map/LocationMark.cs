using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prometeo.Planner.Console.Map
{
    public class LocationMark : Location
    {
        public string ImagePath { get; set; }
        public ImageDetectionResult DetectionResults { get; set; }

        public LocationMark()
            : base()
        { }
        public LocationMark(double latitude, double longitude, string imagePath, ImageDetectionResult dr)
            :base(latitude, longitude)
        {
            ImagePath = imagePath;
            DetectionResults = dr;
        }

        public override bool Equals(object obj)
        {
            var coord = (LocationMark)obj;

            return Latitude == coord.Latitude && Longitude == coord.Longitude;
        }
        public bool IsLeftOfLine(LocationMark from, LocationMark to)
        {
            return calcCrossProduct(from, to).CompareTo(0) > 0;
        }
        private double calcCrossProduct(LocationMark origin, LocationMark p2)
        {
            return (p2.Latitude - origin.Latitude) * (this.Longitude - origin.Longitude)
                    - (p2.Longitude - origin.Longitude) * (this.Latitude - origin.Latitude);
        }
        public double DistanceToLine(LocationMark a, LocationMark b)
        {
            return Math.Abs((b.Longitude - a.Longitude) * (a.Latitude - this.Latitude) - (a.Longitude - this.Longitude) * (b.Latitude - a.Latitude))
                    / Math.Sqrt(Math.Pow(b.Longitude - a.Longitude, 2) + Math.Pow(b.Latitude - a.Latitude, 2));
        }
    }
}
