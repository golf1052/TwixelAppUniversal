using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Newtonsoft.Json.Linq;
using TwixelAPI;
using TwixelAPI.Constants;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TwixelAppUniversal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FinalConfirmation : Page
    {
        List<TwitchConstants.Scope> scopes;

        public FinalConfirmation()
        {
            this.InitializeComponent();

            HelperMethods.HideSplitView();
            SystemNavigationManager currentView = SystemNavigationManager.GetForCurrentView();
            currentView.BackRequested += CurrentView_BackRequested;
        }

        private void CurrentView_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame.GoBack();
            e.Handled = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            scopes = e.Parameter as List<TwitchConstants.Scope>;
            Uri authPageUri = AppConstants.Twixel.Login(scopes);
            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            HttpCookieManager cookieManager = filter.CookieManager;
            HttpCookieCollection cookies = cookieManager.GetCookies(authPageUri);
            foreach (HttpCookie cookie in cookies)
            {
                cookieManager.DeleteCookie(cookie);
            }
            webView.Navigate(authPageUri);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            SystemNavigationManager currentView = SystemNavigationManager.GetForCurrentView();
            currentView.BackRequested -= CurrentView_BackRequested;
            base.OnNavigatingFrom(e);
        }

        private async void webView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (webView.Source.Host == "golf1052.com")
            {
                if (webView.Source.Query != "?error=access_denied&error_description=The+user+denied+you+access")
                {
                    webView.Visibility = Visibility.Collapsed;
                    string[] splitString = webView.Source.Fragment.Split('=');
                    string[] secondSplitString = splitString[1].Split('&');
                    string[] scopes = splitString[2].Split('+');
                    List<TwitchConstants.Scope> authorizedScopes = new List<TwitchConstants.Scope>();
                    foreach (string scope in scopes)
                    {
                        authorizedScopes.Add(TwitchConstants.StringToScope(scope));
                    }
                    User user = null;
                    try
                    {
                        user = await AppConstants.Twixel.RetrieveUserWithAccessToken(secondSplitString[0]);
                    }
                    catch (TwixelException ex)
                    {
                        if (ex.Message == TwitchConstants.twitchAPIErrorString)
                        {
                            await HelperMethods.ShowMessageDialog("There was a problem trying to authenticate. Here's the error:\n" + ex.Error.Error);
                        }
                        else
                        {
                            await HelperMethods.ShowMessageDialog(ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        await HelperMethods.ShowMessageDialog("There was a general error.\n" + ex.Message);
                    }
                    finally
                    {
                        Frame.Navigate(typeof(HomePage));
                    }

                    ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
                    StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
                    StorageFile usersFile = await roamingFolder.GetFileAsync("Users.json");
                    string usersDataString = await FileIO.ReadTextAsync(usersFile);

                    JObject userO = new JObject();
                    userO["name"] = user.name;
                    userO["access_token"] = user.accessToken;

                    if (string.IsNullOrEmpty(usersDataString))
                    {
                        JArray usersA = new JArray();
                        usersA.Add(userO);
                        await FileIO.WriteTextAsync(usersFile, usersA.ToString());
                    }
                    else
                    {
                        JArray usersA = JArray.Parse(usersDataString);
                        bool foundUser = false;
                        foreach (JObject fileUser in usersA)
                        {
                            if ((string)fileUser["name"] == user.name)
                            {
                                fileUser["access_token"] = user.accessToken;
                                foundUser = true;
                                break;
                            }
                        }
                        if (!foundUser)
                        {
                            usersA.Add(userO);
                        }
                        await FileIO.WriteTextAsync(usersFile, usersA.ToString());
                    }

                    roamingSettings.Values["activeUser"] = user.name;
                    AppConstants.activeUser = user;
                    Frame.Navigate(typeof(HomePage));
                }
                else
                {
                    Frame.Navigate(typeof(HomePage));
                }
            }
        }
    }
}
