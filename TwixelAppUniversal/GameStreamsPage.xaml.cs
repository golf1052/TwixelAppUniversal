using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class GameStreamsPage : Page
    {
        bool currentlyLoading;
        bool endOfList;
        int offset;

        ObservableCollection<GameStreamsGridViewBinding> streamsCollection;
        Game game;

        public GameStreamsPage()
        {
            this.InitializeComponent();
            currentlyLoading = true;
            endOfList = false;
            offset = 0;
            streamsCollection = new ObservableCollection<GameStreamsGridViewBinding>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            game = e.Parameter as Game;
            BitmapImage gameImage = new BitmapImage(game.logo["medium"]);
            gameLogo.Source = gameImage;
            gameName.Text = game.name;
            if (game.channels.HasValue)
            {
                gameChannels.Text = game.channels.Value.ToString();
            }
            if (game.viewers.HasValue)
            {
                gameViewers.Text = game.viewers.Value.ToString();
            }

            streamsCollection = new ObservableCollection<GameStreamsGridViewBinding>();
            await LoadStreams();
            
            base.OnNavigatedTo(e);
        }
        
        async Task LoadStreams()
        {
            currentlyLoading = true;
            Total<List<Stream>> streams = await AppConstants.Twixel.RetrieveStreams(game.name, new List<string>(), offset, 100);
            if (streams.wrapped.Count == 0)
            {
                endOfList = true;
                currentlyLoading = false;
                return;
            }
            foreach (Stream stream in streams.wrapped)
            {
                streamsCollection.Add(new GameStreamsGridViewBinding(stream));
            }
            offset += 100;
            currentlyLoading = false;
        }

        private void gameStreamsGridView_Loaded(object sender, RoutedEventArgs e)
        {
            gameStreamsGridView.ItemsSource = streamsCollection;
        }

        private async void gameStreamsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            GameStreamsGridViewBinding streamItem = e.ClickedItem as GameStreamsGridViewBinding;
            List<object> parameters = new List<object>();
            parameters.Add(streamItem.stream);
            Dictionary<AppConstants.StreamQuality, Uri> qualities = await HelperMethods.RetrieveHlsStream(streamItem.stream.channel.name);
            parameters.Add(qualities);
            Frame.Navigate(typeof(StreamPage), parameters);
        }

        private async void scrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (scrollViewer.ScrollableWidth == scrollViewer.HorizontalOffset)
            {
                if (!currentlyLoading && !endOfList)
                {
                    await LoadStreams();
                }
            }
        }
    }
}
