using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TwixelAPI;
using TwixelChat.Universal;
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
using TwixelChat;
using TwixelChat.Events;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TwixelAppUniversal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StreamPage : Page
    {
        bool doneLoading = false;
        Stream stream;
        Dictionary<AppConstants.StreamQuality, Uri> qualities;
        bool showBars;

        ChatWindow chatWindow;

        ObservableCollection<ChatListViewBinding> messages;

        public StreamPage()
        {
            this.InitializeComponent();

            showBars = true;
            messages = new ObservableCollection<ChatListViewBinding>();
            chatListView.ItemsSource = messages;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            List<object> parameters = (List<object>)e.Parameter;
            stream = (Stream)parameters[0];
            qualities = (Dictionary<AppConstants.StreamQuality, Uri>)parameters[1];

            chatWindow = new ChatWindow(Dispatcher, stream.channel.name, chatListView, scrollViewer, chatBox, sendButton);
            await chatWindow.LoadChatWindow();
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
            if (qualities.ContainsKey(AppConstants.WifiStreamQuality))
            {
                streamElement.Source = qualities[AppConstants.WifiStreamQuality];
                SetQualityComboBox(AppConstants.WifiStreamQuality, streamQualitiesComboBox);
            }
            else
            {
                streamElement.Source = qualities[AppConstants.StreamQuality.Chunked];
                SetQualityComboBox(AppConstants.StreamQuality.Chunked, streamQualitiesComboBox);
            }
            streamElement.Play();
            base.OnNavigatedTo(e);
            doneLoading = true;
        }

        private void streamElement_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (showBars)
            {
                HideBarsAnimation.Begin();
                AppConstants.RootSplitView.DisplayMode = SplitViewDisplayMode.Overlay;
                topBar.Visibility = Visibility.Collapsed;
                bottomBar.Visibility = Visibility.Collapsed;
                showBars = false;
            }
            else
            {
                ShowBarsAnimation.Begin();
                AppConstants.RootSplitView.DisplayMode = SplitViewDisplayMode.CompactOverlay;
                topBar.Visibility = Visibility.Visible;
                bottomBar.Visibility = Visibility.Visible;
                showBars = true;
            }
        }

        private void streamQualitiesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (doneLoading)
            {
                streamElement.Source = qualities[HelperMethods.GetStreamQuality((string)((ComboBoxItem)streamQualitiesComboBox.SelectedItem).Content)];
                streamElement.Play();
            }
        }

        private void SetQualityComboBox(AppConstants.StreamQuality quality, ComboBox comboBox)
        {
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                ComboBoxItem item = comboBox.Items[i] as ComboBoxItem;
                if (item != null)
                {
                    if ((string)item.Content == HelperMethods.GetStreamQualityString(quality))
                    {
                        comboBox.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void chatButton_Click(object sender, RoutedEventArgs e)
        {
            if (chatGrid.Visibility == Visibility.Collapsed)
            {
                chatGrid.Visibility = Visibility.Visible;
            }
            else
            {
                chatGrid.Visibility = Visibility.Collapsed;
            }
        }
    }
}
