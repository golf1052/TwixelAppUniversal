using System;
using TwixelChat;
using Windows.UI.Xaml;
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

        public ChatListViewBinding(ChatMessage message, string channel)
        {
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
                    Subscriber = "Assets/Chat/staff-icon.png";
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
            Message = message.Message;
        }
    }
}
