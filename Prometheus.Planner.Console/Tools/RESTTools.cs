using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            try
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
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static Stream DownloadImage(Uri uri, string savePath)
        {
            try
            {
                var request = (HttpWebRequest)HttpWebRequest.Create(uri);

                request.Method = "GET";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.139 Safari/537.36";
                request.Timeout = 30 * 60 * 1000;
                request.UseDefaultCredentials = true;
                request.Proxy.Credentials = request.Credentials;

                var response = request.GetResponse();
                using (var stream = response.GetResponseStream())
                {
                    Byte[] buffer = new Byte[response.ContentLength];
                    int offset = 0, actuallyRead = 0;
                    do
                    {
                        actuallyRead = stream.Read(buffer, offset, buffer.Length - offset);
                        offset += actuallyRead;
                    }
                    while (actuallyRead > 0);
                    if (!string.IsNullOrEmpty(savePath))
                        File.WriteAllBytes(savePath, buffer);
                    return new MemoryStream(buffer);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error downloading maps: " + ex.Message);
                return null;
            }
        }
    }
}
