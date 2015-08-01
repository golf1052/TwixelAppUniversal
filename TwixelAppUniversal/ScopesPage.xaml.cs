using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TwixelAPI.Constants;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class ScopesPage : Page
    {
        public ScopesPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HelperMethods.HideSplitView();
            HelperMethods.DisableBackButton();
            base.OnNavigatedTo(e);
        }

        private async void userReadButton_Click(object sender, RoutedEventArgs e)
        {
            await HelperMethods.ShowMessageDialog(
@"Lets us get your user information (this includes your email).
Lets us get the streams you follow.
You really should leave this enabled.
We promise not to do anything bad with this.");
        }

        private async void userBlocksEditButton_Click(object sender, RoutedEventArgs e)
        {
            await HelperMethods.ShowMessageDialog("You can block people. Then you can unblock them.");
        }

        private async void userBlocksReadButton_Click(object sender, RoutedEventArgs e)
        {
            await HelperMethods.ShowMessageDialog("You can view your ignore list.");
        }

        private async void userFollowsEditButton_Click(object sender, RoutedEventArgs e)
        {
            await HelperMethods.ShowMessageDialog("You can follow channels. You can unfollow channels.\nThese are all pretty straight forward.");
        }

        private async void channelReadButton_Click(object sender, RoutedEventArgs e)
        {
            await HelperMethods.ShowMessageDialog(
@"Lets us view your email.
Lets us get your stream key.");
        }

        private async void channelEditorButton_Click(object sender, RoutedEventArgs e)
        {
            await HelperMethods.ShowMessageDialog("You can update your status and what game you're playing.");
        }

        private async void channelCommercialButton_Click(object sender, RoutedEventArgs e)
        {
            await HelperMethods.ShowMessageDialog("Must be partnered on Twitch to use this.");
        }

        private async void channelStreamButton_Click(object sender, RoutedEventArgs e)
        {
            await HelperMethods.ShowMessageDialog("You can reset your stream key. We won't reset your stream key unless you really want to.");
        }

        private async void channelSubscriptionsButton_Click(object sender, RoutedEventArgs e)
        {
            await HelperMethods.ShowMessageDialog("Must be partnered on Twitch to use this.");
        }

        private async void userSubscriptionsButton_Click(object sender, RoutedEventArgs e)
        {
            await HelperMethods.ShowMessageDialog("Ability to view subscriptions.");
        }

        private async void channelCheckSubscriptionButton_Click(object sender, RoutedEventArgs e)
        {
            await HelperMethods.ShowMessageDialog("Must be partnered on Twitch to use this.");
        }

        private async void chatLoginButton_Click(object sender, RoutedEventArgs e)
        {
            await HelperMethods.ShowMessageDialog("Ability to login to chat and send messages.");
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            List<TwitchConstants.Scope> scopes = new List<TwitchConstants.Scope>();
            if (userReadCheckbox.IsChecked.Value)
            {
                scopes.Add(TwitchConstants.Scope.UserRead);
            }
            if (userBlocksEditCheckbox.IsChecked.Value)
            {
                scopes.Add(TwitchConstants.Scope.UserBlocksEdit);
            }
            if (userBlocksReadCheckbox.IsChecked.Value)
            {
                scopes.Add(TwitchConstants.Scope.UserBlocksRead);
            }
            if (userFollowsEditCheckbox.IsChecked.Value)
            {
                scopes.Add(TwitchConstants.Scope.UserFollowsEdit);
            }
            if (channelReadCheckbox.IsChecked.Value)
            {
                scopes.Add(TwitchConstants.Scope.ChannelRead);
            }
            if (channelEditorCheckbox.IsChecked.Value)
            {
                scopes.Add(TwitchConstants.Scope.ChannelEditor);
            }
            if (channelCommercialCheckbox.IsChecked.Value)
            {
                scopes.Add(TwitchConstants.Scope.ChannelCommercial);
            }
            if (channelStreamCheckbox.IsChecked.Value)
            {
                scopes.Add(TwitchConstants.Scope.ChannelStream);
            }
            if (channelSubscriptionsCheckbox.IsChecked.Value)
            {
                scopes.Add(TwitchConstants.Scope.ChannelSubscriptions);
            }
            if (userSubscriptionsCheckbox.IsChecked.Value)
            {
                scopes.Add(TwitchConstants.Scope.UserSubcriptions);
            }
            if (channelCheckSubscriptionCheckbox.IsChecked.Value)
            {
                scopes.Add(TwitchConstants.Scope.ChannelCheckSubscription);
            }
            if (chatLoginCheckbox.IsChecked.Value)
            {
                scopes.Add(TwitchConstants.Scope.ChatLogin);
            }
            Frame.Navigate(typeof(FinalConfirmation), scopes);
        }
    }
}
