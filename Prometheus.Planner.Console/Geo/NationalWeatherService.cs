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

namespace Prometeo.Planner.Console.Geo
{
    public class NationalWeatherService
    {
        public static IEnumerable<string> QueryFireAlerts()
        {
            var nwsUrl = ConfigurationManager.AppSettings["fireAlerts_us"].ToString();
            var fireAlertsStream = RESTTools.Get(nwsUrl);

            JObject jObj = JObject.Parse(fireAlertsStream);
            var alerts = jObj.SelectTokens("$.features[?(@.properties.event == 'Fire Weather Watch')].properties.geocode.UGC");

            foreach (var alert in alerts)
                foreach (var zone in alert)
                    yield return (string)zone;

            yield break;
        }

        public static IEnumerable<LocationMark> ConvertUGCtoPosition(IEnumerable<string> ugcs)
        {
            var gisServices = ConfigurationManager.AppSettings["prometheusWebServiceUrl"].ToString() + "/gis/{0}";
            
            foreach(var ugc in ugcs)
            {
                var coordResult = RESTTools.Get(gisServices, ugc);
                var jObj = JsonConvert.DeserializeObject<List<double>>(coordResult);
                if (jObj != null)
                    yield return new LocationMark()
                    {
                        Longitude = jObj[0],
                        Latitude = jObj[1],
                    };
            }
        }
        public static void ConvertUGCtoPolygon(IEnumerable<string> ugcs, MapModel map)
        {
            var gisServices = ConfigurationManager.AppSettings["prometheusWebServiceUrl"].ToString() + "/gis/{0}";
            List<List<double>> jObj;

            foreach (var ugc in ugcs)
            {
                var coordResult = RESTTools.Get(gisServices, ugc);
                try
                {
                    jObj = JsonConvert.DeserializeObject<List<List<double>>>(coordResult);
                }
                catch(Exception)
                {
                    jObj = null;
                }
                if (jObj != null)
                {
                    map.StartNewPolygon();
                    foreach (var p in jObj)
                        map.AddPointToPolygon(new Location(p[1], p[0]));
                    map.FinishCurrentPolygon();
                }
            }
        }
    }
}
