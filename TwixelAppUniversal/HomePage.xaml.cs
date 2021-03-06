﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TwixelAPI;
using TwixelAPI.Constants;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

        ObservableCollection<GameGridViewBinding> topGamesCollection;

        public HomePage()
        {
            this.InitializeComponent();
            streams = new List<FeaturedStream>();
            qualities = new List<Dictionary<AppConstants.StreamQuality, Uri>>();
            topGamesCollection = new ObservableCollection<GameGridViewBinding>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Frame.BackStack.Count > 0)
            {
                if (Frame.BackStack[Frame.BackStack.Count - 1].SourcePageType == typeof(LoadingPage) ||
                    Frame.BackStack[Frame.BackStack.Count - 1].SourcePageType == typeof(FinalConfirmation))
                {
                    HelperMethods.DisableBackButton();
                }
            }

            progressRing.IsActive = true;

            try
            {
                streams = await AppConstants.Twixel.RetrieveFeaturedStreams(0, 10);
            }
            catch (TwixelException ex)
            {
                await HelperMethods.ShowErrorDialog(ex);
            }
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

            if (streams.Count > 0)
            {
                SetUpFeaturedStream();
                streamButton.IsEnabled = true;
                playButton.IsEnabled = true;
                nextButton.IsEnabled = true;
            }

            Total<List<Game>> topGames = null;
            try
            {
                topGames = await AppConstants.Twixel.RetrieveTopGames(0, 10);
            }
            catch (TwixelException ex)
            {
                await HelperMethods.ShowErrorDialog(ex);
            }
            if (topGames != null)
            {
                foreach (Game game in topGames.wrapped)
                {
                    topGamesCollection.Add(new GameGridViewBinding(game));
                }
            }
            progressRing.IsActive = false;
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

        private async void playButton_Click(object sender, RoutedEventArgs e)
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
                featuredStreamMediaElement.Source = await HelperMethods.GetPreferredQuality(selectedQuality);
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
            featuredStreamMediaElement.Source = null;
            streamButton.IsEnabled = true;
            playButton.IsEnabled = true;
            streamOfflineTextBlock.Visibility = Visibility.Collapsed;
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

        private void topGamesGridView_Loaded(object sender, RoutedEventArgs e)
        {
            topGamesGridView.ItemsSource = topGamesCollection;
        }

        private void topGamesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            GameGridViewBinding gameItem = e.ClickedItem as GameGridViewBinding;
            Frame.Navigate(typeof(GameStreamsPage), gameItem.game);
        }
    }
}
