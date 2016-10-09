using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Newtonsoft.Json.Linq;
using TwixelAPI;
using Windows.Networking.Connectivity;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace TwixelAppUniversal
{
    public class HelperMethods
    {
        public static async Task<string> GetWebData(Uri uri)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Client-ID", Secrets.ClientId);
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

        public static async Task ShowErrorDialog(TwixelException ex)
        {
            if (ex.Error != null)
            {
                await ShowMessageDialog(string.Format("Message: {0}\n\nError: {1}\n\nStatus Code: {2}", ex.Message, ex.Error.Error, ex.Error.Status.ToString()));
            }
            else
            {
                await ShowMessageDialog(string.Format("Message: {0}", ex.Message), ex.Message);
            }
        }

        public static void HideSplitView()
        {
            AppConstants.RootSplitView.DisplayMode = SplitViewDisplayMode.Overlay;
        }

        public static void ShowSplitView()
        {
            AppConstants.RootSplitView.DisplayMode = SplitViewDisplayMode.CompactOverlay;
        }

        public static void EnableBackButton()
        {
            SystemNavigationManager currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }

        public static void DisableBackButton()
        {
            SystemNavigationManager currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        public static void EnableIndeterminateProgressBar(ProgressBar progressBar)
        {
            progressBar.IsIndeterminate = true;
            progressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        public static void DisableIndeterminateProgressBar(ProgressBar progressBar)
        {
            progressBar.IsIndeterminate = false;
            progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        public static async Task GoToStreamPage(ItemClickEventArgs e, Frame frame)
        {
            GameStreamsGridViewBinding streamItem = e.ClickedItem as GameStreamsGridViewBinding;
            List<object> parameters = new List<object>();
            parameters.Add(streamItem.stream);
            Dictionary<AppConstants.StreamQuality, Uri> qualities = await RetrieveHlsStream(streamItem.stream.channel.name);
            parameters.Add(qualities);
            frame.Navigate(typeof(StreamPage), parameters);
        }

        public static async Task<Uri> GetPreferredQuality(Dictionary<AppConstants.StreamQuality, Uri> qualities)
        {
            AppConstants.NetworkConnectionType connection = await DetermineInternetConnection();
            AppConstants.StreamQuality preferredQuality = AppConstants.StreamQuality.Mobile;
            if (connection == AppConstants.NetworkConnectionType.Cellular)
            {
                preferredQuality = AppConstants.CellStreamQuality;
            }
            else if (connection == AppConstants.NetworkConnectionType.WiFi)
            {
                preferredQuality = AppConstants.WifiStreamQuality;
            }

            if (preferredQuality == AppConstants.StreamQuality.Source)
            {
                if (qualities.ContainsKey(AppConstants.StreamQuality.Source))
                {
                    return qualities[AppConstants.StreamQuality.Source];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.Chunked))
                {
                    return qualities[AppConstants.StreamQuality.Chunked];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.High))
                {
                    return qualities[AppConstants.StreamQuality.High];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.Medium))
                {
                    return qualities[AppConstants.StreamQuality.Medium];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.Low))
                {
                    return qualities[AppConstants.StreamQuality.Low];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.Mobile))
                {
                    return qualities[AppConstants.StreamQuality.Mobile];
                }
            }
            else if (preferredQuality == AppConstants.StreamQuality.High)
            {
                if (qualities.ContainsKey(AppConstants.StreamQuality.High))
                {
                    return qualities[AppConstants.StreamQuality.High];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.Chunked))
                {
                    return qualities[AppConstants.StreamQuality.Chunked];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.Source))
                {
                    return qualities[AppConstants.StreamQuality.Source];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.Medium))
                {
                    return qualities[AppConstants.StreamQuality.Medium];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.Low))
                {
                    return qualities[AppConstants.StreamQuality.Low];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.Mobile))
                {
                    return qualities[AppConstants.StreamQuality.Mobile];
                }
            }
            else if (preferredQuality == AppConstants.StreamQuality.Medium)
            {
                if (qualities.ContainsKey(AppConstants.StreamQuality.Medium))
                {
                    return qualities[AppConstants.StreamQuality.Medium];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.High))
                {
                    return qualities[AppConstants.StreamQuality.High];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.Chunked))
                {
                    return qualities[AppConstants.StreamQuality.Chunked];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.Source))
                {
                    return qualities[AppConstants.StreamQuality.Source];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.Low))
                {
                    return qualities[AppConstants.StreamQuality.Low];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.Mobile))
                {
                    return qualities[AppConstants.StreamQuality.Mobile];
                }
            }
            else if (preferredQuality == AppConstants.StreamQuality.Low)
            {
                if (qualities.ContainsKey(AppConstants.StreamQuality.Low))
                {
                    return qualities[AppConstants.StreamQuality.Low];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.Mobile))
                {
                    return qualities[AppConstants.StreamQuality.Mobile];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.Medium))
                {
                    return qualities[AppConstants.StreamQuality.Medium];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.High))
                {
                    return qualities[AppConstants.StreamQuality.High];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.Chunked))
                {
                    return qualities[AppConstants.StreamQuality.Chunked];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.Source))
                {
                    return qualities[AppConstants.StreamQuality.Source];
                }
            }
            else if (preferredQuality == AppConstants.StreamQuality.Mobile)
            {
                if (qualities.ContainsKey(AppConstants.StreamQuality.Mobile))
                {
                    return qualities[AppConstants.StreamQuality.Mobile];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.Low))
                {
                    return qualities[AppConstants.StreamQuality.Low];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.Medium))
                {
                    return qualities[AppConstants.StreamQuality.Medium];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.High))
                {
                    return qualities[AppConstants.StreamQuality.High];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.Chunked))
                {
                    return qualities[AppConstants.StreamQuality.Chunked];
                }
                else if (qualities.ContainsKey(AppConstants.StreamQuality.Source))
                {
                    return qualities[AppConstants.StreamQuality.Source];
                }
            }
            return qualities[AppConstants.StreamQuality.Chunked];
        }

        public static async Task<AppConstants.NetworkConnectionType> DetermineInternetConnection()
        {
            ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            NetworkConnectivityLevel level = connectionProfile.GetNetworkConnectivityLevel();

            if (level != NetworkConnectivityLevel.InternetAccess)
            {
                await ShowMessageDialog("You are not connected to the internet. Check your connection settings.", "No Internet Connection");
                throw new Exception("No internet connection");
            }

            if (connectionProfile.IsWwanConnectionProfile)
            {
                return AppConstants.NetworkConnectionType.Cellular;
            }
            else if (connectionProfile.IsWlanConnectionProfile)
            {
                return AppConstants.NetworkConnectionType.WiFi;
            }
            else
            {
                return AppConstants.NetworkConnectionType.WiFi;
            }
        }
    }
}
