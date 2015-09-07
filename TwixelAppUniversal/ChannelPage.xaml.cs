using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TwixelAPI;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
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
    public sealed partial class ChannelPage : Page
    {
        Channel channel;
        ItemLoader videosLoader;

        ObservableCollection<VideosGridViewBinding> videosCollection;

        public ChannelPage()
        {
            this.InitializeComponent();

            videosLoader = new ItemLoader(LoadVideos, scrollViewer, progressBar);
            videosCollection = new ObservableCollection<VideosGridViewBinding>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            channel = e.Parameter as Channel;
            if (channel.logo != null)
            {
                channelImage.Fill = new ImageBrush() { ImageSource = new BitmapImage(channel.logo) };
            }

            channelNameTextBlock.Text = channel.displayName;
            if (channel.game == null)
            {
                playingTextBlock.Visibility = Visibility.Collapsed;
                gameTextBlock.Text = string.Empty;
            }
            else
            {
                gameTextBlock.Text = channel.game;
            }

            if (channel.profileBanner != null)
            {
                BitmapImage image = new BitmapImage(channel.profileBanner);
                bannerImage.Source = image;
            }
            else if (channel.banner != null)
            {
                BitmapImage image = new BitmapImage(channel.banner);
                bannerImage.Source = image;
            }

            await LoadVideos();
            base.OnNavigatedTo(e);
        }

        async Task LoadVideos()
        {
            videosLoader.StartLoading();
            Total<List<Video>> videos = await AppConstants.Twixel.RetrieveVideos(channel.name, videosLoader.Offset, 100);
            if (!videosLoader.CheckForEnd(videos.wrapped))
            {
                foreach (Video video in videos.wrapped)
                {
                    videosCollection.Add(new VideosGridViewBinding(video));
                }
                videosLoader.EndLoading(100);
            }
        }

        private void videosGridView_Loaded(object sender, RoutedEventArgs e)
        {
            videosGridView.ItemsSource = videosCollection;
        }

        private async void videosGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            VideosGridViewBinding videoItem = e.ClickedItem as VideosGridViewBinding;
            LauncherOptions options = new LauncherOptions();
            options.DesiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.UseHalf;
            await Launcher.LaunchUriAsync(new Uri(videoItem.Url), options);
        }
    }
}
