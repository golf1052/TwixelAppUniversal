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
using TwixelEmotes;

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
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            await DoEmotesStuff(localFolder);
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
                string activeUser = (string)roamingSettings.Values["activeUser"];
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

        private async Task DoEmotesStuff(StorageFolder folder)
        {
            loadingText.Text = "loading emotes";
            StorageFile globalEmotesFile = await folder.CreateFileAsync("GlobalEmotes.json", CreationCollisionOption.OpenIfExists);
            StorageFile globalTimeFile = await folder.CreateFileAsync("GlobalTime.json", CreationCollisionOption.OpenIfExists);
            StorageFile subscriberEmotesFile = await folder.CreateFileAsync("SubscriberEmotes.json", CreationCollisionOption.OpenIfExists);
            StorageFile subscriberTimeFile = await folder.CreateFileAsync("SubscriberTime.json", CreationCollisionOption.OpenIfExists);
            StorageFile setsEmotesFile = await folder.CreateFileAsync("SetsEmotes.json", CreationCollisionOption.OpenIfExists);
            StorageFile setsTimeFile = await folder.CreateFileAsync("SetsTime.json", CreationCollisionOption.OpenIfExists);
            StorageFile imagesEmotesFile = await folder.CreateFileAsync("ImagesEmotes.json", CreationCollisionOption.OpenIfExists);
            StorageFile imagesTimeFile = await folder.CreateFileAsync("ImagesTime.json", CreationCollisionOption.OpenIfExists);
            StorageFile basic0EmotesFile = await folder.CreateFileAsync("Basic0Emotes.json", CreationCollisionOption.OpenIfExists);
            StorageFile basic33EmotesFile = await folder.CreateFileAsync("Basic33Emotes.json", CreationCollisionOption.OpenIfExists);
            StorageFile basic42EmotesFile = await folder.CreateFileAsync("Basic42Emotes.json", CreationCollisionOption.OpenIfExists);
            StorageFile basicTimeFile = await folder.CreateFileAsync("BasicTime.json", CreationCollisionOption.OpenIfExists);
            EmoteDataCache emoteDataCache = new EmoteDataCache(await FileIO.ReadTextAsync(globalEmotesFile), await FileIO.ReadTextAsync(globalTimeFile),
                await FileIO.ReadTextAsync(subscriberEmotesFile), await FileIO.ReadTextAsync(subscriberTimeFile),
                await FileIO.ReadTextAsync(setsEmotesFile), await FileIO.ReadTextAsync(setsTimeFile),
                await FileIO.ReadTextAsync(imagesEmotesFile), await FileIO.ReadTextAsync(imagesTimeFile),
                await FileIO.ReadTextAsync(basic0EmotesFile), await FileIO.ReadTextAsync(basic33EmotesFile), await FileIO.ReadTextAsync(basic42EmotesFile), await FileIO.ReadTextAsync(basicTimeFile));
            if (string.IsNullOrEmpty(emoteDataCache.GlobalString) || string.IsNullOrEmpty(emoteDataCache.SubscriberString) || string.IsNullOrEmpty(emoteDataCache.SetsString) ||
                string.IsNullOrEmpty(emoteDataCache.ImagesString) || string.IsNullOrEmpty(emoteDataCache.Basic0String) || string.IsNullOrEmpty(emoteDataCache.Basic33String) ||
                string.IsNullOrEmpty(emoteDataCache.Basic42String))
            {
                await EmoteManager.Initialize();
            }
            else
            {
                await EmoteManager.Initialize(emoteDataCache);
            }
            EmoteDataCache newCache = EmoteManager.GetDataCache();
            await FileIO.WriteTextAsync(globalEmotesFile, newCache.GlobalString);
            await FileIO.WriteTextAsync(globalTimeFile, newCache.GlobalTime.Value.ToString("o"));
            await FileIO.WriteTextAsync(subscriberEmotesFile, newCache.SubscriberString);
            await FileIO.WriteTextAsync(subscriberTimeFile, newCache.SubscriberTime.Value.ToString("o"));
            await FileIO.WriteTextAsync(setsEmotesFile, newCache.SetsString);
            await FileIO.WriteTextAsync(setsTimeFile, newCache.SetsTime.Value.ToString("o"));
            await FileIO.WriteTextAsync(imagesEmotesFile, newCache.ImagesString);
            await FileIO.WriteTextAsync(imagesTimeFile, newCache.ImagesTime.Value.ToString("o"));
            await FileIO.WriteTextAsync(basic0EmotesFile, newCache.Basic0String);
            await FileIO.WriteTextAsync(basic33EmotesFile, newCache.Basic33String);
            await FileIO.WriteTextAsync(basic42EmotesFile, newCache.Basic42String);
            await FileIO.WriteTextAsync(basicTimeFile, newCache.BasicTime.Value.ToString("o"));
        }
    }
}
