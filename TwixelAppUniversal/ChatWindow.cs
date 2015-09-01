using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwixelChat.Universal;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace TwixelAppUniversal
{
    public class ChatWindow
    {
        ChatClient chatClient;
        ObservableCollection<ChatListViewBinding> chatMessages;
        private CoreDispatcher Dispatcher { get; set; }
        ListView chatListView;
        ScrollViewer scrollViewer;
        string channelName;

        bool loadedChatView;

        public ChatWindow(CoreDispatcher dispatcher,
            string channelName,
            ListView chatView)
        {
            Dispatcher = dispatcher;
            this.channelName = channelName;
            chatListView = chatView;

            loadedChatView = false;
            
            chatMessages = new ObservableCollection<ChatListViewBinding>();

            chatListView.Loaded += ChatListView_Loaded;

            chatClient = new ChatClient();
            chatClient.MessageRecieved += ChatClient_MessageRecieved;
        }

        private void ChatListView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!loadedChatView)
            {
                loadedChatView = true;
                LoadChatView();
            }
        }

        private void LoadChatView()
        {
            chatListView.ItemsSource = chatMessages;
        }

        private void ChatClient_MessageRecieved(object sender, TwixelChat.Events.MessageRecievedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
