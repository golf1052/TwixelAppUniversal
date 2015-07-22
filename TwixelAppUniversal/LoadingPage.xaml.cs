using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Newtonsoft.Json.Linq;
using TwixelAPI;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TwixelAppUniversal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoadingPage : Page
    {
        public LoadingPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private async void page_Loaded(object sender, RoutedEventArgs e)
        {
            HelperMethods.HideSplitView();
            ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
            StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
            loadingText.Text = "loading settings";
            if (!roamingSettings.Values.ContainsKey("wifiStreamQuality"))
            {
                roamingSettings.Values["wifiStreamQuality"] = "Medium";
            }
            if (!roamingSettings.Values.ContainsKey("cellStreamQuality"))
            {
                roamingSettings.Values["cellStreamQuality"] = "Mobile";
            }
            AppConstants.WifiStreamQuality = HelperMethods.GetStreamQuality((string)roamingSettings.Values["wifiStreamQuality"]);
            AppConstants.CellStreamQuality = HelperMethods.GetStreamQuality((string)roamingSettings.Values["cellStreamQuality"]);
            StorageFile usersFile = await roamingFolder.CreateFileAsync("Users.json", CreationCollisionOption.OpenIfExists);
            string usersData = await FileIO.ReadTextAsync(usersFile);
            if (string.IsNullOrEmpty(usersData))
            {
                roamingSettings.Values["activeUser"] = string.Empty;
                AppConstants.activeUser = null;
                HelperMethods.ShowSplitView();
                Frame.Navigate(typeof(HomePage));
            }
            else
            {
                JArray usersA = JArray.Parse(usersData);
                Dictionary<string, User> tmpUsers = new Dictionary<string, User>();
                foreach (JObject userO in usersA)
                {
                    User tmpUser = null;
                    string name = (string)userO["name"];
                    string accessToken = (string)userO["access_token"];
                    loadingText.Text = "loading " + name;
                    try
                    {
                        tmpUser = await AppConstants.Twixel.RetrieveUserWithAccessToken(accessToken);
                    }
                    catch (TwixelException ex)
                    {
                        if (ex.Message == $"{accessToken} is not a valid access token")
                        {
                            await HelperMethods.ShowMessageDialog($"The access token for {name} isn't currently valid.\nYou can fix this later in the settings menu.");
                        }
                        else
                        {
                            await HelperMethods.ShowMessageDialog($"Something went wrong when trying to talk to Twitch. You should probably try again later. Here's the error:\n{ex.Message}");
                        }
                    }
                    if (tmpUser == null)
                    {
                        AppConstants.users.Add(name, string.Empty);
                    }
                    else
                    {
                        tmpUsers.Add(name, tmpUser);
                        AppConstants.users.Add(name, accessToken);
                    }
                }
                if (!roamingSettings.Values.ContainsKey("activeUser"))
                {
                    roamingSettings.Values["activeUser"] = string.Empty;
                }
                string activeUser = (string)roamingSettings.Values["aciveUser"];
                if (!string.IsNullOrEmpty(activeUser))
                {
                    if (AppConstants.users.ContainsKey(activeUser))
                    {
                        if (!string.IsNullOrEmpty(AppConstants.users[activeUser]))
                        {
                            AppConstants.activeUser = tmpUsers[activeUser];
                            Frame.Navigate(typeof(HomePage));
                        }
                        else
                        {
                            // need to choose a new active user
                        }
                    }
                    else
                    {
                        // need to choose a new active user
                    }
                }
                else
                {
                    // need to choose a new active user
                }
            }
        }
    }
}
