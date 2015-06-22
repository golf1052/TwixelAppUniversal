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
            ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
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
            base.OnNavigatedTo(e);
        }

        private void page_Loaded(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HomePage));
        }
    }
}
