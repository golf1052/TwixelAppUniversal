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
            LoadUser(message.User, channel, message.Username);

            if (message.Emotes == null || message.Emotes.Count == 0)
            {
                AddText(message.Message);
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
                    AddText(message.Message.Substring(currentPosition, (int)emote.Key - currentPosition));
                    Emote e = null;
                    try
                    {
                        e = EmoteManager.EmotesById[emote.Value.Id];
                    }
                    catch
                    {

                    }
                    if (e != null)
                    {
                        AddImage(e);
                        var position = FindTuple(emote.Value.Positions, emote.Key);
                        currentPosition = (int)position.Item2 + 1;
                    }
                    else
                    {
                        currentPosition = (int)emote.Key;
                        var position = FindTuple(emote.Value.Positions, emote.Key);
                        AddText(message.Message.Substring(currentPosition, ((int)position.Item2 + 1) - (int)position.Item1));
                        currentPosition = (int)position.Item2 + 1;
                    }
                }
                if (currentPosition < message.Message.Length)
                {
                    AddText(message.Message.Substring(currentPosition));
                }
            }
        }

        public ChatListViewBinding(UserState user, string channel, string username, string message)
        {
            ChatThings = new ObservableCollection<UIElement>();
            LoadUser(user, channel, username);

            string[] splitMessage = message.Split(' ');
            if (splitMessage.Length == 1)
            {
                Emote emote = null;
                try
                {
                    emote = EmoteManager.EmotesByCode[splitMessage[0]];
                }
                catch
                {
                    AddText(splitMessage[0]);
                }
                if (emote != null)
                {
                    AddImage(emote);
                }
            }
            else
            {
                string accumulatedString = string.Empty;
                foreach (string s in splitMessage)
                {
                    Emote emote = null;
                    try
                    {
                        emote = EmoteManager.EmotesByCode[s];
                    }
                    catch
                    {
                        accumulatedString += s + " ";
                    }
                    if (emote != null)
                    {
                        if (accumulatedString != string.Empty)
                        {
                            AddText(accumulatedString);
                            accumulatedString = string.Empty;
                        }
                        AddImage(emote);
                    }
                }

                if (accumulatedString != string.Empty)
                {
                    AddText(accumulatedString);
                    accumulatedString = string.Empty;
                }
            }
        }

        private void LoadUser(UserState user, string channel, string defaultUsername)
        {
            if (user != null)
            {
                if (user.UserType == UserState.UserTypes.Admin)
                {
                    UserType = "Assets/Chat/admin-icon.png";
                }
                else if (user.UserType == UserState.UserTypes.GlobalMod)
                {
                    UserType = "Assets/Chat/globalmod-icon.png";
                }
                else if (user.UserType == UserState.UserTypes.Mod)
                {
                    UserType = "Assets/Chat/mod-icon.png";
                }
                else if (user.UserType == UserState.UserTypes.Staff)
                {
                    UserType = "Assets/Chat/staff-icon.png";
                }
                else if (user.UserType == UserState.UserTypes.None)
                {
                    UserType = null;
                }

                if (defaultUsername.ToLower() == channel.ToLower())
                {
                    Broadcaster = "Assets/Chat/broadcaster-icon.png";
                }
                else
                {
                    Broadcaster = null;
                }

                if (user.Turbo)
                {
                    Turbo = "Assets/Chat/turbo-icon.png";
                }
                else
                {
                    Turbo = null;
                }

                if (user.Subscriber)
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

                if (string.IsNullOrEmpty(user.DisplayName))
                {
                    Username = defaultUsername;
                }
                else
                {
                    Username = user.DisplayName;
                }

                if (string.IsNullOrEmpty(user.Color))
                {
                    Color = "#000000";
                }
                else
                {
                    Color = user.Color;
                }
            }
            else
            {
                UserType = null;
                Broadcaster = null;
                Turbo = null;
                Subscriber = null;
                Username = defaultUsername;
                Color = "#000000";
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

        private void AddText(string str)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = str;
            textBlock.TextWrapping = TextWrapping.WrapWholeWords;
            ChatThings.Add(textBlock);
        }

        private void AddImage(Emote emote)
        {
            if (emote != null)
            {
                Image image = new Image();
                BitmapImage bitmapImage = new BitmapImage(emote.Small);
                image.Source = bitmapImage;
                image.Stretch = Windows.UI.Xaml.Media.Stretch.None;
                ChatThings.Add(image);
            }
        }
    }
}
