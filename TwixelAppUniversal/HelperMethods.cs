using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Newtonsoft.Json.Linq;
using TwixelAPI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace TwixelAppUniversal
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

        public static AppConstants.StreamQuality GetStreamQuality(string streamQuality)
        {
            streamQuality = streamQuality.ToLower();
            if (streamQuality == "source")
            {
                return AppConstants.StreamQuality.Source;
            }
            else if (streamQuality == "high")
            {
                return AppConstants.StreamQuality.High;
            }
            else if (streamQuality == "medium")
            {
                return AppConstants.StreamQuality.Medium;
            }
            else if (streamQuality == "low")
            {
                return AppConstants.StreamQuality.Low;
            }
            else if (streamQuality == "mobile")
            {
                return AppConstants.StreamQuality.Mobile;
            }
            else if (streamQuality == "chunked")
            {
                return AppConstants.StreamQuality.Chunked;
            }
            else
            {
                return AppConstants.StreamQuality.Chunked;
            }
        }

        public static string GetStreamQualityString(AppConstants.StreamQuality streamQuality)
        {
            if (streamQuality == AppConstants.StreamQuality.Source)
            {
                return "Source";
            }
            else if (streamQuality == AppConstants.StreamQuality.High)
            {
                return "High";
            }
            else if (streamQuality == AppConstants.StreamQuality.Medium)
            {
                return "Medium";
            }
            else if (streamQuality == AppConstants.StreamQuality.Low)
            {
                return "Low";
            }
            else if (streamQuality == AppConstants.StreamQuality.Mobile)
            {
                return "Mobile";
            }
            else if (streamQuality == AppConstants.StreamQuality.Chunked)
            {
                return "Chunked";
            }
            else
            {
                return string.Empty;
            }
        }

        public static async Task ShowMessageDialog(string message, string title = "")
        {
            MessageDialog messageDialog = new MessageDialog(message, title);
            await messageDialog.ShowAsync();
        }

        public static void HideSplitView()
        {
            AppConstants.RootSplitView.DisplayMode = SplitViewDisplayMode.Overlay;
        }

        public static void ShowSplitView()
        {
            AppConstants.RootSplitView.DisplayMode = SplitViewDisplayMode.CompactOverlay;
        }
    }
}
