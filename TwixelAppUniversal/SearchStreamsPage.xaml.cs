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
    public sealed partial class SearchStreamsPage : Page
    {
        ItemLoader searchLoader;

        ObservableCollection<GameStreamsGridViewBinding> streamsCollection;

        string searchQuery;

        public SearchStreamsPage()
        {
            this.InitializeComponent();

            searchLoader = new ItemLoader(LoadSearch, scrollViewer, progressBar);
            streamsCollection = new ObservableCollection<GameStreamsGridViewBinding>();
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
            searchLoader.StartLoading();
            Total<List<Stream>> searchedStreams = await AppConstants.Twixel.SearchStreams(searchQuery, searchLoader.Offset, 100);
            if (searchedStreams.total.HasValue)
            {
                numberOfStreamsTextBlock.Text = searchedStreams.total.Value.ToString();
            }
            
            if (!searchLoader.CheckForEnd(searchedStreams.wrapped))
            {
                foreach (Stream stream in searchedStreams.wrapped)
                {
                    streamsCollection.Add(new GameStreamsGridViewBinding(stream));
                }
                searchLoader.EndLoading(100);
            }
        }

        private void streamsGridView_Loaded(object sender, RoutedEventArgs e)
        {
            streamsGridView.ItemsSource = streamsCollection;
        }

        private async void streamsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            await HelperMethods.GoToStreamPage(e, Frame);
        }
    }
}
