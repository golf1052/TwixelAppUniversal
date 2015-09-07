using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TwixelAPI;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class UserPage : Page
    {
        User user;
        Channel channel;
        Stream stream;
        Dictionary<AppConstants.StreamQuality, Uri> qualities;

        ItemLoader followedOnlineStreamsLoader;
        ItemLoader followingChannelsLoader;
        ItemLoader blockedUsersLoader;

        ObservableCollection<GameStreamsGridViewBinding> followedOnlineStreamsCollection;
        ObservableCollection<ChannelProfileListViewBinding> followingChannelsCollection;
        ObservableCollection<ChannelProfileListViewBinding> blockedUsersCollection;
        ObservableCollection<ChannelProfileListViewBinding> channelEditorsCollection;

        bool videoPlaying;

        public UserPage()
        {
            this.InitializeComponent();
            channel = null;
            qualities = null;

            followedOnlineStreamsLoader = new ItemLoader(LoadOnlineStreams, followedStreamsScrollViewer, followedStreamsProgressBar);
            followingChannelsLoader = new ItemLoader(LoadFollowingChannels, followedChannelsScrollViewer, followedChannelsProgressBar);
            blockedUsersLoader = new ItemLoader(LoadBlockedUsers, blockedUsersScrollViewer, blockedUsersProgressBar);
            followedOnlineStreamsCollection = new ObservableCollection<GameStreamsGridViewBinding>();
            followingChannelsCollection = new ObservableCollection<ChannelProfileListViewBinding>();
            blockedUsersCollection = new ObservableCollection<ChannelProfileListViewBinding>();
            channelEditorsCollection = new ObservableCollection<ChannelProfileListViewBinding>();
            videoPlaying = false;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (AppConstants.activeUser.authorized)
            {
                user = AppConstants.activeUser;
                try
                {
                    channel = await user.RetrieveChannel();
                }
                catch (TwixelException ex)
                {
                }
                if (channel != null)
                {
                    statusTextBox.Text = channel.status;
                    if (channel.game != null)
                    {
                        gameTextBox.Text = channel.game;
                    }
                    await PlayStream();
                }

                streamKeyTextBox.Text = user.streamKey;
                await LoadOnlineStreams();
                await LoadFollowingChannels();
                await LoadBlockedUsers();
                await LoadChannelEditors();
            }
            base.OnNavigatedTo(e);
        }

        async Task LoadOnlineStreams()
        {
            followedOnlineStreamsLoader.StartLoading();
            List<Stream> onlineStreams = await user.RetrieveOnlineFollowedStreams(followedOnlineStreamsLoader.Offset, 100);
            if (!followedOnlineStreamsLoader.CheckForEnd(onlineStreams))
            {
                foreach (Stream onlineStream in onlineStreams)
                {
                    followedOnlineStreamsCollection.Add(new GameStreamsGridViewBinding(onlineStream));
                }
                followedOnlineStreamsLoader.EndLoading(100);
            }
        }

        async Task LoadFollowingChannels()
        {
            followingChannelsLoader.StartLoading();
            Total<List<Follow<Channel>>> followingChannels = await user.RetrieveFollowing(followingChannelsLoader.Offset, 100);
            if (!followingChannelsLoader.CheckForEnd(followingChannels.wrapped))
            {
                foreach (Follow<Channel> follow in followingChannels.wrapped)
                {
                    followingChannelsCollection.Add(new ChannelProfileListViewBinding(follow.wrapped.logo, follow.wrapped.displayName, follow.wrapped));
                }
                followingChannelsLoader.EndLoading(100);
            }
        }

        async Task LoadBlockedUsers()
        {
            blockedUsersLoader.StartLoading();
            List<Block> blockedUsers = await user.RetrieveBlockedUsers(blockedUsersLoader.Offset, 100);
            if (!blockedUsersLoader.CheckForEnd(blockedUsers))
            {
                foreach (Block block in blockedUsers)
                {
                    blockedUsersCollection.Add(new ChannelProfileListViewBinding(block.user));
                }
                blockedUsersLoader.EndLoading(100);
            }
        }

        async Task LoadChannelEditors()
        {
            List<User> channelEditors = await user.RetrieveChannelEditors();
            foreach (User channelEditor in channelEditors)
            {
                channelEditorsCollection.Add(new ChannelProfileListViewBinding(channelEditor));
            }
        }

        async Task PlayStream()
        {
            try
            {
                qualities = await HelperMethods.RetrieveHlsStream("riotgames");
            }
            catch (Exception ex)
            {
                qualities = null;
                streamOfflineTextBlock.Visibility = Visibility.Visible;
                ((SymbolIcon)playPauseButton.Icon).Symbol = Symbol.Play;
                await HelperMethods.ShowMessageDialog("Looks like you're not streaming currently.", "Stream offline");
            }
            if (qualities != null)
            {
                //stream = await AppConstants.Twixel.RetrieveStream(channel.name);
                streamPlayer.Source = qualities[AppConstants.StreamQuality.Chunked];
                ((SymbolIcon)playPauseButton.Icon).Symbol = Symbol.Stop;
                videoPlaying = true;
            }
        }

        private async void playPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (videoPlaying)
            {
                streamPlayer.Source = null;
                ((SymbolIcon)playPauseButton.Icon).Symbol = Symbol.Play;
                videoPlaying = false;
            }
            else
            {
                await PlayStream();
            }
        }

        private void followedStreamsListView_Loaded(object sender, RoutedEventArgs e)
        {
            followedStreamsListView.ItemsSource = followedOnlineStreamsCollection;
        }

        private async void followedStreamsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            await HelperMethods.GoToStreamPage(e, Frame);
        }

        private void followedChannelsListView_Loaded(object sender, RoutedEventArgs e)
        {
            followedChannelsListView.ItemsSource = followingChannelsCollection;
        }

        private void followedChannelsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
        }

        private void blockedUsersListView_Loaded(object sender, RoutedEventArgs e)
        {
            blockedUsersListView.ItemsSource = blockedUsersCollection;
        }

        private void blockedUsersListView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void channelEditorsListView_Loaded(object sender, RoutedEventArgs e)
        {
            channelEditorsListView.ItemsSource = channelEditorsCollection;
        }

        private void channelEditorsListView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private async void resetKeyButton_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog warning = new MessageDialog("You are about to reset your stream key. This will make your old stream key invalid.\n\nAre you sure you want to reset your stream key?", "Reset Stream Key");
            warning.Commands.Add(new UICommand("Yes", async (c) =>
                {
                    string newStreamKey = await user.ResetStreamKey();
                    streamKeyTextBox.Text = newStreamKey;
                }));
            warning.Commands.Add(new UICommand("No"));
            await warning.ShowAsync();
        }

        private async void showHideKeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (streamKeyTextBox.Visibility == Visibility.Collapsed)
            {
                MessageDialog warning = new MessageDialog("Never share your stream key with anyone or show it on stream! Twitch Staff, Admins, or Global Moderators will never ask you for this information.\n\nPlease click \"I Understand\" if you understand the above and would like to view your stream key.", "Show Stream Key");
                warning.Commands.Add(new UICommand("I Understand", (c) =>
                    {
                        streamKeyTextBox.Visibility = Visibility.Visible;
                        showHideKeyButton.Content = "Hide Key";
                    }));
                warning.Commands.Add(new UICommand("Cancel"));
                await warning.ShowAsync();
            }
            else
            {
                showHideKeyButton.Content = "Show Key";
                streamKeyTextBox.Visibility = Visibility.Collapsed;
            }
        }
    }
}
