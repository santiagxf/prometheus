using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maps.MapControl.WPF;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prometeo.Planner.Console.Map;
using Prometeo.Planner.Console.Tools;
using Catfood.Shapefile;

namespace Prometeo.Planner.Console.Geo
{
    public class NationalWeatherService
    {
        public static readonly string FIRE_WATCH_ALERT = "Fire Weather Watch";
        public static readonly string RED_FLAG_ALERT = "Red Flag Warning";
        public static IEnumerable<string> QueryFireAlerts(string alertType, string countryCode)
        {
            var nwsUrl = ConfigurationManager.AppSettings["fireAlerts_us"].ToString();
            var fireAlertsStream = RESTTools.Get(nwsUrl);

            if (string.IsNullOrEmpty(fireAlertsStream))
                yield break;

            JObject jObj = JObject.Parse(fireAlertsStream);
            var alerts = jObj.SelectTokens("$.features[?(@.properties.event == 'Red Flag Warning')].properties.geocode.UGC");
            //var alerts = jObj.SelectTokens("$.features[?(@.properties.event == 'Fire Weather Watch')].properties.geocode.UGC");

            foreach (var alert in alerts)
                foreach (var zone in alert)
                    yield return (string)zone;

            yield break;
        }

        public static IEnumerable<LocationMark> ConvertUGCtoPosition(IEnumerable<string> ugcs)
        {
            using (var shp = new Shapefile("./gis/us/fz25jn18.shp"))
            {
                foreach (var ugc in ugcs)
                {
                    var state = ugc.Substring(0, 2);
                    var zone = ugc.Substring(3, ugc.Length);

                    foreach (Shape row in shp)
                    {
                        if ((string)row.DataRecord[0] == state && (string)row.DataRecord[1] == zone)
                        {
                            ShapePolygon p = row as ShapePolygon;

                            yield return new LocationMark()
                            {
                                Longitude = (double)row.DataRecord[7],
                                Latitude = (double) row.DataRecord[8]
                            };
                            break;
                        }
                    }
                }
            }
        }
        public static void ConvertUGCtoPolygon(IEnumerable<string> ugcs, MapModel map)
        {
            using (var shp = new Shapefile("./gis/us/fz25jn18.shp"))
            {
                var ugcsLeft = ugcs.ToList();
                foreach (Shape row in shp)
                {
                    if (!ugcsLeft.Any())
                        break;

                    var ugc = string.Concat((string)row.DataRecord[0], "Z", (string)row.DataRecord[1]);

                    if (ugcsLeft.Contains(ugc))
                    {
                        ShapePolygon p = row as ShapePolygon;
                        map.StartNewPolygon();

                        foreach (var part in p.Parts)
                            foreach (var point in part)
                                map.AddPointToPolygon(new Location(point.Y, point.X));

                        map.FinishCurrentPolygon();

                        map.Marks.Add(new LocationMark()
                        {
                            Longitude = (double)row.DataRecord[7],
                            Latitude = (double)row.DataRecord[8]
                        });

                        ugcsLeft.Remove(ugc);
                    }
                }
            }
        }

        public static void GetAllPolygonsFromShape(MapModel map, string geo)
        {
            using (var shp = new Shapefile(string.Format("./gis/{0}/alerts.shp", geo)))
            {
                foreach (Shape row in shp)
                {
                    ShapePolygon p = row as ShapePolygon;
                    map.StartNewPolygon();

                    foreach (var part in p.Parts)
                        foreach (var point in part)
                            map.AddPointToPolygon(new Location(point.Y, point.X));

                    var pol = map.FinishCurrentPolygon();
                    var center = pol.CenterPosition();


                    map.Marks.Add(new LocationMark()
                    {
                        Longitude = center.Longitude,
                        Latitude = center.Latitude,
                    });
                }
            }
        }
    }
}
