using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwixelAPI;

namespace TwixelAppUniversal
{
    public class VideosGridViewBinding
    {
        public Uri Image { get; set; }
        public long Viewers { get; set; }
        public string Title { get; set; }
        public string Length { get; set; }
        public string Url { get; set; }

        public VideosGridViewBinding(string image, int viewers, string title, TimeSpan length, string url)
        {
            Image = new Uri(image);
            Viewers = viewers;
            Title = title;
            Length = length.Minutes.ToString() + ":" + length.Seconds.ToString();
            Url = url;
        }

        public VideosGridViewBinding(Video video)
        {
            if (video.preview != null)
            {
                Image = video.preview;
            }
            Viewers = video.views;
            Title = video.title;
            TimeSpan length = TimeSpan.FromSeconds(video.length);
            if (length.Seconds > 10)
            {
                Length = length.Minutes.ToString() + ":" + length.Seconds.ToString();
            }
            else
            {
                Length = length.Minutes.ToString() + ":0" + length.Seconds.ToString();
            }
            Url = video.url.ToString();
        }
    }
}
