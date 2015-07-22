using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwixelAPI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace TwixelAppUniversal
{
    public class AppConstants
    {
        public static Twixel Twixel;
        public static Dictionary<string, string> users;
        public static User activeUser;
        private static readonly SolidColorBrush themeColor;
        public static SolidColorBrush ThemeColor { get { return themeColor; } }
        public static SplitView RootSplitView;

        public enum StreamQuality
        {
            Source,
            High,
            Medium,
            Low,
            Mobile,
            Chunked
        }

        public static StreamQuality WifiStreamQuality { get; set; }
        public static StreamQuality CellStreamQuality { get; set; }

        static AppConstants()
        {
            themeColor = new SolidColorBrush(Windows.UI.Colors.CornflowerBlue);
        }
    }
}
