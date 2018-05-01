using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Prometeo.Planner.Console.Tools
{
    public static class RESTTools
    {
        public static bool SimpleURLRequest(string endpoint, params string[] args)
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
    }
}
