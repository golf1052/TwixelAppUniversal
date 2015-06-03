﻿using System;
using System.Collections.Generic;
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
    public sealed partial class StreamPage : Page
    {
        Stream stream;
        Dictionary<AppConstants.StreamQuality, Uri> qualities;
        bool showBars;

        public StreamPage()
        {
            this.InitializeComponent();

            showBars = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            List<object> parameters = (List<object>)e.Parameter;
            stream = (Stream)parameters[0];
            qualities = (Dictionary<AppConstants.StreamQuality, Uri>)parameters[1];
            streamerImage.Fill = new ImageBrush() { ImageSource = new BitmapImage(stream.channel.logo) };
            streamerNameTextBlock.Text = stream.channel.displayName;
            streamDescriptionTextBlock.Text = stream.channel.status;
            gameNameTextBlock.Text = stream.game;
            if (stream.viewers.HasValue)
            {
                streamViewersTextBlock.Text = stream.viewers.Value.ToString();
            }
            foreach (KeyValuePair<AppConstants.StreamQuality, Uri> quality in qualities)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = HelperMethods.GetStreamQualityString(quality.Key);
                streamQualitiesComboBox.Items.Add(item);
            }
            if (qualities.ContainsKey(AppConstants.StreamQuality.Mobile))
            {
                streamElement.Source = qualities[AppConstants.StreamQuality.Mobile];
            }
            else
            {
                streamElement.Source = qualities[AppConstants.StreamQuality.Chunked];
            }
            streamElement.Play();

            base.OnNavigatedTo(e);
        }

        private void streamElement_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (showBars)
            {
                topBar.Visibility = Visibility.Collapsed;
                bottomBar.Visibility = Visibility.Collapsed;
                showBars = false;
            }
            else
            {
                topBar.Visibility = Visibility.Visible;
                bottomBar.Visibility = Visibility.Visible;
                showBars = true;
            }
        }
    }
}
