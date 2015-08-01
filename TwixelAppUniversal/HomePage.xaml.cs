using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TwixelAPI;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TwixelAppUniversal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        List<FeaturedStream> streams;
        List<Dictionary<AppConstants.StreamQuality, Uri>> qualities;
        int selectedStreamIndex = 0;

        public HomePage()
        {
            this.InitializeComponent();
            streams = new List<FeaturedStream>();
            qualities = new List<Dictionary<AppConstants.StreamQuality, Uri>>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Frame.BackStack.Count > 0)
            {
                if (Frame.BackStack[0].SourcePageType == typeof(LoadingPage) ||
                    Frame.BackStack[0].SourcePageType == typeof(FinalConfirmation))
                {
                    HelperMethods.DisableBackButton();
                }
            }

            streams = await AppConstants.Twixel.RetrieveFeaturedStreams(0, 10);
            foreach (FeaturedStream stream in streams)
            {
                stream.CleanTextString();
                stream.text = stream.text.Replace('\n', ' ').Trim();
                Dictionary<AppConstants.StreamQuality, Uri> q = null;
                try
                {
                    q = await HelperMethods.RetrieveHlsStream(stream.stream.channel.name);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                if (q != null)
                {
                    qualities.Add(q);
                }
                else
                {
                    qualities.Add(q);
                }
            }
            SetUpFeaturedStream();
            playButton.IsEnabled = true;
            nextButton.IsEnabled = true;
            base.OnNavigatedTo(e);
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
                SetUpFeaturedStream();
                nextButton.IsEnabled = true;
                if (selectedStreamIndex == 0)
                {
                    prevButton.IsEnabled = false;
                }
                playPauseIcon.Symbol = Symbol.Play;
            }
        }

        private void streamButton_Click(object sender, RoutedEventArgs e)
        {
            streamPreviewImage.Source = null;
            List<object> parameters = new List<object>();
            parameters.Add(streams[selectedStreamIndex].stream);
            parameters.Add(qualities[selectedStreamIndex]);
            Frame.Navigate(typeof(StreamPage), parameters);
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (featuredStreamMediaElement.CurrentState != MediaElementState.Paused &&
                featuredStreamMediaElement.CurrentState != MediaElementState.Stopped &&
                featuredStreamMediaElement.CurrentState != MediaElementState.Closed)
            {
                featuredStreamMediaElement.Stop();
                playPauseIcon.Symbol = Symbol.Play;
            }
            else if (featuredStreamMediaElement.CurrentState == MediaElementState.Paused ||
                featuredStreamMediaElement.CurrentState == MediaElementState.Stopped)
            {
                featuredStreamMediaElement.Play();
                playPauseIcon.Symbol = Symbol.Stop;
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
                playPauseIcon.Symbol = Symbol.Stop;
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
                SetUpFeaturedStream();
                prevButton.IsEnabled = true;
                if (selectedStreamIndex == streams.Count - 1)
                {
                    nextButton.IsEnabled = false;
                }
                playPauseIcon.Symbol = Symbol.Play;
            }
        }

        private void SetUpFeaturedStream()
        {
            streamButton.IsEnabled = true;
            playButton.IsEnabled = true;
            streamOfflineTextBlock.Visibility = Visibility.Collapsed;
            streamPreviewImage.Source = 
            streamPreviewImage.Source = new BitmapImage(streams[selectedStreamIndex].stream.previewList["large"]);
            if (streams[selectedStreamIndex].stream.channel.logo != null)
            {
                featuredStreamerImage.Fill = new ImageBrush() { ImageSource = new BitmapImage(streams[selectedStreamIndex].stream.channel.logo) };
            }
            featuredGameTitle.Text = streams[selectedStreamIndex].title;
            featuredGameDescription.Text = streams[selectedStreamIndex].text;
            if (qualities[selectedStreamIndex] == null)
            {
                streamButton.IsEnabled = false;
                playButton.IsEnabled = false;
                streamPreviewImage.Source = null;
                streamOfflineTextBlock.Visibility = Visibility.Visible;
            }
        }
    }
}
