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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TwixelAppUniversal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppShell : Page
    {
        public static AppShell Current = null;
        public Frame AppFrame { get { return frame; } }
        private SolidColorBrush buttonColor;

        public AppShell()
        {
            this.InitializeComponent();
            AppConstants.RootSplitView = RootSplitView;

            this.Loaded += AppShell_Loaded;
            buttonColor = (SolidColorBrush)homeButton.Background;
        }

        private void AppShell_Loaded(object sender, RoutedEventArgs e)
        {
            Current = this;
        }

        private void togglePaneButton_Click(object sender, RoutedEventArgs e)
        {
            RootSplitView.IsPaneOpen = !RootSplitView.IsPaneOpen;
        }

        private void homeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(AppFrame.Content is HomePage))
            {
                AppFrame.Navigate(typeof(HomePage));
            }
        }

        private void streamsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(AppFrame.Content is StreamsPage))
            {
                AppFrame.Navigate(typeof(StreamsPage));
            }
        }

        private void gamesButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(AppFrame.Content is GamesPage))
            {
                AppFrame.Navigate(typeof(GamesPage));
            }
        }

        private void userButton_Click(object sender, RoutedEventArgs e)
        {
            if (AppConstants.activeUser != null)
            {
                if (!(AppFrame.Content is UserPage))
                {
                    AppFrame.Navigate(typeof(UserPage));
                }
            }
            else
            {
                if (!(AppFrame.Content is AccountPage))
                {
                    AppFrame.Navigate(typeof(AccountPage));
                }
            }
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(AppFrame.Content is SettingsPage))
            {
                AppFrame.Navigate(typeof(SettingsPage));
            }
        }

        private void RootSplitView_PaneClosed(SplitView sender, object args)
        {
            if (togglePaneButton.IsChecked.HasValue)
            {
                if (togglePaneButton.IsChecked.Value)
                {
                    togglePaneButton.IsChecked = false;
                }
            }
        }

        private void frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            ResetButtons();
            if (e.SourcePageType == typeof(HomePage))
            {
                homeButton.Background = AppConstants.ThemeColor;
            }
            else if (e.SourcePageType == typeof(StreamsPage))
            {
                streamsButton.Background = AppConstants.ThemeColor;
            }
            else if (e.SourcePageType == typeof(GamesPage))
            {
                gamesButton.Background = AppConstants.ThemeColor;
            }
            else if (e.SourcePageType == typeof(UserPage))
            {
                userButton.Background = AppConstants.ThemeColor;
            }
            else if (e.SourcePageType == typeof(SettingsPage))
            {
                settingsButton.Background = AppConstants.ThemeColor;
            }
        }

        private void ResetButtons()
        {
            homeButton.Background = buttonColor;
            streamsButton.Background = buttonColor;
            gamesButton.Background = buttonColor;
            userButton.Background = buttonColor;
            settingsButton.Background = buttonColor;
        }

        private void homeTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            homeButton_Click(sender, e);
        }

        private void streamsTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            streamsButton_Click(sender, e);
        }

        private void gamesTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gamesButton_Click(sender, e);
        }

        private void userTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            userButton_Click(sender, e);
        }

        private void settingsTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            settingsButton_Click(sender, e);
        }
    }
}
