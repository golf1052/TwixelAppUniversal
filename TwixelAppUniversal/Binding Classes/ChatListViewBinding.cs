using TwixelChat;

namespace TwixelAppUniversal
{
    public class ChatListViewBinding
    {
        public string Username { get; set; }
        public string Color { get; set; }
        public string Message { get; set; }

        public ChatListViewBinding(ChatMessage message)
        {
            if (message.User != null)
            {
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
                Username = message.Username;
                Color = "#000000";
            }
            Message = message.Message;
        }
    }
}
