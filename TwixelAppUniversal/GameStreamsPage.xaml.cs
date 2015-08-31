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
        ItemLoader streamsLoader;

        ObservableCollection<GameStreamsGridViewBinding> streamsCollection;
        Game game;

        public GameStreamsPage()
        {
            this.InitializeComponent();
            streamsLoader = new ItemLoader(LoadStreams, scrollViewer, progressBar);
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

            await LoadStreams();
            
            base.OnNavigatedTo(e);
        }
        
        async Task LoadStreams()
        {
            streamsLoader.StartLoading();
            Total<List<Stream>> streams = await AppConstants.Twixel.RetrieveStreams(game.name, new List<string>(), streamsLoader.Offset, 100);
            if (!streamsLoader.CheckForEnd(streams.wrapped))
            {
                foreach (Stream stream in streams.wrapped)
                {
                    streamsCollection.Add(new GameStreamsGridViewBinding(stream));
                }
                streamsLoader.EndLoading(100);
            }
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
            Dictionary<AppConstants.StreamQuality, Uri> qualities = null;
            try
            {
                qualities = await HelperMethods.RetrieveHlsStream(streamItem.stream.channel.name);
            }
            catch (Exception ex)
            {
                await HelperMethods.ShowMessageDialog(string.Format("Looks like {0} is offline. Sorry about that.", streamItem.stream.channel.displayName), string.Format("{0} is offline", streamItem.stream.channel.displayName));
            }
            parameters.Add(qualities);
            Frame.Navigate(typeof(StreamPage), parameters);
        }
    }
}
