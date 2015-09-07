using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwixelAPI;

namespace TwixelAppUniversal
{
    public class ChannelProfileListViewBinding
    {
        public Uri ProfileImage { get; set; }
        public string Name { get; set; }
        public Channel Channel { get; set; }

        public ChannelProfileListViewBinding(Uri profileImage, string name, Channel channel)
        {
            if (profileImage != null)
            {
                ProfileImage = profileImage;
            }
            else
            {
                ProfileImage = new Uri("ms-appx:///Assets/defaultFollowerPicture.png");
            }
            Name = name;
            Channel = channel;
        }

        public ChannelProfileListViewBinding(User user)
        {
            Name = user.displayName;
            if (user.logo != null)
            {
                ProfileImage = user.logo;
            }
            else
            {
                ProfileImage = new Uri("ms-appx:///Assets/defaultFollowerPicture.png");
            }
        }
    }
}
