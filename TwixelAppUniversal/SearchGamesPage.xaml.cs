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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TwixelAppUniversal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchGamesPage : Page
    {
        ObservableCollection<GameGridViewBinding> gamesCollection;

        string searchQuery;

        bool clickedItem;

        public SearchGamesPage()
        {
            this.InitializeComponent();
            gamesCollection = new ObservableCollection<GameGridViewBinding>();
            clickedItem = false;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            searchQuery = e.Parameter as string;
            searchText.Text = searchQuery;

            await LoadSearch();
            base.OnNavigatedTo(e);
        }

        async Task LoadSearch()
        {
            List<SearchedGame> searchedGames = await AppConstants.Twixel.SearchGames(searchQuery, true);
            numberOfGamesTextBlock.Text = searchedGames.Count.ToString();
            foreach (SearchedGame game in searchedGames)
            {
                gamesCollection.Add(new GameGridViewBinding(game.name, game.viewers, game.channels, game.box["medium"]));
            }
        }

        private void gamesGridView_Loaded(object sender, RoutedEventArgs e)
        {
            gamesGridView.ItemsSource = gamesCollection;
        }

        private async void gamesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!clickedItem)
            {
                clickedItem = true;
                GameGridViewBinding gameItem = e.ClickedItem as GameGridViewBinding;
                int gamesOffset = 0;
                Total<List<Game>> games = null;
                bool foundGame = false;
                do
                {
                    games = await AppConstants.Twixel.RetrieveTopGames(gamesOffset, 100);
                    foreach (Game game in games.wrapped)
                    {
                        if (game.name == gameItem.Name)
                        {
                            foundGame = true;
                            Frame.Navigate(typeof(GameStreamsPage), game);
                            break;
                        }
                    }
                    gamesOffset += 100;
                }
                while (games.wrapped.Count != 0 && !foundGame);
            }
        }
    }
}
