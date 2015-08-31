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
    public sealed partial class StreamsPage : Page
    {
        ItemLoader streamsLoader;

        ObservableCollection<GameStreamsGridViewBinding> streamsCollection;

        public StreamsPage()
        {
            this.InitializeComponent();

            streamsLoader = new ItemLoader(LoadStreams, scrollViewer, progressBar);
            streamsCollection = new ObservableCollection<GameStreamsGridViewBinding>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await LoadStreams();
            base.OnNavigatedTo(e);
        }

        async Task LoadStreams()
        {
            streamsLoader.StartLoading();
            Total<List<Stream>> streams = await AppConstants.Twixel.RetrieveStreams(null, null, streamsLoader.Offset, 100);
            if (!streamsLoader.CheckForEnd(streams.wrapped))
            {
                foreach (Stream stream in streams.wrapped)
                {
                    streamsCollection.Add(new GameStreamsGridViewBinding(stream));
                }
                streamsLoader.EndLoading(100);
            }
        }

        private async void streamsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            GameStreamsGridViewBinding streamItem = e.ClickedItem as GameStreamsGridViewBinding;
            List<object> parameters = new List<object>();
            parameters.Add(streamItem.stream);
            Dictionary<AppConstants.StreamQuality, Uri> qualities = await HelperMethods.RetrieveHlsStream(streamItem.stream.channel.name);
            parameters.Add(qualities);
            Frame.Navigate(typeof(StreamPage), parameters);
        }

        private void streamsGridView_Loaded(object sender, RoutedEventArgs e)
        {
            streamsGridView.ItemsSource = streamsCollection;
        }
    }
}
