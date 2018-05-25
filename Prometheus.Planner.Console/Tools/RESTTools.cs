using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Prometeo.Planner.Console.Tools
{
    public static class RESTTools
    {
        public static bool SimpleGet(string endpoint, params string[] args)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(endpoint);
                var response = client.PostAsync(string.Format(endpoint, args), null).Result;
                if (!response.IsSuccessStatusCode)
                    return false;

                return true;
            }
        }

        public static string Get(string endpoint, params string[] args)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(string.Format(endpoint, args));
            request.Method = "GET";
            request.Accept = "application/json";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.139 Safari/537.36";
            var webResponse = request.GetResponse();

            // Process response
            using (StreamReader responseReader = new StreamReader(webResponse.GetResponseStream()))
            {
                return responseReader.ReadToEnd();
            }
        }
    }
}
