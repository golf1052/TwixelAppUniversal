using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Flurl;
using Newtonsoft.Json.Linq;

namespace TwixelApp
{
    public class HelperMethods
    {
        public static async Task<string> GetWebData(Uri uri)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(uri);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return string.Empty;
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        public static async Task<Dictionary<AppConstants.StreamQuality, Uri>> RetrieveHlsStream(string channel)
        {
            string response;
            Url url = new Url("https://api.twitch.tv/api").AppendPathSegments("channels", channel, "access_token");
            try
            {
                response = await GetWebData(new Uri(url.ToString()));
            }
            catch (Exception ex)
            {
                throw;
            }

            JObject accessToken = JObject.Parse(response);
            string token = (string)accessToken["token"];
            string sig = (string)accessToken["sig"];
            url = new Url("http://usher.twitch.tv/api")
                .AppendPathSegments("channel", "hls", channel + ".m3u8")
                .SetQueryParams(new
                {
                    token = token,
                    sig = sig,
                    allow_source = true
                });
            try
            {
                response = await GetWebData(new Uri(url.ToString()));
            }
            catch (Exception ex)
            {
                throw;
            }
            string[] lines = response.Split('\n');
            Dictionary<AppConstants.StreamQuality, Uri> qualities = new Dictionary<AppConstants.StreamQuality, Uri>();
            foreach (string line in lines)
            {
                if (line.Contains("/source/"))
                {
                    qualities.Add(AppConstants.StreamQuality.Source, new Uri(line));
                }
                else if (line.Contains("/high/"))
                {
                    qualities.Add(AppConstants.StreamQuality.High, new Uri(line));
                }
                else if (line.Contains("/medium/"))
                {
                    qualities.Add(AppConstants.StreamQuality.Medium, new Uri(line));
                }
                else if (line.Contains("/low/"))
                {
                    qualities.Add(AppConstants.StreamQuality.Low, new Uri(line));
                }
                else if (line.Contains("/mobile/"))
                {
                    qualities.Add(AppConstants.StreamQuality.Mobile, new Uri(line));
                }
                else if (line.Contains("/chunked/"))
                {
                    qualities.Add(AppConstants.StreamQuality.Chunked, new Uri(line));
                }
            }
            return qualities;
        }
    }
}
