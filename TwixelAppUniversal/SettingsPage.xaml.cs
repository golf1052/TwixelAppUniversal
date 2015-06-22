using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class SettingsPage : Page
    {
        bool doneLoading = false;

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            wifiStreamQualityComboBox.SelectedIndex = (int)AppConstants.WifiStreamQuality;
            base.OnNavigatedTo(e);
            doneLoading = true;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void wifiStreamQualityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (doneLoading)
            {
                ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
                AppConstants.WifiStreamQuality = (AppConstants.StreamQuality)wifiStreamQualityComboBox.SelectedIndex;
                roamingSettings.Values["wifiStreamQuality"] = HelperMethods.GetStreamQualityString(AppConstants.WifiStreamQuality);
            }
        }
    }
}
