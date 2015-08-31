using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TwixelAPI;
using TwixelAPI.Constants;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
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
    public sealed partial class VideosPage : Page
    {
        ItemLoader videosLoader;
        ObservableCollection<VideosGridViewBinding> videosCollection;

        public enum Selected
        {
            Week,
            Month,
            AllTime
        }

        Selected selectedPeriod;
        bool finishedLoading = false;

        public VideosPage()
        {
            this.InitializeComponent();
            finishedLoading = true;
            selectedPeriod = Selected.Week;
            videosLoader = new ItemLoader(LoadVideos, scrollViewer, progressBar);
            videosCollection = new ObservableCollection<VideosGridViewBinding>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await LoadVideos();
            base.OnNavigatedTo(e);
        }

        async Task LoadVideos()
        {
            videosLoader.StartLoading();
            DisableButtons();
            List<Video> videos = new List<Video>();
            try
            {
                if (selectedPeriod == Selected.Week)
                {
                    videos = await AppConstants.Twixel.RetrieveTopVideos(null, TwitchConstants.Period.Week, videosLoader.Offset, 100);
                }
                else if (selectedPeriod == Selected.Month)
                {
                    videos = await AppConstants.Twixel.RetrieveTopVideos(null, TwitchConstants.Period.Month, videosLoader.Offset, 100);
                }
                else if (selectedPeriod == Selected.AllTime)
                {
                    videos = await AppConstants.Twixel.RetrieveTopVideos(null, TwitchConstants.Period.All, videosLoader.Offset, 100);
                }
            }
            catch (TwixelException ex)
            {
                await HelperMethods.ShowMessageDialog(string.Format("Looks like there was an error. Here are the details:\nStatus Code: {0}\nError: {1}\nMessage: {2}", ex.Error.Status.ToString(), ex.Error.Error, ex.Error.Message), ex.Message);
            }
            if (!videosLoader.CheckForEnd(videos))
            {
                foreach (Video video in videos)
                {
                    videosCollection.Add(new VideosGridViewBinding(video));
                }
                videosLoader.EndLoading(100);
            }
            EnableButtons();
        }

        private async void videosGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            VideosGridViewBinding videoItem = e.ClickedItem as VideosGridViewBinding;
            LauncherOptions options = new LauncherOptions();
            options.DesiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.UseHalf;
            await Launcher.LaunchUriAsync(new Uri(videoItem.Url), options);
        }

        private void videosGridView_Loaded(object sender, RoutedEventArgs e)
        {
            videosGridView.ItemsSource = videosCollection;
        }

        private async void weekButton_Checked(object sender, RoutedEventArgs e)
        {
            if (finishedLoading)
            {
                selectedPeriod = Selected.Week;
                await Reset();
            }
        }

        private async void monthButton_Checked(object sender, RoutedEventArgs e)
        {
            selectedPeriod = Selected.Month;
            await Reset();
        }

        private async void allTimeButton_Checked(object sender, RoutedEventArgs e)
        {
            selectedPeriod = Selected.AllTime;
            await Reset();
        }

        async Task Reset()
        {
            videosCollection.Clear();
            videosLoader.Reset();
            await LoadVideos();
        }

        void DisableButtons()
        {
            weekButton.IsEnabled = false;
            monthButton.IsEnabled = false;
            allTimeButton.IsEnabled = false;
        }

        void EnableButtons()
        {
            weekButton.IsEnabled = true;
            monthButton.IsEnabled = true;
            allTimeButton.IsEnabled = true;
        }
    }
}
