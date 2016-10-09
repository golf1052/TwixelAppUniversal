using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TwixelAPI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;

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
        VolumeFlyout volumeFlyout;

        ObservableCollection<ChatListViewBinding> messages;

        private bool VideoPlaying
        {
            get
            {
                if (streamElement != null)
                {
                    if (streamElement.CurrentState != MediaElementState.Paused &&
                        streamElement.CurrentState != MediaElementState.Stopped &&
                        streamElement.CurrentState != MediaElementState.Closed)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public StreamPage()
        {
            this.InitializeComponent();

            showBars = true;
            messages = new ObservableCollection<ChatListViewBinding>();
            chatListView.ItemsSource = messages;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            playPauseButton.IsEnabled = false;
            List<object> parameters = (List<object>)e.Parameter;
            stream = (Stream)parameters[0];
            qualities = (Dictionary<AppConstants.StreamQuality, Uri>)parameters[1];

            volumeFlyout = new VolumeFlyout(volumeSlider, muteButton, volumeButton, streamElement);
            chatWindow = new ChatWindow(Dispatcher, stream.channel.name, chatListView, scrollViewer, chatBox, sendButton);
            await chatWindow.LoadChatWindow();
            streamerImage.Fill = new ImageBrush() { ImageSource = new BitmapImage(stream.channel.logo) };
            await ResetStatus();
            foreach (KeyValuePair<AppConstants.StreamQuality, Uri> quality in qualities)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = HelperMethods.GetStreamQualityString(quality.Key);
                streamQualitiesComboBox.Items.Add(item);
            }
            await PlayStream();
            SetQualityComboBox(qualities.First(u => u.Value == streamElement.Source).Key, streamQualitiesComboBox);

            if (AppConstants.activeUser != null)
            {
                if (AppConstants.activeUser.authorized)
                {
                    int followingOffset = 0;
                    List<Channel> followingChannels = new List<Channel>();
                    Total<List<Follow<Channel>>> channels = null;
                    do
                    {
                        channels = await AppConstants.activeUser.RetrieveFollowing(followingOffset, 100);
                        foreach (Follow<Channel> channel in channels.wrapped)
                        {
                            followingChannels.Add(channel.wrapped);
                        }
                        followingOffset += 100;
                    }
                    while (channels.wrapped.Count != 0);
                    if (AppConstants.activeUser.authorizedScopes.Contains(TwixelAPI.Constants.TwitchConstants.Scope.UserFollowsEdit))
                    {
                        foreach (Channel channel in followingChannels)
                        {
                            if (channel.name == stream.channel.name)
                            {
                                followButton.Label = "Unfollow";
                                ((SymbolIcon)followButton.Icon).Symbol = Symbol.Clear;
                                break;
                            }
                        }
                    }
                    else
                    {
                        followButton.IsEnabled = false;
                    }
                }
                else
                {
                    followButton.IsEnabled = false;
                }
            }
            else
            {
                followButton.IsEnabled = false;
            }
            base.OnNavigatedTo(e);
            doneLoading = true;
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            await chatWindow.UnloadChatWindow();
            base.OnNavigatedFrom(e);
        }

        private async void streamElement_Tapped(object sender, TappedRoutedEventArgs e)
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
                await ResetStatus();
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

        private void channelButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ChannelPage), stream.channel);
        }

        private async void followButton_Click(object sender, RoutedEventArgs e)
        {
            if (followButton.Label == "Follow")
            {
                await AppConstants.activeUser.FollowChannel(stream.channel.name);
                followButton.Label = "Unfollow";
                ((SymbolIcon)followButton.Icon).Symbol = Symbol.Clear;
            }
            else
            {
                await AppConstants.activeUser.UnfollowChannel(stream.channel.name);
                followButton.Label = "Follow";
                ((SymbolIcon)followButton.Icon).Symbol = Symbol.Add;
            }
        }

        private async void playPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (VideoPlaying)
            {
                streamElement.Stop();
                playPauseButton.Label = "Play";
                ((SymbolIcon)playPauseButton.Icon).Symbol = Symbol.Play;
            }
            else
            {
                playPauseButton.Label = "Stop";
                ((SymbolIcon)playPauseButton.Icon).Symbol = Symbol.Stop;
                await PlayStream();
            }
        }

        private async Task PlayStream()
        {
            try
            {
                qualities = await HelperMethods.RetrieveHlsStream(stream.channel.name);
            }
            catch
            {
                streamOfflineTextBlock.Visibility = Visibility.Visible;
                playPauseButton.IsEnabled = false;
            }
            streamElement.Source = await HelperMethods.GetPreferredQuality(qualities);
            streamElement.Play();
            playPauseButton.IsEnabled = true;
        }

        private async Task ResetStatus()
        {
            try
            {
                stream = await AppConstants.Twixel.RetrieveStream(stream.channel.name);
            }
            catch (TwixelException ex)
            {
                await HelperMethods.ShowErrorDialog(ex);
                return;
            }
            streamerNameTextBlock.Text = stream.channel.displayName;
            streamDescriptionTextBlock.Text = stream.channel.status;
            gameNameTextBlock.Text = stream.game;
            if (stream.viewers.HasValue)
            {
                streamViewersTextBlock.Text = stream.viewers.Value.ToString();
            }
        }
    }
}
