using Microsoft.Maps.MapControl.WPF;
using Prometeo.Planner.Console.Map;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prometeo.Planner.Console.Geo
{
    public class GeoCoordinateCollection : Collection<LocationMark>
    {
        public GeoCoordinateCollection()
            : base()
        { }

        public GeoCoordinateCollection(IList<LocationMark> locations)
            : base(locations)
        { }
        public LocationMark CalculateCenterLocation()
        {
            if (Items.Count == 1)
                return Items.Single();

            double x = 0;
            double y = 0;
            double z = 0;

            foreach (var geoCoordinate in Items)
            {
                var latitude = geoCoordinate.Latitude * Math.PI / 180;
                var longitude = geoCoordinate.Longitude * Math.PI / 180;

                x += Math.Cos(latitude) * Math.Cos(longitude);
                y += Math.Cos(latitude) * Math.Sin(longitude);
                z += Math.Sin(latitude);
            }

            var total = Items.Count;

            x = x / total;
            y = y / total;
            z = z / total;

            var centralLongitude = Math.Atan2(y, x);
            var centralSquareRoot = Math.Sqrt(x * x + y * y);
            var centralLatitude = Math.Atan2(z, centralSquareRoot);

            return new LocationMark()
            {
                Latitude = centralLatitude * 180 / Math.PI,
                Longitude = centralLongitude * 180 / Math.PI
            };
        }

        private LocationMark getRightmostPoint()
        {
            return Items.OrderByDescending((p) => p.Longitude).First();
        }
        private LocationMark getLeftmostPoint()
        {
            return Items.OrderBy((p) => p.Longitude).First();
        }

        public GeoCoordinateCollection ConvexHull()
        {
            List<LocationMark> convexHull = new List<LocationMark>();
            if (!Items.Any())
                return new GeoCoordinateCollection();

            // search extreme values
            var rightmostPoint = getRightmostPoint();
            var leftmostPoint = getLeftmostPoint();

            // divide the set into two halfes
            var leftOfLine = new List<LocationMark>();
            var rightOfLine = new List<LocationMark>();

            foreach (LocationMark point in Items)
            {
                if (point.Equals(rightmostPoint) || point.Equals(leftmostPoint))
                    continue;

                if (point.IsLeftOfLine(leftmostPoint, rightmostPoint))
                    leftOfLine.Add(point);
                else
                    rightOfLine.Add(point);
            }

            convexHull.Add(leftmostPoint);
            List<LocationMark> hull = divide(leftOfLine, leftmostPoint, rightmostPoint);
            convexHull.AddRange(hull);
            convexHull.Add(rightmostPoint);

            hull = divide(rightOfLine, rightmostPoint, leftmostPoint);
            convexHull.AddRange(hull);


            return new GeoCoordinateCollection(convexHull);
        }

        private List<LocationMark> divide(List<LocationMark> points, LocationMark p1, LocationMark p2)
        {

            List<LocationMark> hull = new List<LocationMark>();

            if (!points.Any())
                return hull;
            else if (points.Count == 1)
            {
                hull.Add(points[0]);
                return hull;
            }

            LocationMark maxDistancePoint = points[0];
            List<LocationMark> l1 = new List<LocationMark>();
            List<LocationMark> l2 = new List<LocationMark>();
            double distance = 0.0;
            foreach (LocationMark point in points)
            {
                if (point.DistanceToLine(p1, p2) > distance)
                {
                    distance = point.DistanceToLine(p1, p2);
                    maxDistancePoint = point;
                }
            }

            points.Remove(maxDistancePoint);

            foreach (LocationMark point in points)
            {
                if (point.IsLeftOfLine(p1, maxDistancePoint))
                {
                    l1.Add(point);
                }
                else if (point.IsLeftOfLine(maxDistancePoint, p2))
                {
                    l2.Add(point);
                }
            }

            points.Clear();

            List<LocationMark> hullPart = divide(l1, p1, maxDistancePoint);
            hull.AddRange(hullPart);
            hull.Add(maxDistancePoint);
            hullPart = divide(l2, maxDistancePoint, p2);
            hull.AddRange(hullPart);

            return hull;
        }
    }
}
