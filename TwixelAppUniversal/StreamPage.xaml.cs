using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using TwixelAPI;
using TwixelChat;
using TwixelChat.Universal;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TwixelApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StreamPage : Page
    {
        string channel;
        ChatClient client;

        public StreamPage()
        {
            this.InitializeComponent();
            client = new ChatClient();
            client.MessageRecieved += Client_MessageRecieved;
        }

        private async void Client_MessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                chatView.Items.Add(e.Username + ": " + e.Message);
            });
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            List<object> parameters = (List<object>)e.Parameter;
            channel = (string)parameters[0];
            Stream stream = null;
            
            await client.Connect("golf1052", Secrets.AccessToken);
            try
            {
                await client.JoinChannel(channel);
            }
            catch (TimeoutException ex)
            {
                throw;
            }
            try
            {
                stream = await AppConstants.twixel.RetrieveStream(channel);
            }
            catch (TwixelException ex)
            {
                throw;
            }
            if (stream != null)
            {
                Dictionary<AppConstants.StreamQuality, Uri> qualities = await HelperMethods.RetrieveHlsStream(channel);
                streamElement.Source = qualities[AppConstants.StreamQuality.Chunked];
                streamElement.Play();
                
            }
            base.OnNavigatedTo(e);
        }
    }
}
