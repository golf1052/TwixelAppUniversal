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
    public sealed partial class GamesPage : Page
    {
        ItemLoader gamesLoader;

        ObservableCollection<GameGridViewBinding> gamesCollection;

        public GamesPage()
        {
            this.InitializeComponent();

            gamesLoader = new ItemLoader(LoadGames, scrollViewer, progressBar);
            gamesCollection = new ObservableCollection<GameGridViewBinding>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await LoadGames();
            base.OnNavigatedTo(e);
        }

        async Task LoadGames()
        {
            gamesLoader.StartLoading();
            Total<List<Game>> games = await AppConstants.Twixel.RetrieveTopGames(gamesLoader.Offset, 100);
            if (!gamesLoader.CheckForEnd(games.wrapped))
            {
                foreach (Game game in games.wrapped)
                {
                    gamesCollection.Add(new GameGridViewBinding(game));
                }
                gamesLoader.EndLoading(100);
            }
        }

        private void gamesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            GameGridViewBinding gameItem = e.ClickedItem as GameGridViewBinding;
            Frame.Navigate(typeof(GameStreamsPage), gameItem.game);
        }

        private void gamesGridView_Loaded(object sender, RoutedEventArgs e)
        {
            gamesGridView.ItemsSource = gamesCollection;
        }
    }
}
