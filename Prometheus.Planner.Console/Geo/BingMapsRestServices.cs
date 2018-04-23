using ExifLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Prometeo.Planner.Console.Geo
{
    public static class BingMapsRestServices
    {
        public static readonly string BingMapsKey = "KPbtiXkBX8VRcWffNjT0 ~j9fmVQYKiGxco8E8u7d9Rw~AtCEMlc7ixbT5zT531wvU_znIXzBl7JKNjOh6D_Wdfdo8hcnDP3LAFAc_zMZBhRN";
        public static string CountryCode = "US";

        private static XmlDocument Geocode(string loca)
        {
            //Create REST Services geocode request using Locations API
            string geocodeRequest = "http://dev.virtualearth.net/REST/v1/Locations?locality=" + loca + "&countryRegion=" + CountryCode + "&o=xml&key=" + BingMapsKey;

            //Make the request and get the response
            XmlDocument geocodeResponse = GetXmlResponse(geocodeRequest);

            return (geocodeResponse);
        }

        private static XmlDocument GeocodeByAddressLine(string addressLine)
        {
            //Create REST Services geocode request using Locations API
            string geocodeRequest = "http://dev.virtualearth.net/REST/v1/Locations?addressLine=" + addressLine + "&countryRegion=" + CountryCode + "&o=xml&key=" + BingMapsKey;
            //Make the request and get the response
            XmlDocument geocodeResponse = GetXmlResponse(geocodeRequest);

            return (geocodeResponse);
        }


        // Submit a REST Services or Spatial Data Services request and return the response
        private static XmlDocument GetXmlResponse(string requestUrl)
        {
            System.Diagnostics.Trace.WriteLine("Request URL (XML): " + requestUrl);
            HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception(String.Format("Server error (HTTP {0}: {1}).",
                    response.StatusCode,
                    response.StatusDescription));
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(response.GetResponseStream());
                return xmlDoc;
            }
        }

        private static bool ProcessXmlResponse(XmlDocument xmlDoc, out double longitud, out double latitude)
        {
            longitud = 0;
            latitude = 0;

            //Create namespace manager
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("rest", "http://schemas.microsoft.com/search/local/ws/rest/v1");

            //Get all geocode locations in the response 
            XmlNodeList locationElements = xmlDoc.SelectNodes("//rest:Location", nsmgr);
            if (locationElements.Count == 0)
                return false;

            //Get the geocode location points that are used for display (UsageType=Display)
            XmlNodeList displayGeocodePoints = locationElements[0].SelectNodes(".//rest:GeocodePoint/rest:UsageType[.='Display']/parent::node()", nsmgr);

            string latitudeStr = displayGeocodePoints[0].SelectSingleNode(".//rest:Latitude", nsmgr).InnerText;
            string longitudeStr = displayGeocodePoints[0].SelectSingleNode(".//rest:Longitude", nsmgr).InnerText;

            longitud = double.Parse(longitudeStr);
            latitude = double.Parse(latitudeStr);

            return true;
        }

        public static bool FindLocationByName(string localition, out double longitud, out double latitude)
        {
            //Get location information from geocode response 
            var xmlDoc = Geocode(localition);

            return ProcessXmlResponse(xmlDoc, out longitud, out latitude);
        }

        public static bool FindLocationByAddress(string address, string city, string state, out double longitud, out double latitude)
        {
            //Get location information from geocode response 
            var xmlDoc = GeocodeByAddressLine(string.Format("{0}, {1}, {2}", address, city, state));

            return ProcessXmlResponse(xmlDoc, out longitud, out latitude);
        }

        public static double[] GetLatLongFromImage(string imagePath)
        {
            try
            {
                ExifReader reader = new ExifReader(imagePath);
            
                // EXIF lat/long tags stored as [Degree, Minute, Second]
                double[] latitudeComponents;
                double[] longitudeComponents;

                string latitudeRef; // "N" or "S" ("S" will be negative latitude)
                string longitudeRef; // "E" or "W" ("W" will be a negative longitude)

                if (reader.GetTagValue(ExifTags.GPSLatitude, out latitudeComponents)
                    && reader.GetTagValue(ExifTags.GPSLongitude, out longitudeComponents)
                    && reader.GetTagValue(ExifTags.GPSLatitudeRef, out latitudeRef)
                    && reader.GetTagValue(ExifTags.GPSLongitudeRef, out longitudeRef))
                {

                    var latitude = ConvertDegreeAngleToDouble(latitudeComponents[0], latitudeComponents[1], latitudeComponents[2], latitudeRef);
                    var longitude = ConvertDegreeAngleToDouble(longitudeComponents[0], longitudeComponents[1], longitudeComponents[2], longitudeRef);
                    return new[] { latitude, longitude };
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Trace.WriteLine("No Exif tag information was found on file " + imagePath);
            }
            return null;
        }

        static double ConvertDegreeAngleToDouble(double degrees, double minutes, double seconds, string latLongRef)
        {
            double result = ConvertDegreeAngleToDouble(degrees, minutes, seconds);
            if (latLongRef == "S" || latLongRef == "W")
            {
                result *= -1;
            }
            return result;
        }

        static double ConvertDegreeAngleToDouble(double degrees, double minutes, double seconds)
        {
            return degrees + (minutes / 60) + (seconds / 3600);
        }
    }
}
