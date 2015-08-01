using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Newtonsoft.Json.Linq;
using TwixelAPI;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TwixelAppUniversal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccountPage : Page
    {
        ObservableCollection<AccountListViewBinding> accounts;
        ApplicationDataContainer roamingSettings;
        StorageFolder roamingFolder;
        Dictionary<string, User> validUsers;

        public AccountPage()
        {
            this.InitializeComponent();
            accounts = new ObservableCollection<AccountListViewBinding>();
            validUsers = new Dictionary<string, User>();
            roamingSettings = ApplicationData.Current.RoamingSettings;
            roamingFolder = ApplicationData.Current.RoamingFolder;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            HelperMethods.HideSplitView();
            if (Frame.BackStack.Count > 0)
            {
                if (Frame.BackStack[0].SourcePageType == typeof(FinalConfirmation))
                {
                    HelperMethods.DisableBackButton();
                }
            }
            if (AppConstants.users.Count == 0)
            {
                proceedTextBlock.Text = "Login to Twitch";
            }
            else
            {
                proceedTextBlock.Text = "Choose an account";
            }

            StorageFile usersFile = await roamingFolder.GetFileAsync("Users.json");
            string usersDataString = await FileIO.ReadTextAsync(usersFile);
            if (!string.IsNullOrEmpty(usersDataString))
            {
                JArray usersA = JArray.Parse(usersDataString);
                foreach (JObject userO in usersA)
                {
                    User tmpUser = null;
                    string name = (string)userO["name"];
                    string accessToken = (string)userO["access_token"];
                    Channel userChannel = await AppConstants.Twixel.RetrieveChannel(name);
                    AccountListViewBinding binding = new AccountListViewBinding();
                    binding.DisplayName = userChannel.displayName;
                    binding.Name = userChannel.name;
                    if (userChannel.logo != null)
                    {
                        binding.Image = userChannel.logo.ToString();
                    }
                    try
                    {
                        tmpUser = await AppConstants.Twixel.RetrieveUserWithAccessToken(accessToken);
                        binding.Invalid = false;
                        accounts.Add(binding);
                        validUsers.Add(binding.Name, tmpUser);
                    }
                    catch (TwixelException ex)
                    {
                        if (ex.Message == $"{accessToken} is not a valid access token")
                        {
                            binding.Invalid = true;
                            accounts.Add(binding);
                        }
                    }
                }
            }
            base.OnNavigatedTo(e);
        }

        private void accountsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (accountsListView.SelectedIndex == -1)
            {
                proceedButton.Content = "Login";
                changePermissionsButton.Content = "Change Permissions";
                changePermissionsButton.IsEnabled = false;
            }
            else
            {
                AccountListViewBinding binding = e.AddedItems[0] as AccountListViewBinding;
                if (!binding.Invalid)
                {
                    proceedButton.Content = "Select";
                    changePermissionsButton.Content = "Change Permissions";
                    changePermissionsButton.IsEnabled = true;
                }
                else
                {
                    proceedButton.Content = "Reauthenticate";
                    changePermissionsButton.Content = "Invalid Access Token";
                    changePermissionsButton.IsEnabled = false;
                }
            }
        }

        private void accountsListView_Loaded(object sender, RoutedEventArgs e)
        {
            accountsListView.ItemsSource = accounts;
        }

        private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            accountsListView.SelectedIndex = -1;
        }

        private void proceedButton_Click(object sender, RoutedEventArgs e)
        {
            if (accountsListView.SelectedIndex == -1)
            {
                Frame.Navigate(typeof(ScopesPage));
            }
            else
            {
                AccountListViewBinding binding = accountsListView.SelectedItem as AccountListViewBinding;
                if (!binding.Invalid)
                {
                    roamingSettings.Values["activeUser"] = binding.Name;
                    AppConstants.activeUser = validUsers[binding.Name];
                    Frame.Navigate(typeof(HomePage));
                }
                else
                {
                    Frame.Navigate(typeof(ScopesPage));
                }
            }
        }

        private void changePermissionsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ScopesPage));
        }
    }
}
