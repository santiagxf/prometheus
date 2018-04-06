using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prometeo.Planner.Console.Map
{
    public static class MapPolygonExtensions
    {
        public static double CoveredArea(this MapPolygon mp)
        {
            double sum = 0;
            double prevcolat = 0;
            double prevaz = 0;
            double colat0 = 0;
            double az0 = 0;

            for (int i = 0; i < mp.Locations.Count; i++)
            {
                double colat = 2 * Math.Atan2(Math.Sqrt(Math.Pow(Math.Sin(mp.Locations[i].Latitude * Math.PI / 180 / 2), 2) + Math.Cos(mp.Locations[i].Latitude * Math.PI / 180) * Math.Pow(Math.Sin(mp.Locations[i].Longitude * Math.PI / 180 / 2), 2)), Math.Sqrt(1 - Math.Pow(Math.Sin(mp.Locations[i].Latitude * Math.PI / 180 / 2), 2) - Math.Cos(mp.Locations[i].Latitude * Math.PI / 180) * Math.Pow(Math.Sin(mp.Locations[i].Longitude * Math.PI / 180 / 2), 2)));
                double az = 0;
                if (mp.Locations[i].Latitude >= 90)
                {
                    az = 0;
                }
                else if (mp.Locations[i].Latitude <= -90)
                {
                    az = Math.PI;
                }
                else
                {
                    az = Math.Atan2(Math.Cos(mp.Locations[i].Latitude * Math.PI / 180) * Math.Sin(mp.Locations[i].Longitude * Math.PI / 180), Math.Sin(mp.Locations[i].Latitude * Math.PI / 180)) % (2 * Math.PI);
                }
                if (i == 0)
                {
                    colat0 = colat;
                    az0 = az;
                }
                if (i > 0 && i < mp.Locations.Count)
                {
                    sum = sum + (1 - Math.Cos(prevcolat + (colat - prevcolat) / 2)) * Math.PI * ((Math.Abs(az - prevaz) / Math.PI) - 2 * Math.Ceiling(((Math.Abs(az - prevaz) / Math.PI) - 1) / 2)) * Math.Sign(az - prevaz);
                }
                prevcolat = colat;
                prevaz = az;
            }
            sum = sum + (1 - Math.Cos(prevcolat + (colat0 - prevcolat) / 2)) * (az0 - prevaz);
            return 5.10072E14 * Math.Min(Math.Abs(sum) / 4 / Math.PI, 1 - Math.Abs(sum) / 4 / Math.PI);
        }

        public static Location CenterPosition(this MapPolygon mp)
        {
            if (mp.Locations.Count == 1)
                return mp.Locations.Single();

            double x = 0;
            double y = 0;
            double z = 0;

            foreach (var geoCoordinate in mp.Locations)
            {
                var latitude = geoCoordinate.Latitude * Math.PI / 180;
                var longitude = geoCoordinate.Longitude * Math.PI / 180;

                x += Math.Cos(latitude) * Math.Cos(longitude);
                y += Math.Cos(latitude) * Math.Sin(longitude);
                z += Math.Sin(latitude);
            }

            var total = mp.Locations.Count;

            x = x / total;
            y = y / total;
            z = z / total;

            var centralLongitude = Math.Atan2(y, x);
            var centralSquareRoot = Math.Sqrt(x * x + y * y);
            var centralLatitude = Math.Atan2(z, centralSquareRoot);

            return new Location()
            {
                Latitude = centralLatitude * 180 / Math.PI,
                Longitude = centralLongitude * 180 / Math.PI
            };
        }

        public static double ToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }
    }
}
