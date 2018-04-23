using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prometeo.Planner.Console.Geo
{
    public struct GeoCoordinate
    {
        public GeoCoordinate(double lon, double lat)
        {
            Latitude = lat;
            Longitude = lon;
            Name = string.Empty;
        }

        public GeoCoordinate(string name, double lon, double lat)
        {
            Latitude = lat;
            Longitude = lon;
            Name = name;
        }

        public double Longitude;
        public double Latitude;
        public string Name;

        public override bool Equals(object obj)
        {
            var coord = (GeoCoordinate)obj;

            return Latitude == coord.Latitude && Longitude == coord.Longitude;
        }
        public bool IsLeftOfLine(GeoCoordinate from, GeoCoordinate to)
        {
            return calcCrossProduct(from, to).CompareTo(0) > 0;
        }
        private double calcCrossProduct(GeoCoordinate origin, GeoCoordinate p2)
        {
            return (p2.Latitude - origin.Latitude) * (this.Longitude - origin.Longitude)
                    - (p2.Longitude - origin.Longitude) * (this.Latitude - origin.Latitude);
        }
        public double DistanceToLine(GeoCoordinate a, GeoCoordinate b)
        {
            return Math.Abs((b.Longitude - a.Longitude) * (a.Latitude - this.Latitude) - (a.Longitude - this.Longitude) * (b.Latitude - a.Latitude))
                    / Math.Sqrt(Math.Pow(b.Longitude - a.Longitude, 2) + Math.Pow(b.Latitude - a.Latitude, 2));
        }
    }
}
