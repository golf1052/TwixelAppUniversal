using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwixelChat;
using TwixelChat.Universal;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace TwixelAppUniversal
{
    public class ChatWindow
    {
        // UI Elements
        ListView chatListView;
        ScrollViewer scrollViewer;
        TextBox chatTextBox;
        Button chatSendButton;

        ChatClient chatClient;
        ObservableCollection<ChatListViewBinding> chatMessages;
        private CoreDispatcher Dispatcher { get; set; }
        
        string channelName;

        bool loadedChatView;
        bool canChat;
        Random random;

        public ChatWindow(CoreDispatcher dispatcher,
            string channelName,
            ListView chatView,
            ScrollViewer scrollViewer,
            TextBox chatBox,
            Button chatButton)
        {
            Dispatcher = dispatcher;
            this.channelName = channelName;

            chatListView = chatView;
            this.scrollViewer = scrollViewer;
            chatTextBox = chatBox;
            chatSendButton = chatButton;

            loadedChatView = false;
            canChat = false;
            random = new Random();
            
            chatMessages = new ObservableCollection<ChatListViewBinding>();

            chatListView.Loaded += ChatListView_Loaded;
            chatTextBox.KeyDown += ChatTextBox_KeyDown;
            chatSendButton.Click += ChatSendButton_Click;

            chatClient = new ChatClient();
            chatClient.MessageRecieved += ChatClient_MessageRecieved;
        }

        private async void ChatSendButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await SendChatMessage();
        }

        private async void ChatTextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                await SendChatMessage();
            }
        }

        private void ChatListView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!loadedChatView)
            {
                loadedChatView = true;
                LoadChatView();
            }
        }

        private async void ChatClient_MessageRecieved(object sender, TwixelChat.Events.MessageRecievedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                chatMessages.Add(new ChatListViewBinding(e.ChatMessage, chatClient.Channel.ChannelName));
                //scrollViewer.ChangeView(null, scrollViewer.ScrollableHeight, null);
            });
        }

        public async Task LoadChatWindow()
        {
            if (chatListView != null)
            {
                if (!loadedChatView)
                {
                    loadedChatView = true;
                    LoadChatView();
                }
            }

            if (AppConstants.activeUser != null)
            {
                if (AppConstants.activeUser.authorized)
                {
                    if (AppConstants.activeUser.authorizedScopes.Contains(TwixelAPI.Constants.TwitchConstants.Scope.ChatLogin))
                    {
                        canChat = true;
                        await chatClient.Connect(AppConstants.activeUser.name, AppConstants.activeUser.accessToken);
                        await chatClient.JoinChannel(channelName);
                        return;
                    }
                }
            }

            await ConnectAnon();
        }

        private async Task ConnectAnon()
        {
            canChat = false;
            chatTextBox.Text = "Not logged in";
            chatTextBox.IsEnabled = false;
            chatSendButton.IsEnabled = false;
            int randomNumber = random.Next(100000000, 1000000000);
            await chatClient.Connect("justinfan" + randomNumber.ToString(), string.Empty);
            await chatClient.JoinChannel(channelName);
        }

        private async Task SendChatMessage()
        {
            if (canChat && chatTextBox.Text != string.Empty)
            {
                string tmp = chatTextBox.Text;
                chatTextBox.Text = string.Empty;
                await chatClient.SendMessage(tmp);
                chatMessages.Add(new ChatListViewBinding(chatClient.Channel.ChannelUserState, channelName, AppConstants.activeUser.name, tmp));
                //scrollViewer.ChangeView(null, scrollViewer.ScrollableHeight, null);
            }
        }

        private void LoadChatView()
        {
            chatListView.ItemsSource = chatMessages;
        }
    }
}
