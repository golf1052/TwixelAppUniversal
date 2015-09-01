using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TwixelChat;
using TwixelEmotes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace TwixelAppUniversal
{
    public class ChatListViewBinding
    {
        public string UserType { get; set; }
        public string Broadcaster { get; set; }
        public string Turbo { get; set; }
        public string Subscriber { get; set; }
        public Thickness UsernamePadding { get; set; }
        public string Username { get; set; }
        public string Color { get; set; }
        public string Message { get; set; }
        public ObservableCollection<UIElement> ChatThings { get; set; }

        public ChatListViewBinding(ChatMessage message, string channel)
        {
            ChatThings = new ObservableCollection<UIElement>();
            if (message.User != null)
            {
                if (message.User.UserType == UserState.UserTypes.Admin)
                {
                    UserType = "Assets/Chat/admin-icon.png";
                }
                else if (message.User.UserType == UserState.UserTypes.GlobalMod)
                {
                    UserType = "Assets/Chat/globalmod-icon.png";
                }
                else if (message.User.UserType == UserState.UserTypes.Mod)
                {
                    UserType = "Assets/Chat/mod-icon.png";
                }
                else if (message.User.UserType == UserState.UserTypes.Staff)
                {
                    UserType = "Assets/Chat/staff-icon.png";
                }
                else if (message.User.UserType == UserState.UserTypes.None)
                {
                    UserType = null;
                }

                if (message.Username.ToLower() == channel.ToLower())
                {
                    Broadcaster = "Assets/Chat/broadcaster-icon.png";
                }
                else
                {
                    Broadcaster = null;
                }

                if (message.User.Turbo)
                {
                    Turbo = "Assets/Chat/turbo-icon.png";
                }
                else
                {
                    Turbo = null;
                }

                if (message.User.Subscriber)
                {
                    Subscriber = "https:" + EmoteManager.ChannelsByName[channel].Badge;
                }
                else
                {
                    Subscriber = null;
                }

                if (UserType == null && Broadcaster == null && Turbo == null && Subscriber == null)
                {
                    UsernamePadding = new Thickness(0);
                }
                else
                {
                    UsernamePadding = new Thickness(5, 0, 0, 0);
                }

                if (string.IsNullOrEmpty(message.User.DisplayName))
                {
                    Username = message.Username;
                }
                else
                {
                    Username = message.User.DisplayName;
                }

                if (string.IsNullOrEmpty(message.User.Color))
                {
                    Color = "#000000";
                }
                else
                {
                    Color = message.User.Color;
                }
            }
            else
            {
                UserType = null;
                Broadcaster = null;
                Turbo = null;
                Subscriber = null;
                Username = message.Username;
                Color = "#000000";
            }

            if (message.Emotes == null || message.Emotes.Count == 0)
            {
                TextBlock textBlockMessage = new TextBlock();
                textBlockMessage.Text = message.Message;
                textBlockMessage.TextWrapping = TextWrapping.WrapWholeWords;
                ChatThings.Add(textBlockMessage);
            }
            else
            {
                SortedDictionary<long, ChatEmote> sortedEmotes = new SortedDictionary<long, ChatEmote>();
                foreach (ChatEmote emote in message.Emotes)
                {
                    foreach (Tuple<long, long> position in emote.Positions)
                    {
                        sortedEmotes.Add(position.Item1, emote);
                    }
                }
                int currentPosition = 0;
                foreach (KeyValuePair<long, ChatEmote> emote in sortedEmotes)
                {
                    TextBlock textBlockMessage = new TextBlock();
                    textBlockMessage.Text = message.Message.Substring(currentPosition, (int)emote.Key - currentPosition);
                    textBlockMessage.TextWrapping = TextWrapping.WrapWholeWords;
                    ChatThings.Add(textBlockMessage);
                    Image image = new Image();
                    BitmapImage bitmapImage = new BitmapImage(EmoteManager.EmotesById[emote.Value.Id].Small);
                    image.Source = bitmapImage;
                    image.Stretch = Windows.UI.Xaml.Media.Stretch.None;
                    ChatThings.Add(image);
                    var position = FindTuple(emote.Value.Positions, emote.Key);
                    currentPosition = (int)position.Item2 + 1;
                }
                if (currentPosition < message.Message.Length)
                {
                    TextBlock textBlockMessage = new TextBlock();
                    textBlockMessage.Text = message.Message.Substring(currentPosition);
                    textBlockMessage.TextWrapping = TextWrapping.WrapWholeWords;
                    ChatThings.Add(textBlockMessage);
                }
            }
        }

        private Tuple<long, long> FindTuple(List<Tuple<long, long>> tuples, long key)
        {
            foreach (var tuple in tuples)
            {
                if (tuple.Item1 == key)
                {
                    return tuple;
                }
            }
            throw new Exception("Couldn't find tuple");
        }
    }
}
