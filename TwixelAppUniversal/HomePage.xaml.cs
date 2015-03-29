using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using TwixelAPI;
using Windows.UI.Xaml.Media.Imaging;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Media.Playback;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TwixelApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        ObservableCollection<GameGridViewBinding> gamesCollection;
        List<FeaturedStream> streams;
        List<Dictionary<AppConstants.StreamQuality, Uri>> qualities;
        int selectedStreamIndex = 0;

        public HomePage()
        {
            this.InitializeComponent();

            gamesCollection = new ObservableCollection<GameGridViewBinding>();
            streams = new List<FeaturedStream>();
            qualities = new List<Dictionary<AppConstants.StreamQuality, Uri>>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            streams = await AppConstants.twixel.RetrieveFeaturedStreams(0, 10);
            foreach (FeaturedStream stream in streams)
            {
                Dictionary<AppConstants.StreamQuality, Uri> q = null;
                try
                {
                    q = await HelperMethods.RetrieveHlsStream(stream.stream.channel.name);
                }
                catch (Exception ex)
                {
                }
                if (q != null)
                {
                    qualities.Add(q);
                }
            }
            streamPreviewImage.Source = new BitmapImage(streams[0].stream.previewList["large"]);
            playButton.IsEnabled = true;
            nextButton.IsEnabled = true;
            Total<List<Game>> games = await AppConstants.twixel.RetrieveTopGames();
            foreach (Game game in games.wrapped)
            {
                gamesCollection.Add(new GameGridViewBinding(game.name, game.box["large"]));
            }
            base.OnNavigatedTo(e);
        }

        private void gamesGridView_Loaded(object sender, RoutedEventArgs e)
        {
            gamesGridView.ItemsSource = gamesCollection;
        }

        private void prevButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedStreamIndex > 0)
            {
                if (featuredStreamMediaElement.CurrentState != MediaElementState.Paused &&
                    featuredStreamMediaElement.CurrentState != MediaElementState.Stopped &&
                    featuredStreamMediaElement.CurrentState != MediaElementState.Closed)
                {
                    featuredStreamMediaElement.Stop();
                    featuredStreamMediaElement.Source = null;
                }
                selectedStreamIndex--;
                streamPreviewImage.Source = new BitmapImage(streams[selectedStreamIndex].stream.previewList["large"]);
                nextButton.IsEnabled = true;
                if (selectedStreamIndex == 0)
                {
                    prevButton.IsEnabled = false;
                }
            }
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (featuredStreamMediaElement.CurrentState != MediaElementState.Paused &&
                featuredStreamMediaElement.CurrentState != MediaElementState.Stopped &&
                featuredStreamMediaElement.CurrentState != MediaElementState.Closed)
            {
                featuredStreamMediaElement.Pause();
            }
            else if (featuredStreamMediaElement.CurrentState == MediaElementState.Paused)
            {
                featuredStreamMediaElement.Play();
            }
            else
            {
                streamPreviewImage.Source = null;
                var selectedQuality = qualities[selectedStreamIndex];
                if (selectedQuality.ContainsKey(AppConstants.StreamQuality.Mobile))
                {
                    featuredStreamMediaElement.Source = selectedQuality[AppConstants.StreamQuality.Mobile];
                }
                else
                {
                    featuredStreamMediaElement.Source = selectedQuality[AppConstants.StreamQuality.Chunked];
                }
                featuredStreamMediaElement.Play();
            }
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedStreamIndex < streams.Count - 1)
            {
                if (featuredStreamMediaElement.CurrentState != MediaElementState.Paused &&
                    featuredStreamMediaElement.CurrentState != MediaElementState.Stopped &&
                    featuredStreamMediaElement.CurrentState != MediaElementState.Closed)
                {
                    featuredStreamMediaElement.Stop();
                    featuredStreamMediaElement.Source = null;
                }
                selectedStreamIndex++;
                streamPreviewImage.Source = new BitmapImage(streams[selectedStreamIndex].stream.previewList["large"]);
                prevButton.IsEnabled = true;
                if (selectedStreamIndex == streams.Count - 1)
                {
                    nextButton.IsEnabled = false;
                }
            }
        }

        private void channelButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(channelTextBox.Text))
            {
                List<object> parameters = new List<object>();
                parameters.Add(channelTextBox.Text);
                Frame.Navigate(typeof(StreamPage), parameters);
            }
        }
    }
}
